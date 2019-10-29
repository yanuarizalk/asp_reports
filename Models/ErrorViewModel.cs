
using System;

namespace ASP_Web_Reports.Models
{
    public class ErrorViewModel : Exception
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
    }
}