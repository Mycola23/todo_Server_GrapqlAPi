using ToDoProject.Models;

namespace ToDoProject.Storages
{
    public interface IStorage
    {
        public TaskResponse CreateTask(TaskModel task);
        public TaskResponse DeleteTask(int id);
        public TaskResponse MarkTask(int id,bool isCompleted);

        public List<TaskModel> GetTasks();
        public List<Category> GetCategories();
    }
}
