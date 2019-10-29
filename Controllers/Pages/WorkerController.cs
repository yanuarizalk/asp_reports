using Microsoft.AspNetCore.Mvc;

namespace ASP_Web_Reports.Controllers {
    public class WorkerController : BaseController {
        public ActionResult Index() {
            ViewData["Title"] = "Worker List";
            showReport("WorkerList");
            return View("Reports");
        }
    }
}