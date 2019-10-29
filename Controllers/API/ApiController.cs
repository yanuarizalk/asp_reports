using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using System.Text.Json;

namespace ASP_Web_Reports.Controllers.API {
    [Route("api/[controller]")]
    [ApiController]
    /*[Consumes("application/json", new string[] { "application/x-www-form-urlencoded", "multipart/form-data" })]
    [Produces("application/json")]*/
    public class ApiController : MainFunc {
        public const string DT_TAKE = "take";
        public const string DT_SKIP = "skip";
        public const string DT_REQTOTAL = "requireTotalCount";


        public bool CheckPayLoad(string[] keyPL) {
            bool bReturn = true;
            foreach (var key in keyPL) {
                try {
                    if (Request.HasFormContentType == false) return false;
                    if (!Request.Form.ContainsKey(key)) {
                        //if (string.IsNullOrEmpty(Request.Form[key])) {
                        //if ((string)Request.Form[key] == null) {
                        bReturn = false; break;
                        //throw Exception;
                    }
                } catch (NullReferenceException) {
                    bReturn = false;
                    break;
                }
            }
            return bReturn;
        }
        public string Send(object json) {
            return JsonConvert.SerializeObject(json);
            //return new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json");
        }
        public string Send(object[] json) {
            return JsonConvert.SerializeObject(json);
        }
        public string ErrorInvalid() {
            return JsonConvert.SerializeObject(new {
                status = "error",
                desc = "Invalid Request Data."
            });
        }
        public string ErrorBadReq() {
            return JsonConvert.SerializeObject(new {
                status = "error",
                desc = "Internal Error on Server, please ask Administrator to handle it."
            });
        }
        public string ErrorAccess() {
            return JsonConvert.SerializeObject(new {
                status = "error",
                desc = "Restricted access. You are not allowed to use this action."
            });
        }
    }
}