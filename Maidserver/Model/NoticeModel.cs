using System.Diagnostics;
using System.Drawing;
using Console = Colorful.Console;

namespace MaidServer
{
    public class NoticeModel
    {
        public void BeepNotice(string content, int level)
        {
            try
            {
                Console.WriteLine(content, Color.DarkSeaGreen);
                var psi = new ProcessStartInfo("beep") {RedirectStandardOutput = false};
                Process.Start(psi);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex, Color.Red);
            }
        }

        public void NewClientConnection(string deviceName, string ipAddress, string port)
        {
            try
            {
                Console.WriteLine("===========New Clent Connection============", Color.Green);
                Console.WriteLine("|*| Device Name: {0}", deviceName, Color.Gold);
                Console.WriteLine("|*| Device IP Address: {0}", ipAddress, Color.Purple);
                Console.WriteLine("|*| Device Port: {0}", port, Color.Salmon);
                Console.WriteLine("===========================================", Color.Green);
            }
            catch {}
        }

        public void DisconnectionMsg(string deviceName, string ipAddress, string port)
        {
                        try
            {
                Console.WriteLine("=============Clent Disconnected============", Color.Pink);
                Console.WriteLine("|-| Device Name: {0}", deviceName, Color.Gold);
                Console.WriteLine("|-| Device IP Address: {0}", ipAddress, Color.Purple);
                Console.WriteLine("|-| Device Port: {0}", port, Color.Salmon);
                Console.WriteLine("===========================================", Color.Pink);
            }
            catch {}
        }
    }
}


