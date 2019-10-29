using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ASP_Web_Reports.Models
{
    public class Groups
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Access_Default { get; set; }
    }
}