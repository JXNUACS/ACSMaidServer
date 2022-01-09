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
    }
}


