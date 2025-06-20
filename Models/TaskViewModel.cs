using System.Text.Json.Serialization;

namespace ToDoProject.Models
{
    public class TaskViewModel
    {
        [JsonPropertyName("tasks")]
        public List<TaskModel> Tasks { get;set; }
        [JsonPropertyName("categories")]
        public List<Category> Categories { get;set; }

    }
}
