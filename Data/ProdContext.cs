using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ASP_Web_Reports.Models
{
    public class ProdContext : LocalContext {
        /*public LocalContext (DbContextOptions<LocalContext> options)
            : base(options) {

        }*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("ProdContext"));
        }
    }
}
