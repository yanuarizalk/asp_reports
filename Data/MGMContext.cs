using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ASP_Web_Reports.Models
{
    public class MgmContext : DbContext {
        public static IConfiguration Configuration { get; set; }
        /*public LocalContext (DbContextOptions<LocalContext> options)
            : base(options) {

        }*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("ProdContext"));
        }
        /*protected override void OnModelCreating(ModelBuilder mb) {
            mb.Entity<Users>(entity => {
                entity.ToView
            });
        }*/
        public DbSet<ASP_Web_Reports.Models.Users> users { get; set; }
        public DbSet<ASP_Web_Reports.Models.Groups> groups { get; set; }
        public DbSet<ASP_Web_Reports.Models.Menu> menu { get; set; }
        public DbSet<ASP_Web_Reports.Models.SalesGraph> SalesGraph { get; set; }
        public DbSet<ASP_Web_Reports.Models.Params> Params { get; set; }
    }
}
