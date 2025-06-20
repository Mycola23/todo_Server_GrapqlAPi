using GraphQL.Types;

namespace ToDoProject.Graphql
{
    public class TaskInputType: InputObjectGraphType
    {
        public TaskInputType() {
            Name = "TaskInputType";
            Field<IntGraphType>("id");
            Field<NonNullGraphType<StringGraphType>>("text");
            Field <DateTimeGraphType > ("plannedTime");
            Field<IntGraphType>("categoryId");
            Field<BooleanGraphType>("isCompleted");
            Field <DateTimeGraphType > ("completedAt");
           // Field<NonNullGraphType<StringGraphType>>("storageType");
        }
    }
}
