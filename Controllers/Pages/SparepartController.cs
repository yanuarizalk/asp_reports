using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_Web_Reports.Controllers {
    public class SparepartController : BaseController {
        // GET: User
        public ActionResult Index() {
            return View();
        }
    }
}