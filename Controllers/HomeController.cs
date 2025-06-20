
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using ToDoProject.Models;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using ToDoProject;
using ToDoProject.Graphql;
using System.ComponentModel;
using ToDoProject.Storages;
using Microsoft.Identity.Client.Extensions.Msal;
namespace WebApplication1.Controllers;
// переробити замість того щоб надсилати тип сховища  а як параметра надсилати його в headers
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    //private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TODO;Integrated Security=True;Encrypt=True";
    private readonly GraphqlService _graphqlService;
    public HomeController(ILogger<HomeController> logger, GraphqlService graphQLService)
    {
        _logger = logger;
        _graphqlService = graphQLService;
    }

   
    public async Task<IActionResult> Index()
    {
        var tasksWrapper = new TasksWrapper();

        try
        {
            var data = await _graphqlService.GetTasksAndCategories();
            var updatedTasks = data.Tasks.Select(t => new TaskModel
                {
                Id = t.Id,
                Text = t.Text,
                IsCompleted = t.IsCompleted,
                CompletedAt = t.CompletedAt,
                PlannedTime = t.PlannedTime,
                CategoryId = t.CategoryId,
                CategoryType = data.Categories
                                .FirstOrDefault(c => c.Id == t.CategoryId)
                                ?.Type
                                ?? ""
                })
            .ToList();
            data.Tasks = updatedTasks; 
            tasksWrapper = new TasksWrapper
            {
                TaskVm = data,
                Task = new TaskModel()
            };

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
        //var handler = new RequestHandler();
        //var tasksWrapperr = handler.GenerateView(DataStorages.MySQL);
        return View(tasksWrapper);  
    }

    public IActionResult XMLView()
    {
        var storage = new XMLStorage();
        var tasks = storage.GetTasks();
        var categories = storage.GetCategories();
        var viewModel = new TaskViewModel
        {
            Tasks = tasks,
            Categories = categories
        };
        var tasksWrapper = new TasksWrapper
        {
            TaskVm = viewModel,
            Task = new TaskModel()
        };
        return View("~/Views/XML/Index.cshtml",tasksWrapper);
    }


    public IActionResult SelectStorageAndRedirect(string storage)
    {
        if (storage == "MSSQL")
        {
            Response.Cookies.Append("storage", "MSSQL", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToAction("Index");
        }
        else if (storage == "XML")
        {
            Response.Cookies.Append("storage", "XML", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToAction("XMLView");
        }

      
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Create(TasksWrapper container)
    {
        //var handler = new RequestHandler();
        string storage = Request.Cookies["storage"];
        if(storage == "XML")
        {
            var storageHandler = new XMLStorage();
            if (!ModelState.IsValid)
            {
               
                var tasks = storageHandler.GetTasks();
                var categories = storageHandler.GetCategories();
                var data = new TaskViewModel
                {
                    Tasks = tasks,
                    Categories = categories
                };

                var modelWithData = new TasksWrapper
                {
                    Task = container.Task,
                    TaskVm = data
                };
               
                return View("XMLView", modelWithData);
            }

            var newTask = storageHandler.CreateTask(container.Task);
            return RedirectToAction("XMLView");
        }
        if (!ModelState.IsValid)
        {
            var data = await _graphqlService.GetTasksAndCategories();
            var modelWithData = new TasksWrapper();
            modelWithData.Task = container.Task;
            modelWithData.TaskVm = data;
            return View("Index", modelWithData);
        }

        //handler.CreateTask(container, DataStorages.MySQL);
        var createdTask  = await _graphqlService.CreateTask(container.Task, "MSSQL");
        return RedirectToAction("Index");
    }
    

    //https://teams.live.com/l/message/19:meeting_YmQzZTc1NmItYjYzZi00OTI0LThiOWUtM2QxZmQzMDk5M2M2@thread.v2/1747405242480?context=%7B%22contextType%22%3A%22chat%22%7D
    //https://learn.microsoft.com/en-us/visualstudio/debugger/debugger-feature-tour?view=vs-2022
    //https://learn.microsoft.com/en-us/dotnet/standard/data/xml/
    //https://learn.microsoft.com/en-us/dotnet/standard/linq/linq-xml-overview
    [HttpPost]
    public async Task<IActionResult> Delete(int Id)
    {
        string storage = Request.Cookies["storage"];
        if (storage == "XML")
        {
            var storageHandler = new XMLStorage();
            var deleted = storageHandler.DeleteTask(Id);
            return RedirectToAction("XMLView");
        }

            //var handler = new RequestHandler();
            //handler.DeleteTask(Id, DataStorages.MySQL);
        var status = await _graphqlService.DeleteTask(Id, "MSSQL");
        return RedirectToAction("Index");
    }

    //public ActionResult Update(int Id,string Text) 
    //{
    //    if (string.IsNullOrWhiteSpace(Text))
    //    {
    //       // Debug.WriteLine("Text must be");
    //        return RedirectToAction("Index");
    //    }

    //    var handler = new RequestHandler();
    //    handler.UpdateTask(Id,Text);
    //    return RedirectToAction("Index");
    //}
    [HttpPost]
    public async Task<IActionResult> MarkComplete(int Id, bool IsCompleted)
    {
        //var handler  = new RequestHandler();
        //handler.MarkTask(Id,IsCompleted, DataStorages.MySQL);
        string storage = Request.Cookies["storage"];
        if (storage == "XML")
        {
            var storageHandler = new XMLStorage();
            var markeed = storageHandler.MarkTask(Id,IsCompleted);
            return RedirectToAction("XMLView");
        }


        var status = await _graphqlService.MarkTask(Id,IsCompleted, "MSSQL");
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
// add more validation on this side , try catch, move methods to classes, not to hard code connection string,  fix  all where it needed, validation attributes, del all js and rewrite all code ...  ,single ... many task in one time work on it