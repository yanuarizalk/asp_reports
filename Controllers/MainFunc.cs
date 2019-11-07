using ASP_Web_Reports.Libs;
using ASP_Web_Reports.Models;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ASP_Web_Reports.Controllers {
    public class MainFunc : Controller {
        //session key
        
        public static IHostingEnvironment hosting { get; set; }
        private const string SKEY_UID = "uid";
        private const string SKEY_TRUST = "trust";
        public MgmContext db;
        public Logger logger = new Logger();
        //Microsoft.AspNetCore.DataProtection.
        public enum ACCESS : int {
            SALES = 1,
            PAYROLL = 2,
            INVENTORY = 3,
            MASTER = 4,
            USER_ADD = 5,
            USER_EDIT = 6,
            USER_DELETE = 7,
            USER_VIEW = 8,
            MASTER_USER = 9,
            USER_EXPORT = 10,
            SALES_RAWS = 11,
            SALES_ANNUAL_SUMMARY = 12,
            SALES_USD_QTY = 13,
            SALES_DENIM = 14,
            SALES_PROD_GROUP = 15,
            SALES_CURRENCY = 16,
            SALES_DESTINATION = 17,
            SALES_PRODUCTION = 18,
            SALES_CUSTOMER12 = 19,
            SALES_CUSTOMER24 = 20,
            SALES_DENIM_YARD = 21,
            SALES_DASHBOARD = 22,
            SALES_DETAIL = 23
        };

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            db.Dispose(); db = null; logger = null;
            //GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public MainFunc() {
            db = new MgmContext();
            // if (hosting.IsDevelopment())
            //db = new LocalContext();
        }

        /*public override void OnActionExecuting(ActionExecutingContext context) {
            HttpContext.Response.RegisterForDispose(db);
            HttpContext.Response.OnCompleted(() => {
                db.Dispose(); db = null;
                logger = null;
                return null;
                //return System.Threading.Tasks.Task.CompletedTask;
            });
        }*/

        public void SetAuth(int iID = 0) {
            if (iID == 0) {
                HttpContext.Session.Remove(SKEY_UID);
            } else {
                HttpContext.Session.SetInt32(SKEY_UID, iID);
            }
        }

        //first return array is UID, & last is MenuTrusty
        public string[] GetAuth() {
            string[] oAuth = new string[] { null, null };
            oAuth[0] = HttpContext.Session.GetString(SKEY_UID);
            oAuth[1] = HttpContext.Session.GetString(SKEY_TRUST);
            return oAuth;
        }
        public int GetID() {
            return (int)HttpContext.Session.GetInt32(SKEY_UID).GetValueOrDefault();
        }
        public bool IsAuthenticated() {
            if (HttpContext.Session.GetInt32(SKEY_UID) is null) {
                return false;
            } else {
                return true;
            }
        }
        public string GetBaseUrl() {
            if (Request.PathBase == "")
                return string.Format("{0}://{1}", Request.Scheme, Request.Host.ToUriComponent());
            else
                return string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host.ToUriComponent(), Request.PathBase);
        }

        public void AuthCheckHandler() {
            if (!IsAuthenticated()) {
                if (Request.Path.ToString().ToLower() != "/user/login") {
                    Response.Redirect(GetBaseUrl() + "/User/Login");
                }
            }
        }
        public string getRoute() {
            return HttpContext.Request.Path.Value.ToLower().Trim('/');
        }

        public string[] GetAccess() {
            if (!IsAuthenticated()) return new[] { "" };
            var ctxUser = db.users.Find(GetID());
            if (ctxUser.ID_Group == 0) {
                return ctxUser.Access.Split(',');
            } else {
                var ctxGroup = db.groups.Find(ctxUser.ID_Group);
                if (ctxGroup == null) {
                    return ctxUser.Access.Split(',');
                } else {
                    return ctxGroup.Access_Default.Split(',');
                }
            }
        }
        public bool CheckAccess(int idMenu = 0) {
            string sAccess = "0";
            if (!IsAuthenticated()) return false;
            var ctxUser = db.users.Find(GetID());
            if (ctxUser.ID_Group == 0) {
                sAccess = ctxUser.Access;
            } else {
                var ctxGroup = db.groups.Find(ctxUser.ID_Group);
                if (ctxGroup == null) {
                    //return false;
                    sAccess = ctxUser.Access;
                } else {
                    sAccess = ctxGroup.Access_Default;
                }
            }
            if (sAccess == "*") return true;
            bool cb = false;
            Menu ctxMenu;
            if (idMenu == 0) {
                ctxMenu = db.menu.SingleOrDefault(res => ( res.Route.ToLower() == getRoute().Substring(0, res.Route.Length) && !String.IsNullOrEmpty(res.Route) ));
            } else {
                ctxMenu = db.menu.SingleOrDefault(res => res.ID == idMenu);
            }
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ctxMenu));
            if (ctxMenu == null) return false;
            foreach (string sFor in sAccess.Split(",")) {
                if (sFor == ctxMenu.ID.ToString()) {
                    cb = true;
                    break;
                }
            }
            return cb;
        }
        public bool CheckAccess(ACCESS idMenu = 0) {
            return CheckAccess((int)idMenu);
        }
        public string showReport(string fName, object[][] Params = null) {
            WebReport webReport = new WebReport();
            webReport.Report.FinishReport += new EventHandler((obj, arg) => {
                webReport.Report.GraphicCache.Dispose();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            });
            webReport.Report.Load($@"Reports/" + fName + ".frx");
            if (Params != null)
                foreach (object[] param in Params) {
                    webReport.Report.SetParameterValue((string)param[0], param[1]);
                }
            //webReport.DesignerLocale = "en";
            //Console.WriteLine(webReport.ID);
            webReport.Debug = false;
            webReport.Report.UseFileCache = true;
            webReport.Report.NeedRefresh = true;
            
            //webReport.Report.SetParameterValue("conString", Configuration.GetConnectionString("WMBContext"));
            ViewBag.WebReport = webReport;
            //Response.RegisterForDispose(webReport.Report);
            /*HttpContext.Response.OnCompleted(() => {
                webReport.Report.Dispose();
                return null;
            });*/
            //webReport.Report.Dispose();
            //webReport = null;
            return webReport.ID;
        }

    }
}