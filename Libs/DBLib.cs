using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ASP_Web_Reports.Libs {
    public class DBLib {

        public IConfiguration Configuration { get; }
        public SqlDataReader sqlReader { get; set; }

        public SqlCommand sqlCMD { get; set; }

        public DBLib() {
            sqlCMD.Connection.ConnectionString = Configuration.GetConnectionString("ManagementContext");
        }

    }
}
