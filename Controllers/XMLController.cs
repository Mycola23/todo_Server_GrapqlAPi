//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using ToDoProject.Models;
//using ToDoProject.Storages;

//namespace ToDoProject.Controllers
//{
//    public class XMLController : Controller
//    {
//        public IActionResult Index()
//        {
//            var handler = new RequestHandler();
//            var tasksWrapper = handler.GenerateView(DataStorages.XML); 
//            return View(tasksWrapper);
//        }

//        [HttpPost]
//        public ActionResult Create(TasksWrapper container)
//        {
//            var handler = new RequestHandler();

//            if (!ModelState.IsValid)
//            {
//                var modelWithData = handler.GenerateView(DataStorages.XML);
//                modelWithData.Task = container.Task; 
//                return View("Index", modelWithData);
//            }
//            handler.CreateTask(container, DataStorages.XML);
//            return RedirectToAction("Index");
//        }
//        [HttpPost]
//        public ActionResult Delete(int Id)
//        {
//            var handler = new RequestHandler();
//            handler.DeleteTask(Id, DataStorages.XML);
//            return RedirectToAction("Index");
//        }
//        [HttpPost]
//        public ActionResult MarkComplete(int Id, bool IsCompleted)
//        {
//            var handler = new RequestHandler();
//            handler.MarkTask(Id, IsCompleted, DataStorages.XML);
//            return RedirectToAction("Index");
//        }
//    }
//}
