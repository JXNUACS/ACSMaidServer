# ACSMaidServer

基于.netcore开发的局域网服务系统的服务器端

## SQL API

### `internal abstract class SQL_USERS`

`public static Users QueryUsers(string username)：`查询指定用户并返回该用户对象的实例

`public static bool RegisterUsers(int operatorIdentity, string userName, string userPasswd, string type = "0")`：向服务器注册用户

`public static Users DeleteUsers(string userName, string userPasswd, int operatorIdentity)`：从服务器中删除指定用户

### `internal abstract class SQLTools`

`public static void Init()`：初始化MYSQL数据库

`public static MySqlConnection ConnectDB()`：创建一个MYSQL连接

`public static bool SQLQuery(string cmd)`：执行一条查询语句

`public static bool SQLQuery(string[] cmdset)`：执行多条查询语句
