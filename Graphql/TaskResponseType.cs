using System;
using GraphQL.Types;
using ToDoProject.Models;

namespace ToDoProject.Graphql
{
    public class TaskResponseType : ObjectGraphType<TaskResponse>
    {
        public TaskResponseType()
        {
            Field<TaskType>("task").Resolve(context => context.Source.Task);
            Field(x => x.Message);
        }

    }
}
