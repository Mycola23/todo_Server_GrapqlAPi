using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using ToDoProject.Models;
using ToDoProject.Storages;
namespace ToDoProject.Graphql
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(IHttpContextAccessor httpContextAccessor) {
            Field<ListGraphType<TaskType>>("tasks")
            .Resolve(context =>
            {
                var storageType = httpContextAccessor.HttpContext?.Request.Headers["Storage-Type"].ToString();
                //var storageStr = context.GetArgument<string>("storage");
                if (!Enum.TryParse<DataStorages>(storageType, true, out var storage))
                    throw new ExecutionError("Invalid storage")
                    {
                        Code = "INVALID_STORAGE"
                    };
                var factory = new RequestHandler();
                var service = factory.GetStorage(storage);
                return service.GetTasks();
            });
            Field<ListGraphType<CategoryType>>("categories")
            .Resolve(context =>
            {
                var storageType = httpContextAccessor.HttpContext?.Request.Headers["Storage-Type"].ToString();
                //var storageStr = context.GetArgument<string>("storage");
                if (!Enum.TryParse<DataStorages>(storageType, true, out var storage))
                    throw new ExecutionError("Invalid storage")
                    {
                        Code = "INVALID_STORAGE"
                    };
                var factory = new RequestHandler();
                var service = factory.GetStorage(storage);
                return service.GetCategories();
            });
        }
    }
}
