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
    public class MenusController : ApiController
    {

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {};
        }

        [HttpPost("{subRoute}")]
        public async Task<string> Post([FromForm] string value, string subRoute) {
            string[] checker; object data = new { };
            List<object> datas = new List<object>();
            switch (subRoute.ToLower()) {
                case "list": {
                    checker = new string[] { "filter" };
                    if (!CheckAccess(ACCESS.MASTER)) return ErrorAccess();
                    if (!ModelState.IsValid) return ErrorBadReq();
                    if (!CheckPayLoad(checker)) {
                        data = await db.menu
                            .Select(s => new { Value = s.ID, s.Name, s.Description })
                            .OrderBy(ob => ob.Value).ToArrayAsync();
                    } else {
                        var filter = JsonConvert.DeserializeObject<JArray>(Request.Form["filter"]);
                        string search = "";
                        if (filter[0].ToString() != "Value") {
                            if (filter[0][0].ToString() == "Name")
                                search = filter[0][2].ToString();
                            else {
                                foreach (var vFor in filter) {
                                    if (vFor.Type == JTokenType.Array) {
                                        if (vFor[0].ToString() == "Value" && vFor[1].ToString() == "=") {
                                            var getMenu = await db.menu
                                                .SingleAsync(sa => sa.ID == (int)vFor[2]);
                                            if (getMenu != null)
                                                datas.Add(new { Value = getMenu.ID, Name = getMenu.Name });
                                        }
                                    }
                                }
                                return Send(datas);
                            }
                        }
                        data = await db.menu
                            .Where(menu => menu.Name.Contains(search))
                            .Select(s => new { Value = s.ID, s.Name })
                            .OrderBy(ob => ob.Value).ToArrayAsync();
                    }
                    return Send(data);
                } case "treelist": {
                    var query = await db.menu.Select(menus => new {
                        id = menus.ID, pid = menus.ID_Main, name = menus.Name
                    }).OrderBy(ob => ob.id).ToArrayAsync();
                    return Send(query);
                }
                default:
                    return ErrorInvalid();
            }
        }
    }
}
