using Azure;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using ToDoProject.Models;

namespace ToDoProject.Storages
{
    public class MSSQLStorage : IStorage
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TODO;Integrated Security=True;Encrypt=True";
        
        public TaskResponse CreateTask(TaskModel task)
        {
            var response = new TaskResponse();
            int genaretedId = 0;
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
                    

                    using (SqlCommand cmdSecond = new SqlCommand("SELECT TOP 1 * FROM Tasks ORDER BY Id DESC", conn))
                    using (SqlDataReader readerSecond = cmdSecond.ExecuteReader())
                    {
                        if (readerSecond.Read())
                        {
                            genaretedId = (int)readerSecond["Id"];
                        }
                    }
                    cmd.ExecuteNonQuery();
                }

                
                response.Task = task;
                response.Task.Id = genaretedId;
                response.Message = "task created successfully";


            }
            catch (Exception ex)
            {
                response.Task = null;
                response.Message = $"Error with creating task: {ex.Message}";
                Debug.WriteLine($"Error: {ex.Message}");
                
            }
           return response;
        }

        public TaskResponse DeleteTask(int id)
        {
            var response = new TaskResponse();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string selectQuery = "SELECT * FROM Tasks WHERE Id = @Id";
                    SqlCommand selectCmd = new SqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var task = new TaskModel
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

                            };

                            response.Task = task;
                        }
                        else
                        {
                            response.Message = "not found task";
                            return response;
                        }
                    }
                    string deleteQuery = "DELETE FROM Tasks WHERE Id = @Id";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@Id", id);
                    deleteCmd.ExecuteNonQuery();

                    response.Message = "task deleted";
                   
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Error with deleting task: {ex.Message}";
               
            }

            return response;
        }

        public TaskResponse MarkTask(int id, bool isCompleted)
        {
            var response = new TaskResponse();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string selectQuery = "SELECT * FROM Tasks WHERE Id = @Id";
                    SqlCommand selectCmd = new SqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@Id", id);

                    TaskModel? task = null;

                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            response.Message = "task not found";
                            return response;
                        }

                        task = new TaskModel
                        {
                            Id = (int)reader["Id"],
                            Text = reader["Text"].ToString(),
                            IsCompleted = isCompleted, 
                            CompletedAt = isCompleted ? DateTime.Now : null,
                            CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("CategoryId")),
                            PlannedTime = reader.IsDBNull(reader.GetOrdinal("PlannedTime"))
                                ? (DateTime?)null
                                : reader.GetDateTime(reader.GetOrdinal("PlannedTime"))
                        };
                    }

                    
                    string markQuery = @"
                            UPDATE Tasks
                            SET 
                                IsCompleted = @IsCompleted,
                                CompletedAt = CASE 
                                            WHEN @IsCompleted = 1 
                                                  THEN GETDATE() 
                                                ELSE NULL 
                                              END
                            WHERE Id = @Id";

                    SqlCommand updateCmd = new SqlCommand(markQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Id", id);
                    updateCmd.Parameters.AddWithValue("@IsCompleted", isCompleted);
                    updateCmd.ExecuteNonQuery();

                    response.Task = task;
                    response.Message = "task marked";
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Error with marking task: {ex.Message}";
                Debug.WriteLine($"Error: {ex.Message}");
            }

            return response;
        }

        public List<TaskModel> GetTasks()
        {
            var tasks = new List<TaskModel>();
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
            return tasks;
        }

        public List<Category> GetCategories()
        {
            var categories = new List<Category>();
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
            return categories;
        }

        // think about how return error with getTasks & getCategories
    }
}
