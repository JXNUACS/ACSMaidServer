using System;
using System.Collections;
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
            catch { }
            return doc;
        }
        
        protected static void Debug(Exception ex, Color C)
        {
            Console.WriteLine(@"*|WebScarp DEBUG|\> " + ex, C);
        }
        protected static void Debug(string str, Color C)
        {
            Console.WriteLine(@"*|WebScarp DEBUG|\> " + str, C);
        }
    }

    internal abstract class CUPSInf : WebScarp
    {
        static IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build();
        private static readonly string CUPSServer = @configuration["ServcieEnv:CUPSServer"];
        private static readonly string CUPSPrinter = CUPSServer + @"printers/";
        private static readonly bool DEBUG_MOD = configuration["env:DEBUG_MOD"] == "true" ? true : false;
        private static bool CUPSStatus = false;
        public static string CUPSVersion = "NULL";
        /// <summary>
        /// 静态构造函数，用于解析CUPS服务状态
        /// </summary>
        static CUPSInf()
        {
            // 解析IP地址和端口，并检查是否可以正常访问服务器
            int ipIndex = CUPSServer.LastIndexOf("http://") + 7;
            int portIndex = CUPSServer.LastIndexOf(":") + 1;
            string CUPSServerIpAddress = CUPSServer.Substring(ipIndex, portIndex - ipIndex - 1);
            string CUPSServerPort = "";
            for (int i = portIndex; CUPSServer[i] != '/'; i++)
            {
                CUPSServerPort += CUPSServer[i];
            }
            CUPSStatus = CheckConnect(CUPSServerIpAddress, int.Parse(CUPSServerPort));
            if (CUPSStatus)
            {
                var doc = ScarpHtml(@CUPSServer);
                try
                {
                    //解析CUPS地址页信息
                    foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div[@class='row']"))
                    {
                        foreach (HtmlNode h1 in div.SelectNodes(".//h1"))
                        {
                            CUPSVersion = h1.InnerHtml;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if(DEBUG_MOD)
                        Debug(ex, Color.Red);
                }
            }
            //Console.WriteLine(CUPSVersion);
        }
        /// <summary>
        /// 查看CUPS服务状态
        /// </summary>
        /// <returns></returns>
        public static bool CheckCUPS()
        {
            if (CUPSStatus && CUPSVersion.Contains("CUPS"))
            {
                return true;
            }
            else
            {
                CUPSStatus = false;
                return false;
            }
        }
        /// <summary>
        /// 获取打印机机信息
        /// </summary>
        /// <returns></returns>
        public static List<Printer> GetPrinters()
        {
            List<Printer> printerList = new List<Printer>();
            if (CUPSStatus)
            {
                var doc = ScarpHtml(CUPSPrinter);
                try
                {
                    //解析CUPS打印机信息
                    foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
                    {
                        //Console.WriteLine("Found: " + table.Id);
                        foreach (HtmlNode row in table.SelectNodes(".//tr"))
                        {
                            HtmlNodeCollection rowData = row.SelectNodes(".//th|td");
                            List<string> res = rowData.Select(i => i.InnerText).ToList();
                            if (res[0] == "Queue Name")
                                continue;
                            printerList.Add(new Printer(res[0], res[1], res[2], res[3], res[4]));
                        }
                    }
                }
                catch {}
            }
            /*
            foreach(var p in printerList)
            {
                Console.WriteLine(p);
            }*/
            return printerList;
        }
        /// <summary>
        /// 获取当前打印队列信息
        /// </summary>
        /// <returns></returns>
        public static List<PrintJobs> GetCurrentPrintJobs()
        {
            List<PrintJobs> jobsList = new List<PrintJobs>();
            if (CUPSStatus)
            {
                var doc = ScarpHtml(CUPSServer + @"jobs");
                try
                {
                    //解析CUPS打印机信息
                    foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
                    {
                        //Console.WriteLine("Found: " + table.Id);
                        foreach (HtmlNode row in table.SelectNodes(".//tr"))
                        {
                            HtmlNodeCollection rowData = row.SelectNodes(".//th|td");
                            List<string> res = rowData.Select(i => i.InnerText.Replace(@"&nbsp;", string.Empty)).ToList();
                            if (res[0].Contains("ID") || res.Count < 5)
                                continue;
                            jobsList.Add(new PrintJobs(res[0], res[1], res[2], res[3], res[4], res[5], res[6]));
                        }
                    }
                }
                catch(Exception ex)
                {
                    if(DEBUG_MOD)
                        Debug(ex, Color.Red);
                }
            }
            return jobsList;
        }
    }
}