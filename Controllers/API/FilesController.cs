using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ASP_Web_Reports.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ApiController
    {
        [HttpPost("{subRoute}")]
        public async Task<string> Post([FromForm] string value, string subRoute) {
            /*string[] checker;*/ object data = new { };
            List<object> datas = new List<object>();
            switch (subRoute.ToLower()) {
                case "sales_detail": {
                    if (!CheckAccess(ACCESS.SALES_DETAIL)) return ErrorAccess();
                    var oFiles = Directory.EnumerateFiles(hosting.WebRootPath + @"\files\Sales_Detail", "*.xlsx").Select(
                        sFilename => new { 
                            Filename = Path.GetFileNameWithoutExtension(sFilename),
                            Ico = "ext/excel.svg", Type = "FILE", FileType = "Excel"
                        }
                    );
                    return Send(new { status = "success", files = oFiles });
                }
                default:
                    return ErrorInvalid();
            }
        }
    }
}
