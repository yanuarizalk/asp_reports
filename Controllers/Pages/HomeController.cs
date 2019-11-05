using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP_Web_Reports.Models;
using Microsoft.AspNetCore.Http;

namespace ASP_Web_Reports.Controllers
{
    public class HomeController : BaseController
    {
        /*public HomeController(LocalContext context) {
            db = context;
        }*/
        public IActionResult Index() {
            ViewData["Access_Sales"] = CheckAccess(ACCESS.SALES_DASHBOARD);
            ViewData["Access_Payroll"] = CheckAccess(2);
            ViewData["Access_Inventory"] = CheckAccess(3);
            return View("Dashboard");
        }
    }
}
