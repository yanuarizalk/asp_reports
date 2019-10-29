using System;
using Microsoft.AspNetCore.Http;


namespace ASP_Web_Reports.Models.MySession
{
    public class SessionModel
    {
        private const string SKEY_UID = "uid";
        private const string SKEY_TRUST = "trust";

        /*public string UID = null;
        public string MenuTrust = null;*/
        /*public enum LevelAccess
        { }*/
        HttpContext http;
        public bool IsLogin()
        {
            
            if (http.Session.GetString("uid") == null)
            {
                return false;
            } else
            {
                return true;
            }
        }

        public void SetAuth(string sUid = null, string sTrust = null)
        {
            http.Session.SetString(SKEY_UID, sUid);
            http.Session.SetString(SKEY_TRUST, sTrust);
        }
        
        //first return array is UID, & last is MenuTrusty
        public string[] GetAuth()
        {
            string[] oAuth = new string[] { null, null };
            oAuth[0] = http.Session.GetString(SKEY_UID);
            oAuth[1] = http.Session.GetString(SKEY_TRUST);
            return oAuth;
        }
        
    }
}