using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ToDoProject.Models
{
    public class TaskModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        [Required(ErrorMessage = "text - empty")]
        public string Text { get; set; }
        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }
        [JsonPropertyName("completedAt")]
        public DateTime? CompletedAt { get; set; }
        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }
        [JsonIgnore]
        public string? CategoryType { get; set; }
        [JsonPropertyName("plannedTime")]
        public DateTime? PlannedTime { get; set; }


    }
}
