using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToDoProject.Models;
using ToDoProject.Storages;

namespace ToDoProject
{
    public class RequestHandler
    {
       
        private string xmlPath = "D:\\dicsiplens\\first-course\\SANA\\TODOWEBy\\WebApplication1\\Data.xml";
        // private string SchemaPath = "D:\\dicsiplens\\first-course\\SANA\\TODOWEBy\\WebApplication1\\Schema.xml";
        private string categoriesPath = "D:\\dicsiplens\\first-course\\SANA\\TODOWEBy\\WebApplication1\\Categories.xml";
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TODO;Integrated Security=True;Encrypt=True";

        public IStorage GetStorage (DataStorages storage)
        {
            switch (storage)
            {
                case DataStorages.MSSQL:
                    return new MSSQLStorage();
                case DataStorages.XML:
                    return new XMLStorage();
                default:
                    throw new ArgumentException("Unknown storage");
            }
        }



        public TasksWrapper GenerateView(DataStorages storage)
        {

            var tasks = new List<TaskModel>();
            var categories = new List<Category>();
            var tasksWrapper = new TasksWrapper();
            if (storage ==  DataStorages.MSSQL)
            {
                try
                {
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
                                    Type = readerSecond["Type"].ToString()
                                });
                            }
                        }
                    }


                    // зробити присвоєння типу тасків для генерування сторінки в головному контролері

                    var tasksWithType = tasks
                        .Select(t => new TaskModel
                        {
                            Id = t.Id,
                            Text = t.Text,
                            IsCompleted = t.IsCompleted,
                            CompletedAt = t.CompletedAt,
                            PlannedTime = t.PlannedTime,
                            CategoryId = t.CategoryId,
                            CategoryType = categories
                                             .FirstOrDefault(c => c.Id == t.CategoryId)
                                             ?.Type
                                             ?? ""
                        })
                        .ToList();

                    var viewModel = new TaskViewModel
                    {
                        Tasks = tasksWithType,
                        Categories = categories
                    };
                    tasksWrapper = new TasksWrapper
                    {
                        TaskVm = viewModel,
                        Task = new TaskModel()
                    };



                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }


            }
            else if(storage == DataStorages.XML)
            {
                try
                {
                   
                   
                    if (File.Exists(xmlPath) && File.Exists(categoriesPath))
                    {
                        XDocument data = XDocument.Load(xmlPath);
                        XDocument categoriesDoc = XDocument.Load(categoriesPath);
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

                        foreach (var el in categoriesDoc.Root.Elements("category"))
                        {

                            int id;
                            int.TryParse(el.Element("id")?.Value,out id);
                            categories.Add(new Category
                            {
                                Id = id,
                                Type = el.Element("type")?.Value
                            });
                        }

                        var viewModel = new TaskViewModel
                        {
                            Tasks = tasks,
                            Categories = categories
                        };
                        tasksWrapper = new TasksWrapper
                        {
                            TaskVm = viewModel,
                            Task = new TaskModel()
                        };
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
            return tasksWrapper;
        }

        public List<TaskModel> GetTasks(DataStorages storage)
        {
            var tasks = new List<TaskModel>();
            if (storage == DataStorages.MSSQL)
            {
                try
                {
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
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }

            return tasks;
        }

        public List<Category> GetCategories(DataStorages storage)
        {
            var categories = new List<Category>();
            if (storage == DataStorages.MSSQL)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Categories", conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new Category
                                {
                                    Id = (int)reader["Id"],
                                    Type = reader["Type"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }

            return categories;
        }

        public void CreateTaskGraphql(TaskModel task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Tasks (Text, CategoryId, PlannedTime) VALUES (@Text, @CategoryId, @PlannedTime)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Text", task.Text);
                    cmd.Parameters.AddWithValue("@CategoryId", task.CategoryId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PlannedTime", task.PlannedTime ?? (object)DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            
        }

        public void DeleteTaskGraphql(int Id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Tasks WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", Id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public void MarkTaskGraphql(int Id, bool IsCompleted)
        {
            try
            {


                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                            UPDATE Tasks
                            SET 
                                IsCompleted = @IsCompleted,
                                CompletedAt = CASE 
                                                WHEN @IsCompleted = 1 
                                                  THEN GETDATE() 
                                                ELSE NULL 
                                              END
                            WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@IsCompleted", IsCompleted);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public void CreateTask( TasksWrapper container, DataStorages storage)
        {


            if (storage == DataStorages.MSSQL)
            {
                try
                {

                    //Debug.WriteLine($"{container.Task.PlannedTime}, {container.Task.CategoryId}");

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
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }

            }
            else if (storage == DataStorages.XML)
            {
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
                            new XElement("text", container.Task.Text),
                            string.IsNullOrWhiteSpace(container.Task.CategoryType)
                                ? null
                                : new XElement("category", container.Task.CategoryType),
                            container.Task.PlannedTime.HasValue
                                ? new XElement("plannedTime", container.Task.PlannedTime.Value.ToString("yyyy-MM-dd HH:mm"))
                                : null
                             )
                        );
                        data.Save(xmlPath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }

               
        }

        public void DeleteTask(int Id, DataStorages storage)
        {

            if (storage == DataStorages.MSSQL)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Tasks WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", Id);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
            else if (storage == DataStorages.XML)
            {
                try
                {
                    if (File.Exists(xmlPath))
                    {
                        XDocument data = XDocument.Load(xmlPath);
                        var taskToDelete = data.Descendants("task")
                            .FirstOrDefault(t => (int)t.Attribute("id") == Id);
                        if (taskToDelete != null) {
                            taskToDelete.Remove();
                            data.Save(xmlPath);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }

        }

        //public void UpdateTask(int Id,string Text)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            string query = @"Update Tasks
        //                            SET Text = @Text
        //                            WHERE Id = @Id";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@Id", Id);
        //            cmd.Parameters.AddWithValue("@Text", Text);
        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error: {ex.Message}");
        //    }
        //}

        public void MarkTask(int Id, bool IsCompleted, DataStorages storage)
        {
            if (storage == DataStorages.MSSQL)
            {
                try
                {

                    
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = @"
                            UPDATE Tasks
                            SET 
                                IsCompleted = @IsCompleted,
                                CompletedAt = CASE 
                                                WHEN @IsCompleted = 1 
                                                  THEN GETDATE() 
                                                ELSE NULL 
                                              END
                            WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", Id);
                        cmd.Parameters.AddWithValue("@IsCompleted", IsCompleted);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
            else if (storage == DataStorages.XML)
            {
                try
                {
                    if (File.Exists(xmlPath))
                    {
                        XDocument data = XDocument.Load(xmlPath);
                        var task = data.Descendants("task")
                            .FirstOrDefault(t => (int)t.Attribute("id") == Id);

                        if (task != null)
                        {
                            var completedTimeElement = task.Element("completedTime");
                            var isCompletedElement = task.Element("isCompleted");

                            if (IsCompleted)
                            { 
                                if (completedTimeElement != null)
                                    completedTimeElement.Value = DateTime.Now.ToString("o");
                                else
                                    task.Add(new XElement("completedTime", DateTime.Now));
                            }
                            else
                            {
                                completedTimeElement?.Remove();
                            }

                            
                            if (isCompletedElement != null)
                                isCompletedElement.Value = IsCompleted.ToString();
                            else
                                task.Add(new XElement("isCompleted", IsCompleted.ToString()));

                            data.Save(xmlPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }


        }
    }
}
