using System;
using Microsoft.AspNetCore.Mvc;

namespace ASP_Web_Reports.Controllers
{
    public class DashboardController : BaseController {

        //[ResponseCache(NoStore = true)]
        public IActionResult Sales()
        {
            ViewData["min"] = DateTime.Now.Year - 3;
            ViewData["max"] = DateTime.Now.Year;
            return View();
        }
        public IActionResult Payrolls()
        {
            return View();
        }
        public IActionResult Inventories()
        {
            return View();
        }
    }
}
