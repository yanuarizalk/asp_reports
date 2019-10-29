using System;
using Microsoft.AspNetCore.Mvc;
using FastReport.Web;

namespace ASP_Web_Reports.Controllers {
    public class UserController : BaseController {
        public ActionResult Index(int ID = 0) {
            if (!CheckAccess(ACCESS.USER_VIEW)) return View("Restricted");
            ViewData["id"] = ID;
            return View();
        }
        public ActionResult Login() {
            if (IsAuthenticated()) {
                Response.Redirect(GetBaseUrl());
            }
            ViewData["MenuState"] = "HIDE";
            return View();
        }
        public ActionResult Logout() {
            logger.Write(Libs.LOG_MODE.ACTION, "ID " + GetID() + " manually logged out");
            SetAuth();
            //Response.Redirect(GetBaseUrl() + "User/Login");
            return RedirectToAction("Login", "User"); //View("Login"); //"<meta http-equiv=\"refresh\"  content = \"0; url=" + GetBaseUrl() + "User/Login\" > ";
        }
        public ActionResult Report() {
            ViewData["Title"] = "User List";
            showReport("User", new string[][] {
                new string[] { "ImgUserPath", hosting.WebRootPath + @"\images\profiles" }
            });
            return View("Reports");
        }
    }
}