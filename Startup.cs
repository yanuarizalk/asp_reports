using ASP_Web_Reports.Controllers;
//using ASP_Web_Reports.Libs.Helper;
using ASP_Web_Reports.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
//using System.Security.Claims;

namespace ASP_Web_Reports {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
            MgmContext.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string CorsPolicy = "_Origin";
        //readonly string[] AllowOrigin = { "http://localhost", "http://127.0.0.1", "http://192.168.0.158" };


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            //services.AddSingleton<IConfiguration>(Configuration);
            services.Configure<IISOptions>(options => {
                options.AutomaticAuthentication = false;
            });
            services.Configure<CookiePolicyOptions>(options => {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddCors(options => {
                options.AddPolicy(CorsPolicy, builder => {
                    builder.AllowAnyOrigin(); builder.AllowAnyMethod(); builder.AllowAnyHeader();
                });
            });
            //services.AddSingleton<MRSApp>();
            //services.AddScoped<IHostingEnvironment, >()
            services.AddMvc()/*.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver())*/.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddDistributedSqlServerCache(options => {
                options.ConnectionString = Configuration.GetConnectionString("ProdContext");
                options.SchemaName = "dbo";
                options.TableName = "Sessions";
            });
            services.AddSession(options => {
                options.Cookie.IsEssential = false;
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromHours(9);
                options.Cookie.Name = "tyf.management";
            });
            services.AddMvc(options => {
                options.EnableEndpointRouting = false;
                //options.RespectBrowserAcceptHeader = true;
                //options.Filters.Add(new ProducesAttribute("application/json"));
            })/*.AddNewtonsoftJson()*/;
            /*services.AddAuthentication("Cookies").AddCookie(cookie => {

            }).AddCookie();*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            MainFunc.hosting = env;
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
            }
            FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
            app.UseFastReport(fr => {
                fr.RouteBasePath = "/Report";
            });
            app.UseStaticFiles();
            
            app.UseSession();
            app.UseCors(CorsPolicy);
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    "others",
                    "{*.}", new {
                    //"{*.:regex(^((?!_fr).)*$)}", new {
                        controller = "Base", action = "Others"
                    }
                );
            });
        }

    }
    public class MsSqlDataConnection : FastReport.Data.DataConnectionBase {
        public override string QuoteIdentifier(string value, System.Data.Common.DbConnection connection) {
            return "\"" + value + "\"";
        }

        public override System.Type GetConnectionType() {
            return typeof(System.Data.SqlClient.SqlConnection);
        }

        public override System.Type GetParameterType() {
            return typeof(System.Data.SqlDbType);
        }

        public override System.Data.Common.DbDataAdapter GetAdapter(string selectCommand, System.Data.Common.DbConnection connection, FastReport.Data.CommandParameterCollection parameters) {
            System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(selectCommand, connection as System.Data.SqlClient.SqlConnection);
            foreach (FastReport.Data.CommandParameter p in parameters) {
                System.Data.SqlClient.SqlParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (System.Data.SqlDbType)p.DataType, p.Size);
                parameter.Value = p.Value;
            }
            return adapter;
        }
        public override void InitializeComponent() {
            base.InitializeComponent();
            
        }
    }
}
