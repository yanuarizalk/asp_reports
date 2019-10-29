using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Web_Reports.Models
{
    //[Table("UsersGroup")]
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ID_Group { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Access { get; set; }
        public bool Status { get; set; }
        public bool Logged_In { get; set; }
        public string Email { get; set; }
        //public Groups Group { get; set; }
        //[Editable(false)]
        //public string? Role { set; get; }
    }
}