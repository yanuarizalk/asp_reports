﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ASP_Web_Reports.Libs;

namespace ASP_Web_Reports.Controllers
{
    public class BaseController : MainFunc {
        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);
            AuthCheckHandler();
            ViewData["BaseUrl"] = GetBaseUrl();
            ViewData["AccessMaster"] = CheckAccess(4);
            //ViewData["AccessSales"] = CheckAccess(ACCESS.SALES);
            ViewData["UA"] = GetAccess();
            //HttpContext.Response.RegisterForDispose(db);
            //ViewData["Master"] = CheckAccess();
        }

        public IActionResult Others() {
            ViewData["Message"] = "Not Found";
            //ViewData["Debug"] = getRoute();
            return View("404");
        }

    }
}