using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using HtmlAgilityPack;
using System.Drawing;
using Console = Colorful.Console;

namespace MaidServer
{
    internal abstract class WebScarp
    {
        /// <summary>
        /// 检查服务器和端口是否可以连接
        /// </summary>
        /// <param name="ipString">服务器ip</param>
        /// <param name="port">端口</param>
        /// <returns>Type(bool) true or false</returns>
        public static bool CheckConnect(string ipString, int port)
        {
            System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient()
            { SendTimeout = 1000 };
            IPAddress ip = IPAddress.Parse(ipString);
            try
            {
                tcpClient.Connect(ip, port);
            }
            catch
            {
                return false;
            }
            bool checkStatus = tcpClient.Connected;
            tcpClient.Close();
            tcpClient.Dispose();
            return checkStatus;
        }

        /// <summary>
        /// 从连接爬取整个Html文档并返回
        /// </summary>
        /// <param name="html">网址</param>
        /// <returns>HtmlDocument</returns>
        public static HtmlDocument ScarpHtml(string html)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            try
            {
                doc = web.Load(html);
            }
            catch {}
            return doc;
        }

        //TODO:异常捕获+完善类
        public static void FromWeb()
        {
            var html = @"http://192.168.1.2:631/printers/";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(html);
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
            {
                Console.WriteLine("Found: " + table.Id);
                foreach (HtmlNode row in table.SelectNodes(".//tr"))
                {
                    Console.WriteLine("row");
                    foreach (HtmlNode cell in row.SelectNodes(".//th|td"))
                    {
                        Console.WriteLine("cell: " + cell.InnerText);
                    }
                }
            }
        }

    }

    internal abstract class CUPSInf : WebScarp
    {
        static IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build();
        private static readonly string CUPSServer = @configuration["ServcieEnv:CUPSServer"];
        private static readonly string CUPSPrinter = CUPSServer + @"/printers/";
        private static bool CUPSStatus = false;
        public static string CUPSVersion = "";
        static CUPSInf()
        {
            // 解析IP地址和端口，并检查是否可以
            int ipIndex = CUPSServer.LastIndexOf("http://") + 7;
            int portIndex = CUPSServer.LastIndexOf(":") + 1;
            string CUPSServerIpAddress = CUPSServer.Substring(ipIndex, portIndex - ipIndex - 1);
            string CUPSServerPort = "";
            for(int i = portIndex; CUPSServer[i] != '/'; i++)
            {
                CUPSServerPort += CUPSServer[i];
            }
            CUPSStatus = CheckConnect(CUPSServerIpAddress, int.Parse(CUPSServerPort));
            if(CUPSStatus)
            {
                var doc = ScarpHtml(@CUPSServer);
                try
                {
                    foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div[@class='row']"))
                    {
                        foreach (HtmlNode h1 in div.SelectNodes(".//h1"))
                        {
                            CUPSVersion = h1.InnerHtml;
                        }
                    }
                }
                catch {}
            }
            Console.WriteLine(CUPSVersion);
        }
        public static bool CheckCUPS()
        {
            //a
            return false;
        }
    }
}