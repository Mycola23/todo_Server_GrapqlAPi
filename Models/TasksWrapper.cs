using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ToDoProject.Models
{
    public class TasksWrapper
    {
        [ValidateNever]
        public TaskViewModel TaskVm { get; set; }
       
        public TaskModel Task { get; set; } 
    }
}
