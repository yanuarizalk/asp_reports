using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ASP_Web_Reports.Models
{
    public class TB_USER
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Dept_Nm { get; set; }
        public bool Status { get; set; }
        public string MenuTrusty { get; set; }
        public Int16 Userlevel { get; set; }
        public string UserPassword { get; set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            //using (var context = new )
        }
    }
}