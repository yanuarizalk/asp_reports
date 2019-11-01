using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
        public enum REQ_TYPE : int {
            QUERY = 1, FORM_DATA = 2
        }


        public Dictionary<string, string> RequestParser(object[][] oData, REQ_TYPE reqType) {
            Dictionary<string, string> cb = new Dictionary<string, string>();
            // data name, isRequired or not
            foreach (object[] data in oData) {
                if (reqType == REQ_TYPE.QUERY) {
                    if (Request.Query.TryGetValue(data[0].ToString(), out StringValues val)) {
                        cb.Add(data[0].ToString(), val.ToString());
                    } else if ((bool)data[1] == false) cb.Add(data[0].ToString(), null);
                    else return null;
                } else if (reqType == REQ_TYPE.FORM_DATA) {
                    if (Request.Form.TryGetValue(data[0].ToString(), out StringValues val)) {
                        cb.Add(data[0].ToString(), val.ToString());
                    } else if ((bool)data[1] == false) cb.Add(data[0].ToString(), null);
                    else return null;
                }
            }
            return cb;
        }

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