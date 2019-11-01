using Microsoft.AspNetCore.Mvc;

namespace ASP_Web_Reports.Controllers
{
    public class ReportsController : BaseController {

        public IActionResult Sales() {
            string subRoute = getRoute().Split("/").Length >= 3 ? getRoute().Split("/")[2] : "";
            ViewData["Title"] = "Sales Report";
            ViewData["ReportFilter"] = true;
            ViewData["ReportApi"] = "Sales";
            if (!CheckAccess(0)) {
                return View("Restricted");
            }
            switch (subRoute.ToLower()) {
                case "":
                    return Redirect(GetBaseUrl() + "/Reports/Sales/Raws");
                case "currency":
                    ViewData["ReportId"] = showReport("Sales-Currencies");
                    ViewData["ReportName"] = "currencies";
                    break;
                case "usd_qty":
                    ViewData["ReportId"] = showReport("Sales-Usd_Qty");
                    ViewData["ReportName"] = "usd_qty";
                    break;
                case "raws":
                    ViewData["ReportId"] = showReport("Sales-Raws");
                    ViewData["ReportName"] = "raws";
                    break;
                case "prod_group":
                    ViewData["ReportId"] = showReport("Sales-ProdGroup");
                    ViewData["ReportName"] = "prodgroup";
                    break;
                case "destination":
                    ViewData["ReportId"] = showReport("Sales-ByDestination");
                    ViewData["ReportName"] = "bydestination";
                    break;
                case "production":
                    ViewData["ReportId"] = showReport("Sales-ByProduction");
                    ViewData["ReportName"] = "byproduction";
                    break;
                case "denim":
                    ViewData["ReportId"] = showReport("Sales-Denim");
                    ViewData["ReportName"] = "denim";
                    break;
                case "denimyard":
                    ViewData["ReportId"] = showReport("Sales-DenimYard");
                    ViewData["ReportName"] = "denimyard";
                    break;
                case "customer12":
                    ViewData["ReportId"] = showReport("Sales-Customer_12");
                    ViewData["ReportName"] = "customer_12";
                    break;
                case "customer24":
                    ViewData["ReportId"] = showReport("Sales-Customer_24");
                    ViewData["ReportName"] = "customer_24";
                    break;
                case "annual_summary":
                    ViewData["ReportId"] = showReport("Sales-AnnualRaws");
                    ViewData["ReportName"] = "annualraws";
                    break;
                default:
                    return View("404");
            }
            //showReport(ReportName);
            return View("Reports");
        }
        public IActionResult Payrolls() {
            return View();
        }
        public IActionResult Inventories() {
            return View();
        }
    }
}
