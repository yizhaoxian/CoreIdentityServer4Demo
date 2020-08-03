using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Study.Repository.Base
{
    static class DbConfig
    {
        private static IConfiguration Configuration;
        internal static string SqlConnectionString;

        static DbConfig()
        {
            if (Configuration == null)
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json", false, true);
                Configuration = builder.Build();
            }
            if (Configuration != null && string.IsNullOrEmpty(SqlConnectionString))
            {
                SqlConnectionString = Configuration.GetConnectionString("Default");
            }
        }
        internal static IDbConnection GetDbConnection()
        {
            var conn = new System.Data.SqlClient.SqlConnection(SqlConnectionString);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn;
        }
    }
}
