using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ASP_Web_Reports.Models
{
    public class Menu
    {
        [Key]
        public int ID { get; set; }
        public int ID_Main { get; set; }
        public string Route { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}