using Microsoft.AspNetCore.Mvc;

namespace ASP_Web_Reports.Controllers {
    public class DepartmentController : BaseController {
        public ActionResult Index() {
            
            return View();
        }
        public ActionResult Division() {
            return View();
        }
        public ActionResult Group() {
            return View();
        }
    }
}