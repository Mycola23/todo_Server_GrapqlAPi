using GraphQL.Types;
using GraphQL;
using GraphQL.SystemTextJson;
using ToDoProject.Models;
using System.Text.Json;
using GraphQL.Execution;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using GraphQLParser.AST;
using static GraphQL.Validation.Rules.OverlappingFieldsCanBeMerged;

namespace ToDoProject.Graphql
{
    public class GraphqlService
    {
        private readonly HttpClient _httpClient;

        public GraphqlService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TaskViewModel> GetTasksAndCategories()
        {
            var query = @"
            {
                tasks {
                    id
                    text
                    isCompleted
                    completedAt
                    categoryId
                    plannedTime
                }
                categories {
                    id
                    type
                }
            }";

            var request = new { query };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/graphql")
            {
                Content = content
            };

            requestMessage.Headers.Add("Storage-Type", "MSSQL");

            var response = await _httpClient.SendAsync(requestMessage);
            var responseJson = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Deserialize<GraphqlResponse>(responseJson, options);
            return result?.Data ?? new TaskViewModel();
        }

        public async Task<TaskModel> CreateTask(TaskModel task, string storage)
        {
            var mutation = @"
            mutation CreateTask($task: TaskInputType!) {
                createTask(task: $task) {
                    task {
                        id
                        categoryId
                        text
                    }
                }
             }";
             
            var request = new
            {
                query = mutation,
                variables = new { task }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/graphql")
            {
                Content = content
            };

            // Додаємо заголовок Storage-Type
            requestMessage.Headers.Add("Storage-Type", storage);

            var response = await _httpClient.SendAsync(requestMessage);
            var responseJson = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Deserialize<GraphqlResponseTask>(responseJson, options);
            return result?.Data ?? new TaskModel();
        }

        public async Task<bool> DeleteTask(int id, string storage)
        {
            var mutation = @"mutation DelTask($id: Int!) {
                delTask(id: $id) {
                    task {
                        id
                        text
                    }
                    message
                }
            }
            ";

            var request = new
            {
                query = mutation,
                variables = new { id }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/graphql")
            {
                Content = content
            };

            // Додаємо заголовок Storage-Type
            requestMessage.Headers.Add("Storage-Type", storage);

            var response = await _httpClient.SendAsync(requestMessage);
            var responseJson = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Deserialize<GraphqlResponseTask>(responseJson, options);
            return result?.Data != null;
        }

        public async Task<bool> MarkTask(int id, bool isCompleted, string storage)
        {
            var mutation = @"
            mutation($id: Int!, $isCompleted: Boolean!) {
                markTask(id: $id, isCompleted: $isCompleted){
                    task{
                      id
                      text
                      isCompleted
                    }
                    message
                }
            }";

            var request = new
            {
                query = mutation,
                variables = new { id, isCompleted }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/graphql")
            {
                Content = content
            };

            // Додаємо заголовок Storage-Type
            requestMessage.Headers.Add("Storage-Type", storage);

            var response = await _httpClient.SendAsync(requestMessage);
            var responseJson = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Deserialize<GraphqlResponseTask>(responseJson, options);
            return result?.Data != null;
        }
    }
}