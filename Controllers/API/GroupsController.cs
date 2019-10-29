using System;
using System.Collections.Generic;
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
    public class GroupsController : ApiController {
        //object[] oFilter { get; set; }
        // GET: api/Users
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {};
        }

        [HttpPost("{subRoute}")]
        public async Task<string> Post([FromForm] string value, string subRoute) {
            string[] checker;  object data = new { };
            List<object> datas = new List<object>();
            switch (subRoute.ToLower()) {
                case "list":
                    checker = new string[] { "filter" };
                    if (!CheckAccess(ACCESS.MASTER_USER)) return ErrorAccess();
                    if (!CheckPayLoad(checker)) {
                        data = await db.groups
                            .Select(group => new { Value = group.ID, group.Name, Access = group.Access_Default })
                            .Where(group => group.Value != 0)
                            .OrderBy(ob => ob.Value).ToArrayAsync();
                    } else {
                        var filter = JsonConvert.DeserializeObject<JArray>(Request.Form["filter"]);
                        string search = ""; int val = 0;
                        if (filter[0].ToString() != "Value") {
                            if (filter[0][0].ToString() == "Name" && filter[0][1].ToString() == "contains")
                                search = filter[0][2].ToString();
                            data = await db.groups
                                .Where(group => group.Name.Contains(search) && group.ID != 0)
                                .Select(group => new { Value = group.ID, group.Name, Access = group.Access_Default })
                                .OrderBy(ob => ob.Value).ToArrayAsync();
                        } else {
                            if (filter[1].ToString() == "=")
                                val = Convert.ToInt32(filter[2].ToString());
                            data = await db.groups
                                .Select(group => new { Value = group.ID, group.Name, Access = group.Access_Default })
                                .Where(group => group.Value != 0)
                                .OrderBy(ob => ob.Value)
                                .SingleAsync(group => group.Value == val);
                        }
                    }
                    return Send(data);
                default:
                    return ErrorInvalid();
            }
        }
    }
}
