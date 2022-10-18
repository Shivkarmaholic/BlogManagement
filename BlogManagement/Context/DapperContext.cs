﻿using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BlogManagement.Context
{
    public class DapperContext
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
       
    }
}
