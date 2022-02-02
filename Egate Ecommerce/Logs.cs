using System;
using System.Globalization;
using System.IO;

namespace Egate_Ecommerce
{
    public static class Logs
    {
        public static void WriteExceptionLogs(Exception ex)
        {
            DoWriteLog(ex.ToString());
        }

        private static void DoWriteLog(string msg)
        {
            string dir = @".\logs";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string file = Path.Combine(dir, "logs.txt");
            string contents = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}\n", DateTime.Now, msg);
            File.AppendAllText(file, contents);
        }
    }
}
