using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using ToDoProject.Models;

namespace ToDoProject.Storages
{
    public class XMLStorage : IStorage
    {
        private string xmlPath = "D:\\dicsiplens\\first-course\\SANA\\TODOWEBy\\WebApplication1\\Data.xml";
        private string categoriesPath = "D:\\dicsiplens\\first-course\\SANA\\TODOWEBy\\WebApplication1\\Categories.xml";

        private TaskModel GetResponseTask(XElement task)
        {
            if (task == null) return null;

            bool.TryParse(task.Element("isCompleted")?.Value, out bool isCompleted);

            DateTime? completedAt = null;
            if (DateTime.TryParse(task.Element("completedTime")?.Value, out DateTime tempCompletedAt))
            {
                completedAt = tempCompletedAt;
            }

            DateTime? plannedTime = null;
            if (DateTime.TryParse(task.Element("plannedTime")?.Value, out DateTime tempPlannedTime))
            {
                plannedTime = tempPlannedTime;
            }

            int id = 0;
            int.TryParse(task.Attribute("id")?.Value, out id);// Безпечно

            var text = task.Element("text")?.Value ?? "";
            var category = task.Element("category")?.Value;

            return new TaskModel
            {
                Id = id,
                Text = text,
                IsCompleted = isCompleted,
                CompletedAt = completedAt,
                CategoryType = category,
                PlannedTime = plannedTime
            };
        }
        public  TaskResponse CreateTask(TaskModel task)
        {
            var response  = new TaskResponse();
            try
            {
                if (File.Exists(xmlPath))
                {
                    XDocument data = XDocument.Load(xmlPath);
                    int maxId = data.Root.Elements("task")
                    .Select(x => (int?)x.Attribute("id") ?? -1)
                    .DefaultIfEmpty(-1)
                    .Max();

                    int newId = maxId + 1;

                    data.Root.Add(
                    new XElement("task",
                        new XAttribute("id", newId),
                    new XElement("text", task.Text),
                        string.IsNullOrWhiteSpace(task.CategoryType)
                    ? null
                            : new XElement("category", task.CategoryType),
                    task.PlannedTime.HasValue
                            ? new XElement("plannedTime", task.PlannedTime.Value.ToString("yyyy-MM-dd HH:mm"))
                            : null
                         )
                    );
                    data.Save(xmlPath);
                    response.Task = task;
                    response.Message = "task created successfully";
                }
            }
            catch (Exception ex)
            {
                response.Task = null;
                response.Message = $"Error with creting task {ex.Message}";
                Debug.WriteLine($"Error: {ex.Message}");
            }
            return response;
        }

       public TaskResponse DeleteTask(int id)
       {
            var response = new TaskResponse();
            var task = new TaskModel();
            try
            { 
                if (File.Exists(xmlPath))
                {
                    XDocument data = XDocument.Load(xmlPath);
                    var taskToDelete = data.Descendants("task")
                        .FirstOrDefault(t => (int)t.Attribute("id") == id);
                    if (taskToDelete != null)
                    {
                        //var some = GetResponseTask(taskToDelete);
                        response.Task = GetResponseTask(taskToDelete);
                        taskToDelete.Remove();
                        data.Save(xmlPath);
                        response.Message = "task deleted";
                    }
                    else
                    {
                        response.Message = "task not found";
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                response.Task = null;
                response.Message = $"Error with deleting task: {ex.Message}";
            }
            return response;
       }

       public TaskResponse MarkTask(int id, bool IsCompleted)
        {
            var response = new TaskResponse();
            try
            {
                if (File.Exists(xmlPath))
                {
                    XDocument data = XDocument.Load(xmlPath);
                    var taskToMark = data.Descendants("task")
                        .FirstOrDefault(t => (int)t.Attribute("id") == id);

                    if (taskToMark != null)
                    {
                        var completedTimeElement = taskToMark.Element("completedTime");
                        var isCompletedElement = taskToMark.Element("isCompleted");

                        if (IsCompleted)
                        {
                            if (completedTimeElement != null)
                                completedTimeElement.Value = DateTime.Now.ToString("o");
                            else
                                taskToMark.Add(new XElement("completedTime", DateTime.Now));
                        }
                        else
                        {
                            completedTimeElement?.Remove();
                        }


                        if (isCompletedElement != null)
                            isCompletedElement.Value = IsCompleted.ToString();
                        else
                            taskToMark.Add(new XElement("isCompleted", IsCompleted.ToString()));

                        data.Save(xmlPath);
                        response.Message = "task marked";
                        response.Task = GetResponseTask(taskToMark);
                    }
                    else
                    {
                        response.Message = "task not found";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                response.Task = null;
                response.Message = $"Error with task marking {ex.Message}";
            }
            return response;
       }

        // write this methods below and add response task to above methods 
        public List<TaskModel> GetTasks()
        {
            var tasks = new List<TaskModel>();

            try
            {
                if (File.Exists(xmlPath) )
                {
                    XDocument data = XDocument.Load(xmlPath);
                    foreach (var el in data.Root.Elements("task"))
                    {
                        bool isCompleted = false;
                        bool.TryParse(el.Element("isCompleted")?.Value, out isCompleted);

                        DateTime? completedAt = null;
                        if (DateTime.TryParse(el.Element("completedTime")?.Value, out DateTime tempCompletedAt))
                        {
                            completedAt = tempCompletedAt;
                        }

                        DateTime? plannedTime = null;
                        if (DateTime.TryParse(el.Element("plannedTime")?.Value, out DateTime tempPlannedTime))
                        {
                            plannedTime = tempPlannedTime;
                        }

                        tasks.Add(new TaskModel
                        {
                            Id = (int)el.Attribute("id"),
                            Text = el.Element("text")?.Value,
                            IsCompleted = isCompleted,
                            CompletedAt = completedAt,
                            CategoryType = el.Element("category")?.Value,
                            PlannedTime = plannedTime
                        });


                    }       
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }

            return tasks;
        }

        public List<Category> GetCategories()
        {
            var categories = new List<Category>();
            try
            {
                if (File.Exists(categoriesPath))
                {
                    XDocument categoriesDoc = XDocument.Load(categoriesPath);
                    foreach (var el in categoriesDoc.Root.Elements("category"))
                    {

                        int id;
                        int.TryParse(el.Element("id")?.Value, out id);
                        categories.Add(new Category
                        {
                            Id = id,
                            Type = el.Element("type")?.Value
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            return categories;
        }
    }
}
