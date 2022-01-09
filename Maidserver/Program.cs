using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace MaidServer
{


    public class MainService
    {

        static void Main(string[] args)
        {
            ServerInit();
            //Console.WriteLine(SqlHelper.QueryUsers("admin").name);         
            //SQL_USERS.RegisterUsers(0,"testuser", "test");
            //SQL_USERS.DeleteUsers("testuser", "test", 0);
            //WebScarp.FromWeb();
            //CUPSInf.GetCurrentPrintJobs();
        }

        private static void ServerInit()
        {
            NoticeModel N = new NoticeModel();
            N.BeepNotice("*-*-*-|Welcome to Association of Computer Science!|-*-*-*", 1);
            SocketServer socketServer = new SocketServer(2000);
            socketServer.StartListen();
        }

    }




}