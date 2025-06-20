using System.Text.Json.Serialization;

namespace ToDoProject.Models
{
    public class Category
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
