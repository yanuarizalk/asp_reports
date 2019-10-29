using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP_Web_Reports.Models;
using Microsoft.AspNetCore.Http;

namespace ASP_Web_Reports.Controllers
{
    public class ErrorController : BaseController
    {
        public IActionResult Index() {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
