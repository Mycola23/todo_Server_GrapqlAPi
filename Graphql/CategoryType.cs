using GraphQL.Types;
using ToDoProject.Models;

namespace ToDoProject.Graphql
{
    public class CategoryType:ObjectGraphType<Category>
    {
       public CategoryType() { 
            Field(x=>x.Id);
            Field(x => x.Type);
       }
    }
}
