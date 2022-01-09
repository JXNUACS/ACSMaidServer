using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Drawing;
using MySql.Data;
using Dapper;
using Console = Colorful.Console;

namespace MaidServer{
    internal abstract class SQL_USERS
    {
        /// <summary>
        /// Get the configuration from the appsettings.json files
        /// </summary>
        static IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build();
        private static readonly string connectionString = configuration["DBSetting:ConnectString"];
        private static readonly bool debugMode = configuration["env:DEBUG_MODE"] == "true" ? true : false;
        /// <summary>
        /// 向MYSQL服务器查询是否存在用户，存在则返回用户，否则返回非法用户
        /// </summary>
        /// <param name="username">待查询的用户名</param>
        public static Users QueryUsers(string username){
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 
            try
            {
                string sqlQuery = "SELECT * FROM `users` WHERE name=\"" + username + "\""; 
                if (debugMode)
                    Console.WriteLine("*|DEBUG::SQL_STATEMENT/> {0}", sqlQuery, Color.AliceBlue);
                MySqlConnection con = new MySqlConnection(connectionString);
                con.Open();
                MySqlCommand sqlcmd = new MySqlCommand(sqlQuery, con);
                MySqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                sqlrdr.Read();
                Users targetUser = new Users((int)sqlrdr[0], (string)sqlrdr[1], (string)sqlrdr[2], (int)sqlrdr[3]);;
                con.Close();
                con.Dispose();
                return targetUser;
                //var res = con.Query<Users>(sqlQuery);
                //Console.WriteLine(res);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Pink);
                return new Users(-1, "", "", -1);
            }
        }
        /// <summary>
        /// 向服务器注册一个新用户，注册成功则返回True，否则返回False
        /// </summary>
        /// <param name="operatorIdentity">The identity of operator</param>
        /// <param name="userName">The name of registing user</param>
        /// <param name="userPasswd">The password of registing user</param>
        /// <param name="type">The type of registing user</param>
        /// <returns>Type(Bool)</returns>
        public static bool RegisterUsers(int operatorIdentity, string userName, string userPasswd, string type = "0"){
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 
            try
            {
                if (operatorIdentity != 0)
                    throw (new PermissionsDenied("You haven't got the permission to operate!"));
                string sqlQuery = string.Format("INSERT INTO users values(null, \"{0}\", \"{1}\", \"{2}\");", userName, userPasswd, type);
                if (debugMode)
                    Console.WriteLine("+|DEBUG::SQL_STATEMENT/> {0}", sqlQuery, Color.Green);
                MySqlConnection con = new MySqlConnection(connectionString);
                con.Open();
                MySqlCommand sqlcmd = new MySqlCommand(sqlQuery, con);
                sqlcmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Pink);
                return false;
            }
        }
        /// <summary>
        /// 从服务器删除一个用户，并返回该用户的实例
        /// </summary>
        /// <param name="userName">The name of registing user</param>
        /// <param name="userPasswd">The password of registing user</param>
        /// <param name="operatorIdentity">The identity of operator</param>
        /// <returns></returns>
        public static Users DeleteUsers(string userName, string userPasswd, int operatorIdentity){
            try
            {
                if (operatorIdentity != 0)
                    throw (new PermissionsDenied("You haven't got the permission to operate!"));
                var tmp = QueryUsers(userName);
                if (!tmp.checkPasswd(userPasswd))
                    throw (new CommonException(string.Format("User:{0}'s password validated faild!", tmp.name)));
                string sqlQuery = string.Format("DELETE FROM users WHERE name=\"{0}\";", userName);
                if (debugMode)
                    Console.WriteLine("-|DEBUG::SQL_STATEMENT/> {0}", sqlQuery, Color.Pink);
                MySqlConnection con = new MySqlConnection(connectionString);
                con.Open();
                MySqlCommand sqlcmd = new MySqlCommand(sqlQuery, con);
                sqlcmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                return tmp;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Pink);
                return new Users(-1, "", "", -1);
            }
        }

    }

    internal abstract class SQLTools
    {
        /// <summary>
        /// Get the configuration from the appsettings.json files
        /// </summary>
        static IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build();
        private static readonly string connectionString = configuration["DBSetting:ConnectString"];
        private static readonly bool debugMode = configuration["env:DEBUG_MODE"] == "true" ? true : false;
        /// <summary>
        /// 用于初始化数据表，未测试
        /// </summary>
        public static void Init()
        {
            string[] cmdset = new string[]
            {
                "CREATE DATABASE IF NOT EXISTS maidserverdb DEFAULT CHARSET utf8mb4;",
                "USE maidserverdb; CREATE TABLE `users` (`account` int(11) NOT NULL AUTO_INCREMENT,`name` TEXT NOT NULL, `passwd` TEXT NOT NULL, `identity` TEXT NOT NULL, PRIMARY KEY(`account`));",
            };
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString);
                con.Open();
                foreach(var cmd in cmdset)
                {
                    if (debugMode)
                        Console.WriteLine("-|DEBUG::SQL_STATEMENT/> {0}", cmd, Color.Pink);
                    MySqlCommand sqlcmd = new MySqlCommand(cmd, con);
                    sqlcmd.ExecuteNonQuery();
                }
                con.Close();
                con.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Pink);
            }
            
        }
        /// <summary>
        /// 用于创建一个Mysql连接并返回该连接的实例
        /// </summary>
        /// <returns>Type(MySqlConnection)</returns>
        public static MySqlConnection ConnectDB()
        {
            try
            {
                return new MySqlConnection(connectionString);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Pink);
            }
            return null;
        }
        /// <summary>
        /// 用于执行单条SQL查询语句
        /// </summary>
        /// <param name="cmd">SQL查询语句</param>
        /// <returns>执行成功返回true，否则返回false</returns>
        public static bool SQLQuery(string cmd)
        {
            try
            {
                if (debugMode)
                    Console.WriteLine("-|DEBUG::SQL_STATEMENT/> {0}", cmd, Color.Gold);
                var con = ConnectDB();
                con.Open();    
                MySqlCommand sqlcmd = new MySqlCommand(cmd, con);
                sqlcmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Pink);
            }
            return false;
        }
        /// <summary>
        /// 用于执行多条SQL查询语句
        /// </summary>
        /// <param name="cmdset">语句集合</param>
        /// <returns>执行成功返回true，否则返回false</returns>
        public static bool SQLQuery(string[] cmdset)
        {
            try
            {
                var con = ConnectDB();
                con.Open();   
                foreach(string cmd in cmdset)
                {
                    if (debugMode)
                        Console.WriteLine("-|DEBUG::SQL_STATEMENT/> {0}", cmd, Color.Gold);
                    MySqlCommand sqlcmd = new MySqlCommand(cmd, con);
                    sqlcmd.ExecuteNonQuery();
                } 
                con.Close();
                con.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Pink);
            }
            return false;
        }
    }
}