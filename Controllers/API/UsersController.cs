using ASP_Web_Reports.Libs;
using ASP_Web_Reports.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_Web_Reports.Controllers.API {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiController {
        private const string UID = "uid";
        private const string PWD = "pwd";

        string ErrorUserNotFound() {
            return Send(new { status = "error", desc = "Can't find User using this name" });
        }
        string ErrorWrongPwd() {
            return Send(new { status = "error", desc = "Authentication rejected. Password didn't match" });
        }

        string ErrorUserExist() {
            return Send(new { status = "error", desc = "Username already exist. Please, choose another Username that have not been used before." });
        }

        [HttpPost("{subRoute}", Order = 1)]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        //[Produces("application/json")]
        public async Task<string> Post(/*[FromForm] dynamic value, */[FromRoute] string subRoute, List<IFormFile> files = null) {
            string[] checker; Users tUser;// Users[] lUser;
            switch (subRoute.ToLower()) {
                case "auth": {
                    checker = new string[] { UID, PWD };
                    if (!CheckPayLoad(checker)) return ErrorInvalid();
                    if (!ModelState.IsValid) return ErrorBadReq();
                    tUser = await db.users.SingleOrDefaultAsync(user => user.Username == Request.Form[UID][0]);
                    if (tUser == null) return ErrorUserNotFound();
                    Simple3Des des = new Simple3Des(Request.Form[UID][0]);
                    if (tUser.Password == des.EncryptData(Request.Form[PWD][0])) {
                        if (tUser.Status == false)
                            return Send(new { status = "error", desc = 
                                "Your account have been suspended. Due to some reason you are not eligible to use this account.<br>" +
                                "For more information, please ask your administrator."
                            });
                        SetAuth(tUser.ID);
                        Console.WriteLine(tUser.Username + "["+ tUser.ID +"] Logged in");
                        logger.Write(LOG_MODE.ACTION, "ID " + GetID() + " login");
                        return Send(new {
                            status = "success"
                        });
                    } else return ErrorWrongPwd();//Send(new { aa = des.EncryptData(Request.Form[PWD][0]), bb = tUser.UserPassword, cc = tUser.UserId });
                }
                case "datalist": {
                    checker = new string[] { DT_SKIP, DT_TAKE, DT_REQTOTAL };
                    if (!CheckAccess(ACCESS.MASTER)) return ErrorAccess();
                    if (!ModelState.IsValid) return ErrorBadReq();
                    if (!CheckPayLoad(checker)) {
                        if (!CheckPayLoad(new string[] { DT_REQTOTAL, "sort" }))
                            return ErrorInvalid();
                        return false.ToString();
                    }
                    string sortBy = "ID", search = ""; bool bDesc = false;
                    //var q = db.users.FromSqlRaw("select * from Users");
                    //var g = db.groups.AsEnumerable();
                    var query = db.users.Join(db.groups.AsEnumerable(), user => user.ID_Group, group => group.ID,
                        (user, group) => new {
                            ID = user.ID, Username = user.Username, Role = group.Name,
                            Email = user.Email, Status = user.Status, IdGroup = user.ID_Group,
                            CustomAccess = user.Access
                        }
                    ).AsEnumerable();
                    /*var query = db.users.Select(user => new {
                        user.ID, user.Username, user.Role,
                        user.Email, user.Status, IdGroup = user.ID_Group,
                        CustomAccess = user.Access
                    }).AsEnumerable();*/
                    //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(query.ToList()));
                    if (CheckPayLoad(new string[] { "filter" })) {
                        var filter = JsonConvert.DeserializeObject<JArray>(Request.Form["filter"]);
                        if ((string)filter[0][0] == "Username" && (string)filter[0][1] == "contains") {
                            search = filter[0][2].ToString().ToLower();
                            query = query.Where(user =>
                                //EF.Functions.Like(user.Role, "%" + search + "%") ||
                                user.Role.ToLower().Contains(search) ||
                                user.Email.ToLower().Contains(search) ||
                                user.Username.ToLower().Contains(search)
                            );
                        } else if ((string)filter[0][0] == "ID" && (string)filter[0][1] == "=") {
                            search = filter[0][2].ToString().ToLower();
                            query = query.Where(user => user.ID.ToString().Contains(search) ||
                                //EF.Functions.Like(user.Role, "%" + search + "%") ||
                                user.Role.ToLower().Contains(search) ||
                                user.Email.ToLower().Contains(search) ||
                                user.Username.ToLower().Contains(search)
                            );
                        }
                    }
                    if (CheckPayLoad(new string[] { "sort" })) {
                        var sort = JsonConvert.DeserializeObject<JArray>(Request.Form["sort"]);
                        sortBy = (string)sort[0]["selector"]; bDesc = (bool)sort[0]["desc"];
                        if (bDesc == false) {
                            switch (sortBy.ToLower()) {
                                case "id": query = query.OrderBy(ob => ob.ID); break;
                                case "username": query = query.OrderBy(ob => ob.Username); break;
                                case "role": query = query.OrderBy(ob => ob.Role); break;
                                case "email": query = query.OrderBy(ob => ob.Email); break;
                                case "status": query = query.OrderBy(ob => ob.Status); break;
                            }
                            //query.OrderBy(ob => EF.Property<object>(ob, sortBy));
                        } else {
                            switch (sortBy.ToLower()) {
                                case "id": query = query.OrderByDescending(ob => ob.ID); break;
                                case "username": query = query.OrderByDescending(ob => ob.Username); break;
                                case "role": query = query.OrderByDescending(ob => ob.Role); break;
                                case "email": query = query.OrderByDescending(ob => ob.Email); break;
                                case "status": query = query.OrderByDescending(ob => ob.Status); break;
                            }
                        }
                    }
                    query = query.Skip(Convert.ToInt32(Request.Form[DT_SKIP][0]));
                    query = query.Take(Convert.ToInt32(Request.Form[DT_TAKE][0]));
                    var data = query.ToList();

                    if (data.Count() < 1) return "[]";
                    int totalCount = await db.users.CountAsync();
                    var result = new { data, totalCount = totalCount };
                    return Send(result);
                }
                case "new": {
                    try {
                        checker = new string[] {
                            "uid", "pwdNew", "email"/*, "status", "role"*/, "customAccess"
                        };
                        if (!CheckPayLoad(checker)) return ErrorInvalid();
                        if (!CheckAccess(ACCESS.USER_ADD)) return ErrorAccess();
                        if (!ModelState.IsValid) return ErrorBadReq();
                        var checkUser = await db.users.SingleOrDefaultAsync(user => user.Username == (string)Request.Form["uid"]);
                        if (checkUser != null) return ErrorUserExist();
                        Simple3Des des = new Simple3Des(Request.Form["uid"]);
                        //Console.WriteLine(Send(Request.Form));
                        var result = await db.users.AddAsync(new Users {
                            Username = (string)Request.Form["uid"],
                            ID_Group = Request.Form["role"] == "null" || Request.Form["role"] == "undefined" ? 0: Convert.ToInt32(Request.Form["role"]),
                            Status = Convert.ToBoolean(Request.Form["status"]),
                            Email = (string)Request.Form["email"], Access = (string)Request.Form["customAccess"],
                            Password = des.EncryptData((string)Request.Form["pwdNew"]), Logged_In = false
                        });
                        if (result.State == EntityState.Added) {
                            result.Context.SaveChanges();
                        } else if (result.State == EntityState.Unchanged) {
                            return Send(new {
                                status = "error", desc = "Can't add new User, due to the state is remainly unchanged"
                            });
                        }
                        if (files.Count == 1) {
                            var fStream = new FileStream(hosting.WebRootPath + "\\images\\profiles\\" + result.Entity.ID + ".jpg", FileMode.Create);
                            await files[0].CopyToAsync(fStream);
                            fStream.Dispose(); fStream = null;
                        } else {
                            if (System.IO.File.Exists(hosting.WebRootPath +
                                "\\images\\profiles\\" + result.Entity.ID + ".jpg"))
                                System.IO.File.Delete(hosting.WebRootPath +
                                    "\\images\\profiles\\" + result.Entity.ID + ".jpg");
                        }
                        logger.Write(LOG_MODE.ACTION, "ID " + GetID() + " added new user(" + result.Entity.ID +")");
                    } catch (Exception exc) {
                        return Send(new {
                            status = "error", desc = exc.Message, number = exc.HResult,
                            source = exc.Source, sTrace = exc.StackTrace
                        });
                    }
                    return Send(new {
                        status = "success"
                    });
                }
                case "update": {
                    try {
                        checker = new string[] {
                            "id", "uid", "pwdNew", "pwdOld", "email", "status", "role", "customAccess",
                            "clearPhoto"
                        };
                        if (!CheckPayLoad(checker)) return ErrorInvalid();
                        if (!CheckAccess(ACCESS.USER_EDIT)) return ErrorAccess();
                        if (!ModelState.IsValid) return ErrorBadReq();

                        var cekUser = await db.users.SingleOrDefaultAsync(user => user.ID == Convert.ToInt32(Request.Form["id"]));
                        if (cekUser == null) return ErrorUserNotFound();
                        Simple3Des des = new Simple3Des(cekUser.Username);
                        if (cekUser.Password != des.EncryptData(Request.Form["pwdOld"])) return Send(new { status = "error", desc = "Old password didn't match" });
                        if (cekUser.Username != Request.Form["uid"]) {
                            var cekUserExist = await db.users.SingleOrDefaultAsync(user =>
                                user.Username == Request.Form["uid"]);
                            if (cekUserExist != null) return ErrorUserExist();
                        }
                        des = new Simple3Des(Request.Form["uid"]);
                        cekUser.Username = Request.Form["uid"];
                        cekUser.Password = des.EncryptData(Request.Form["pwdNew"]);
                        cekUser.Email = Request.Form["email"];
                        cekUser.Status = Convert.ToBoolean(Request.Form["status"]);
                        cekUser.ID_Group = Request.Form["role"] == "null" || Request.Form["role"] == "undefined" ? 0 : Convert.ToInt32(Request.Form["role"]);
                        cekUser.Access = (string)Request.Form["customAccess"] == "null" ? "" : (string)Request.Form["customAccess"];
                        var result = db.users.Update(cekUser);
                        if (result.State == EntityState.Modified) {
                            await result.Context.SaveChangesAsync();
                        } else if (result.State == EntityState.Unchanged) {
                            return Send(new {
                                status = "error", desc = "Can't edit existing User, due to state is remainly unchanged"
                            });
                        }
                        if (Request.Form["clearPhoto"] == "true") {
                            if (System.IO.File.Exists(hosting.WebRootPath +
                                "\\images\\profiles\\" + cekUser.ID + ".jpg"))
                                System.IO.File.Delete(hosting.WebRootPath +
                                    "\\images\\profiles\\" + cekUser.ID + ".jpg");
                        } else {
                            if (files.Count == 1) {
                                var fStream = new FileStream(hosting.WebRootPath +
                                    "\\images\\profiles\\" + cekUser.ID + ".jpg", FileMode.Create);
                                await files[0].CopyToAsync(fStream);
                                fStream.Dispose(); fStream = null;
                            }
                        }
                        logger.Write(LOG_MODE.ACTION, "ID " + GetID() + " commit update to user(" + result.Entity.ID + ")");
                    } catch (Exception exc) {
                        return Send(new {
                            status = "Error", desc = exc.Message, number = exc.HResult,
                            source = exc.Source, sTrace = exc.StackTrace
                        });
                    }
                    return Send(new {
                        status = "success"
                    });
                }
                case "delete": {
                    try {
                        checker = new string[] {
                            "id"
                        };
                        if (!CheckPayLoad(checker)) return ErrorInvalid();
                        if (!CheckAccess(ACCESS.USER_DELETE)) return ErrorAccess();
                        if (!ModelState.IsValid) return ErrorBadReq();

                        var cekUser = await db.users.SingleOrDefaultAsync(user => user.ID == Convert.ToInt32(Request.Form["id"]));
                        if (cekUser == null) return ErrorUserNotFound();
                        if (GetID() == cekUser.ID) return Send(new {
                            status = "error", desc = "You can't delete yourself"
                        });
                        var result = db.users.Remove(cekUser);
                        if (result.State == EntityState.Deleted) {
                            result.Context.SaveChanges();
                        } else if (result.State == EntityState.Unchanged) {
                            return Send(new {
                                status = "error", desc = "Can't delete existing User, due to state is remainly unchanged"
                            });
                        }
                        logger.Write(LOG_MODE.ACTION, "ID " + GetID() + " delete user(" + result.Entity.ID + ")");
                    } catch (Exception exc) {
                        return Send(new {
                            status = "Error", desc = exc.Message, number = exc.HResult,
                            source = exc.Source, sTrace = exc.StackTrace
                        });
                    }
                    return Send(new {
                        status = "success"
                    });
                }
                case "change_pw": {
                    try {
                        checker = new string[] {
                            "new", "old"
                        };
                        if (!CheckPayLoad(checker)) return ErrorInvalid();
                        if (!IsAuthenticated()) return ErrorAccess();
                        if (!ModelState.IsValid) return ErrorBadReq();

                        var cekUser = await db.users.SingleOrDefaultAsync(user => user.ID == GetID());
                        if (cekUser == null) return ErrorUserNotFound();
                        Simple3Des des = new Simple3Des(cekUser.Username);
                        if (cekUser.Password != des.EncryptData(Request.Form["old"])) return Send(new { status = "error", desc = "Old password didn't match" });
                        cekUser.Password = des.EncryptData(Request.Form["new"]);
                        var result = db.users.Update(cekUser);
                        if (result.State == EntityState.Modified) {
                            await result.Context.SaveChangesAsync();
                        } else if (result.State == EntityState.Unchanged) {
                            return Send(new {
                                status = "error", desc = "Can't change password, due to state is remainly unchanged"
                            });
                        }
                    } catch (Exception exc) {
                        return Send(new {
                            status = "Error", desc = exc.Message, number = exc.HResult,
                            source = exc.Source, sTrace = exc.StackTrace
                        });
                    }
                    return Send(new {
                        status = "success"
                    });
                }
                /*case "checker": {
                    if (!CheckPayLoad(new string[] {
                        "uid"
                    })) return ErrorInvalid();
                    return "";
                }*/
                default:
                    return ErrorInvalid();
            }
        }
        [HttpGet]
        public async Task<string> Get() {
            return ErrorInvalid();
        }

        [HttpGet("Form/{subRoute}", Order = 2)]
        public async Task<IActionResult> Get([FromRoute] string subRoute) {
            switch (subRoute.ToLower()) {
                case "change_password": {
                    return View("../User/Change_Password");
                }
                default: {
                    return View("404");
                }
            }
        }
    }
}
