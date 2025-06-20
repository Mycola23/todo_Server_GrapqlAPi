using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using ToDoProject.Models;
using ToDoProject.Storages;

namespace ToDoProject.Graphql
{
    public class TaskMutation: ObjectGraphType
    {
        public TaskMutation(IHttpContextAccessor httpContextAccessor) {
            Field<TaskResponseType>("createTask")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<TaskInputType>> { Name = "task" }
                //new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storage" }
            ))
           .Resolve(context =>
           {
               var input = context.GetArgument<TaskModel>("task");
               //var storageStr = context.GetArgument<string>("storage");

               var storageType = httpContextAccessor.HttpContext?.Request.Headers["Storage-Type"].ToString();

               if (!Enum.TryParse<DataStorages>(storageType, true, out var storage))
                   throw new ExecutionError("Invalid storage type")
                   {
                       Code = "INVALID_STORAGE"
                   };
              

               var factory = new RequestHandler();
               var service = factory.GetStorage(storage);
               var response = service.CreateTask(input);
               return response;
           });

            Field<TaskResponseType>("delTask")
            .Arguments(new QueryArguments( 
                new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }))
               // new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storage" }))
            .Resolve(context =>
            {
                var id = context.GetArgument<int>("id");
                //var storageStr = context.GetArgument<string>("storage");
                var storageType = httpContextAccessor.HttpContext?.Request.Headers["Storage-Type"].ToString();

                if (!Enum.TryParse<DataStorages>(storageType, true, out var storage))
                    throw new ExecutionError("Invalid storage type")
                    {
                        Code = "INVALID_STORAGE"
                    };
                var factory = new RequestHandler();
                var service = factory.GetStorage(storage);
                var response = service.DeleteTask(id);
                return response;
               
            });
            Field<TaskResponseType>("markTask")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>> { Name = "isCompleted" }
                )).Resolve(context =>
                {
                    var id = context.GetArgument<int>("id");
                    var status = context.GetArgument<bool>("isCompleted");

                    var storageType = httpContextAccessor.HttpContext?.Request.Headers["Storage-Type"].ToString();
                    if (!Enum.TryParse<DataStorages>(storageType, true, out var storage))
                        throw new ExecutionError("Invalid storage type")
                        {
                            Code = "INVALID_STORAGE"
                        };

                    var factory = new RequestHandler();
                    var service = factory.GetStorage(storage);
                    var response = service.MarkTask(id,status);
                    return response;
                });
        }
    }
}
