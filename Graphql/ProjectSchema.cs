using GraphQL.Types;
namespace ToDoProject.Graphql
{
    public class ProjectSchema : Schema
    {
        public ProjectSchema(IServiceProvider provider) : base(provider) {
            Query = provider.GetRequiredService<RootQuery>();
            Mutation = provider.GetRequiredService<TaskMutation>();
        }
    }
}
