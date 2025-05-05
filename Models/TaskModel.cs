namespace ToDoProject.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? PlannedTime { get; set; }


    }
}
