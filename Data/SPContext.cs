using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ASP_Web_Reports.Models
{
    public class SPContext : DbContext
    {
        public SPContext (DbContextOptions<SPContext> options)
            : base(options)
        {
        }

        public DbSet<ASP_Web_Reports.Models.TB_USER> TB_USER { get; set; }
        public DbSet<ASP_Web_Reports.Models.TB_USER> TB_MENU { get; set; }
    }
}
