using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ASP_Web_Reports.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ApiController
    {

        // GET: api/Users
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {};
        }

        [HttpPost("{subRoute}")]
        public async Task<string> Post([FromForm] string value, string subRoute) {
            switch (subRoute.ToLower()) {
                case "chart": {
                    if (!CheckAccess(ACCESS.SALES)) return ErrorAccess();
                    string[] checker = { "range", "min", "max" };
                    if (!CheckPayLoad(checker)) return ErrorInvalid();
                    if (!ModelState.IsValid) return ErrorBadReq();
                    switch (Request.Form["range"]) {
                        case "year": {
                            var pastYear = new {
                                one = Convert.ToInt32(Request.Form["max"]),
                                two = Convert.ToInt32(Request.Form["max"]) - 1,
                                three = Convert.ToInt32(Request.Form["max"]) - 2,
                                four = Convert.ToInt32(Request.Form["max"]) - 3
                            };
                            DateTime monthParser = new DateTime();
                            var query = db.SalesGraph
                                .Where(sg =>
                                    Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) >= Convert.ToInt32(Request.Form["min"]) &&
                                    Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) <= pastYear.one
                                ).AsEnumerable()
                                .OrderBy(sg => sg.YYYYMM)
                                .Select(sg => new {
                                    year1 = pastYear.one == Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) ? sg.COLOUR + sg.DENIM + sg.GARMENT + sg.GREY + sg.HOMELINEN + sg.TOWEL + sg.YARN : 0,
                                    year2 = pastYear.two == Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) ? sg.COLOUR + sg.DENIM + sg.GARMENT + sg.GREY + sg.HOMELINEN + sg.TOWEL + sg.YARN : 0,
                                    year3 = pastYear.three == Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) ? sg.COLOUR + sg.DENIM + sg.GARMENT + sg.GREY + sg.HOMELINEN + sg.TOWEL + sg.YARN : 0,
                                    year4 = pastYear.four == Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) ? sg.COLOUR + sg.DENIM + sg.GARMENT + sg.GREY + sg.HOMELINEN + sg.TOWEL + sg.YARN : 0,
                                    month = monthParser.AddMonths(Convert.ToInt32(sg.YYYYMM.Substring(4, 2)) - 1).ToString("MMM")
                                })
                                .GroupBy(sg => new { sg.month })
                                .Select(sg => new {
                                    year1 = sg.Sum(sales => sales.year1) * 1000,
                                    year2 = sg.Sum(sales => sales.year2) * 1000,
                                    year3 = sg.Sum(sales => sales.year3) * 1000,
                                    year4 = sg.Sum(sales => sales.year4) * 1000,
                                    sg.Key.month
                                })
                                .Select(sg => new {
                                    year1 = sg.year1 == 0 ? null : (double?)sg.year1,
                                    year2 = sg.year2 == 0 ? null : (double?)sg.year2,
                                    year3 = sg.year3 == 0 ? null : (double?)sg.year3,
                                    year4 = sg.year4 == 0 ? null : (double?)sg.year4,
                                    sg.month
                                });
                            return Send(query.ToList());
                        }
                        default:
                            return ErrorInvalid();
                    }
                }
                case "summary": {
                    if (!CheckAccess(ACCESS.SALES)) return ErrorAccess();
                    if (!ModelState.IsValid) return ErrorBadReq();
                    var yearNow = DateTime.Now.Year;
                    var pastYear = new {
                        one = Convert.ToInt32(yearNow),
                        two = Convert.ToInt32(yearNow) - 1,
                        three = Convert.ToInt32(yearNow) - 2,
                        four = Convert.ToInt32(yearNow) - 3
                    };
                    var data = await db.SalesGraph
                        .Where(sg =>
                            Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) >= pastYear.four &&
                            Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) <= pastYear.one
                        )
                        .OrderBy(sg => sg.YYYYMM)
                        .Select(sg => new {
                            value = sg.COLOUR + sg.DENIM + sg.GARMENT + sg.GREY + sg.HOMELINEN + sg.TOWEL + sg.YARN,
                            year = Convert.ToInt32(sg.YYYYMM.Substring(0, 4))
                        })
                        .GroupBy(sg => new { sg.year })
                        .Select(sg => new {
                            value = sg.Sum(sale => sale.value) * 1000,
                            sg.Key.year
                        })
                        .ToListAsync();
                    var lastMonth = await db.SalesGraph
                        .Where(sg => Convert.ToInt32(sg.YYYYMM.Substring(0, 4)) == pastYear.one)
                        .MaxAsync(sg => Convert.ToInt32(sg.YYYYMM.Substring(4, 2)));
                    var result = new {
                        status = "success",
                        total = data.Sum(summ => summ.value),
                        data = data.Select(summ => new { summ.year, value = summ.year == DateTime.Now.Year ? summ.value / lastMonth : summ.value / 12 }).ToArray()
                    };
                    data = null;
                    return Send(result);
                }
                case "detail": {
                    if (!CheckAccess(ACCESS.SALES)) return ErrorAccess();
                    string[] checker = { "month", "year", "showBy" };
                    if (!CheckPayLoad(checker)) return ErrorInvalid();
                    if (!ModelState.IsValid) return ErrorBadReq();
                    switch (Request.Form["showBy"].ToString().ToLower()) {
                        case "month": {
                            string YMparser = Request.Form["month"] + " " + Request.Form["year"];
                            YMparser = Request.Form["year"] + DateTime.Parse(YMparser).ToString("MM");
                            var detail = await db.SalesGraph
                                .SingleOrDefaultAsync(sale => sale.YYYYMM == YMparser);
                            if (detail == null) return Send(new { status = "error", desc = "Not found" });
                            return Send(new { status = "success",
                                data = new[] {
                                    new { arg = "Yarn", val = detail.YARN * 1000 },
                                    new { arg = "Towel", val = detail.TOWEL * 1000 },
                                    new { arg = "Homelinen", val = detail.HOMELINEN * 1000 },
                                    new { arg = "Grey", val = detail.GREY * 1000 },
                                    new { arg = "Garment", val = detail.GARMENT * 1000 },
                                    new { arg = "Denim", val = detail.DENIM * 1000 },
                                    new { arg = "Colour", val = detail.COLOUR * 1000 }
                                }
                            });
                        }
                        case "year": {
                            var year = Request.Form["year"].ToString();
                            var detail = await db.SalesGraph
                                .Where(sale => sale.YYYYMM.Substring(0, 4) == year)
                                .GroupBy(sale => new { year = sale.YYYYMM.Substring(0, 4) })
                                .Select(sale => new {
                                    YARN = sale.Sum(field => field.YARN),
                                    TOWEL = sale.Sum(field => field.TOWEL),
                                    HOMELINEN = sale.Sum(field => field.HOMELINEN),
                                    GREY = sale.Sum(field => field.GREY),
                                    GARMENT = sale.Sum(field => field.GARMENT),
                                    DENIM = sale.Sum(field => field.DENIM),
                                    COLOUR = sale.Sum(field => field.COLOUR),
                                    year = sale.Key.year
                                })
                                .SingleOrDefaultAsync(sale => sale.year == year);
                            if (detail == null) return Send(new { status = "error", desc = "Not found" });
                            return Send(new { status = "success",
                                data = new[] {
                                    new { arg = "Yarn", val = detail.YARN },
                                    new { arg = "Towel", val = detail.TOWEL },
                                    new { arg = "Homelinen", val = detail.HOMELINEN },
                                    new { arg = "Grey", val = detail.GREY },
                                    new { arg = "Garment", val = detail.GARMENT },
                                    new { arg = "Denim", val = detail.DENIM },
                                    new { arg = "Colour", val = detail.COLOUR }
                                }
                            });
                        }
                        default: {
                            return ErrorBadReq();
                        }
                    }
                }
                default:
                    return ErrorInvalid();
            }
            //return subRoute + " - " + JsonConvert.SerializeObject(Request.Form.ToArray());//JsonConvert.SerializeObject(value);
        }

        [HttpGet("Reports/{report}")]
        /*[ProducesResponseType(typeof(HtmlString))]*/
        public async Task<ContentResult> Get([FromRoute] string report) {
            string ReportId = "";

            //if (!CheckAccess(ACCESS.SALES)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" };
            //if (!ModelState.IsValid) return new ContentResult { Content = ErrorBadAccess(), ContentType = "application/json" };
            int iMonth, iYear; string sReport;
            switch (report.ToLower()) {
                case "currencies":
                    if (!CheckAccess(ACCESS.SALES_CURRENCY)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "raws":
                    if (!CheckAccess(ACCESS.SALES_RAWS)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "usd_qty":
                    if (!CheckAccess(ACCESS.SALES_USD_QTY)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "prodgroup":
                    if (!CheckAccess(ACCESS.SALES_PROD_GROUP)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "annualraws":
                    if (!CheckAccess(ACCESS.SALES_ANNUAL_SUMMARY)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "byproduction":
                    if (!CheckAccess(ACCESS.SALES_PRODUCTION)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "bydestination":
                    if (!CheckAccess(ACCESS.SALES_DESTINATION)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "denim":
                    if (!CheckAccess(ACCESS.SALES_DENIM)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "customer_12":
                    if (!CheckAccess(ACCESS.SALES_CUSTOMER12)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "customer_24":
                    if (!CheckAccess(ACCESS.SALES_CUSTOMER24)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                case "denimyard":
                    if (!CheckAccess(ACCESS.SALES_DENIM_YARD)) return new ContentResult { Content = ErrorAccess(), ContentType = "application/json" }; break;
                default:
                    return new ContentResult {
                        Content = ErrorInvalid(), ContentType = "application/json"
                    };
            }
            sReport = report.ToLower();
            if (Request.Query.TryGetValue("month", out StringValues sMonth) && Request.Query.TryGetValue("year", out StringValues sYear)) {
                if (!int.TryParse(sMonth, out iMonth) || !int.TryParse(sYear, out iYear)) {
                    return new ContentResult { Content = ErrorInvalid(), ContentType = "application/json" };
                }
                var parseDate = new DateTime().AddYears(iYear - 1).AddMonths(iMonth);
                var maxDate = db.Params.FirstOrDefault().Period;
                if (maxDate < parseDate)
                    return new ContentResult { 
                        Content = Send(new { 
                            status = "error", 
                            desc = "There is no data for this period."
                        }), 
                        ContentType = "application/json" };
                iMonth++;
                ReportId = showReport("Sales-" + sReport, new object[][] { new object[] { "Month", iMonth }, new object[] { "Year", iYear } });
            } else
                ReportId = showReport("Sales-" + sReport);
            /*switch (report.ToLower()) {
                case "currency": {
                    if (Request.Query.TryGetValue("month", out StringValues sMonth) && Request.Query.TryGetValue("year", out StringValues sYear)) {
                        int iMonth, iYear;
                        if (!int.TryParse(sMonth, out iMonth) || !int.TryParse(sYear, out iYear)) {
                            return new ContentResult { Content = ErrorInvalid(), ContentType = "application/json" };
                        }
                        iMonth++;
                        ReportId = showReport("SalesCurrency", new object[][] { new object[] { "Month", iMonth }, new object[] { "Year", iYear } });
                    } else {
                        ReportId = showReport("SalesCurrency");
                    }
                    break;
                } case "summary-1": {
                    if (Request.Query.TryGetValue("month", out StringValues sMonth) && Request.Query.TryGetValue("year", out StringValues sYear)) {
                        int iMonth, iYear;
                        if (!int.TryParse(sMonth, out iMonth) || !int.TryParse(sYear, out iYear)) {
                            return new ContentResult { Content = ErrorInvalid(), ContentType = "application/json" };
                        }
                        iMonth++;
                        ReportId = showReport("Sales1", new object[][] { new object[] { "Month", iMonth }, new object[] { "Year", iYear } });
                    } else {
                        ReportId = showReport("Sales1");
                    }
                    break;
                } case "summary-2": {
                    if (Request.Query.TryGetValue("month", out StringValues sMonth) && Request.Query.TryGetValue("year", out StringValues sYear)) {
                        int iMonth, iYear;
                        if (!int.TryParse(sMonth, out iMonth) || !int.TryParse(sYear, out iYear)) {
                            return new ContentResult { Content = ErrorInvalid(), ContentType = "application/json" };
                        }
                        iMonth++;
                        ReportId = showReport("Sales2", new object[][] { new object[] { "Month", iMonth }, new object[] { "Year", iYear } });
                    } else {
                        ReportId = showReport("Sales2");
                    }
                    break;
                } case "summary-3": {
                    if (Request.Query.TryGetValue("month", out StringValues sMonth) && Request.Query.TryGetValue("year", out StringValues sYear)) {
                        int iMonth, iYear;
                        if (!int.TryParse(sMonth, out iMonth) || !int.TryParse(sYear, out iYear)) {
                            return new ContentResult { Content = ErrorInvalid(), ContentType = "application/json" };
                        }
                        iMonth++;
                        ReportId = showReport("Sales3", new object[][] { new object[] { "Month", iMonth }, new object[] { "Year", iYear } });
                    } else {
                        ReportId = showReport("Sales3");
                    }
                    break;
                } default: return new ContentResult {
                    Content = ErrorInvalid(), ContentType = "application/json"
                };
            }*/
            return new ContentResult {
                Content = /*await ViewBag.WebReport.Render().Result.Value*/ViewBag.WebReport.RenderSync().Value + "<script>var ReportId = \"" + ReportId +"\";</script>", ContentType = "text/html"
            };
        }
    }
}
