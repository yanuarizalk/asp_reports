using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ASP_Web_Reports.Models
{
    public class TB_MENU
    {
        [Key]
        public string MenuId { get; set; }
        public string Caption { get; set; }
        public string Descriptions { get; set; }
        public Int16 SubMenu { get; set; }
        public Int16 LevelMenu { get; set; }
        public string MenuForm { get; set; }
        public string Parameter { get; set; }
        public string ParentMenuId { get; set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            //using (var context = new )
        }
    }
}