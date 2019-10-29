using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Web_Reports.Models
{
    //[Table("UsersGroup")]
    [Table("PARAM")]
    public class Params
    {
        [Key]
        public DateTime Period { get; set; }
    }
}