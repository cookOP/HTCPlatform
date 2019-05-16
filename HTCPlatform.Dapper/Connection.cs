using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace HTCPlatform.Dapper
{
    /// <summary>
    /// 连接数据库地址
    /// </summary>
    public class DBConnection
    {
         public static  IDbConnection connection = new SqlConnection(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=HTCPlatform;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");       
    }
}
