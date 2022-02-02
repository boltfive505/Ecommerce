using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Bulk_Update
{
    public static class Logs
    {
        private static object _lockObj = new object();

        public static void Write(string msg)
        {
            Logs.DoWriteLog(msg);
        }

        public static void WriteException(Exception ex)
        {
            Logs.DoWriteLog(ex.ToString());
        }

        private static void DoWriteLog(string msg)
        {
            lock (_lockObj)
            {
                string dir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "logs");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, "bulk update_logs.txt");
                string content = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}\n", DateTime.Now, msg);
                File.AppendAllText(file, content);
            }
        }
    }
}
