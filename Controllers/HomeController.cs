
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using ToDoProject.Models;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private string connectionString  = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TODO;Integrated Security=True;Encrypt=True";
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

   
    public IActionResult Index()
    {
        var tasks = new List<TaskModel>();
        var categories = new List<Category>();



        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Tasks", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(new TaskModel
                    {
                        Id = (int)reader["Id"],
                        Text = reader["Text"].ToString(),
                        IsCompleted = (bool)reader["IsCompleted"],
                        CompletedAt = reader.IsDBNull(reader.GetOrdinal("CompletedAt"))
                          ? (DateTime?)null
                          : reader.GetDateTime(reader.GetOrdinal("CompletedAt")),
                        CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId"))
                          ? (int?)null
                          : reader.GetInt32(reader.GetOrdinal("CategoryId")),
                          PlannedTime = reader.IsDBNull(reader.GetOrdinal("PlannedTime"))
                          ? (DateTime?)null
                          : reader.GetDateTime(reader.GetOrdinal("PlannedTime"))
                    });
                }
            }
            using (SqlCommand cmdSecond = new SqlCommand("SELECT * FROM Categories", conn))
            using (SqlDataReader readerSecond = cmdSecond.ExecuteReader())
            {
                while (readerSecond.Read())
                {
                    categories.Add(new Category
                    {
                        Id = (int)readerSecond["Id"],
                        Type = readerSecond["Text"].ToString()
                    });
                }
            }
        }
        var viewModel = new TaskViewModel
        {
            Tasks = tasks,
            Categories = categories
        };
        var tasksWrapper = new TasksWrapper
        {
            TaskVm = viewModel,
            Task =  new TaskModel()
        };

        return View(tasksWrapper);
    }
   
    [HttpPost]
    public ActionResult Create(TasksWrapper container)
    {
        try
        {
            if (!container.Task.Text.IsNullOrEmpty())
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Tasks (Text, CategoryId, PlannedTime) VALUES (@Text, @CategoryId, @PlannedTime)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Text", container.Task.Text);
                    cmd.Parameters.AddWithValue("@CategoryId", container.Task.CategoryId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PlannedTime", container.Task.PlannedTime ?? (object)DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                Debug.WriteLine($" success");
            }
            Debug.WriteLine($" problems");
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }

       
        return RedirectToAction("Index");
    }

    
    public ActionResult Delete(int Id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Tasks WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }

    public ActionResult Update(int Id) 
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "Update Tasks SET  WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
