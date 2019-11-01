using Microsoft.AspNetCore.Http;
using System;

namespace ASP_Web_Reports.Libs.Manager {
    public class UserHelper
    {
        /*public static bool IsAuthenticated() {
            if (HttpContext.Session.GetInt32(SKEY_UID) is null) {
                return false;
            } else {
                return true;
            }
        }
        public static string[] GetAccess() {
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
        }*/
    }
}
