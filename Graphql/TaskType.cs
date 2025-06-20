using GraphQL.Types;
using ToDoProject.Models;
namespace ToDoProject.Graphql
{
    public class TaskType : ObjectGraphType<TaskModel>
    {
        public TaskType() {
            Field<IntGraphType>("Id").Resolve(context => context.Source.Id);
            Field(x => x.Text);
            Field<BooleanGraphType>("isCompleted").Resolve(context => context.Source.IsCompleted);
            Field<DateTimeGraphType>("completedAt").Resolve(context => context.Source.CompletedAt);
            Field<IntGraphType>("categoryId").Resolve( context => context.Source.CategoryId);
            Field<DateTimeGraphType>("plannedTime").Resolve( context => context.Source.PlannedTime);
        }
    }
}
