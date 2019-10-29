using System;
using System.IO;
using System.Text;

namespace ASP_Web_Reports.Libs {
    public enum LOG_MODE {
        ACTION = 0,
        ERROR = 1,
        WARNING = 2
    }
    public class Logger
    {
        private string PATH = ASP_Web_Reports.Controllers.MainFunc.hosting.ContentRootPath;
        private FileStream fStream;
        public bool Write(LOG_MODE mode, string sMsg) {
            cekDir();
            string sDate = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            string sTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            byte[] bMsg = Encoding.UTF8.GetBytes("["+ sTime +"] " + sMsg + Environment.NewLine);
            switch (mode) {
                case LOG_MODE.ACTION: {
                    fStream = new FileStream(PATH + "\\Logs\\" + sDate + ".txt", FileMode.Append);
                    fStream.Write(bMsg, 0, bMsg.Length);
                    break;
                }
                case LOG_MODE.ERROR: {
                    fStream = new FileStream(PATH + "\\Logs\\Errors\\" + sDate + ".txt", FileMode.Append);
                    fStream.Write(bMsg, 0, bMsg.Length);
                    break;
                }
                case LOG_MODE.WARNING: {
                    fStream = new FileStream(PATH + "\\Logs\\Warnings\\" + sDate + ".txt", FileMode.Append);
                    fStream.Write(bMsg, 0, bMsg.Length);
                    break;
                }
            }
            fStream.Close();
            fStream.Dispose(); fStream = null;
            return true;
        }
        
        private void cekDir() {
            if (!Directory.Exists(PATH + "\\Logs"))
                Directory.CreateDirectory(PATH + "\\Logs");
            if (!Directory.Exists(PATH + "\\Logs\\Errors"))
                Directory.CreateDirectory(PATH + "\\Logs\\Errors");
            if (!Directory.Exists(PATH + "\\Logs\\Warnings"))
                Directory.CreateDirectory(PATH + "\\Logs\\Warnings");
        }
    }
}
