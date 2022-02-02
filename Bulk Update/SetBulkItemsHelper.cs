using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Linq.Expressions;
using System.Diagnostics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using bolt5.CustomMappingObject.LinkedMapping;

namespace Bulk_Update
{
    public static class SetBulkItemsHelper
    {
        private static void SaveWorkbook(string file, IWorkbook workbook)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
        }

        public static void SetShopeeBulkItems2(string excelFile, IEnumerable<BulkItem> items)
        {
            string json = JsonConvert.SerializeObject(items);
            string jsonFile = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(excelFile) + ".json");
            File.WriteAllText(jsonFile, json);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            string exeFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "py", "excel shopee.exe");
            startInfo.WorkingDirectory = Path.GetDirectoryName(exeFile);
            startInfo.FileName = exeFile;
            startInfo.Arguments = string.Format("--file \"{0}\" --json \"{1}\"", excelFile, jsonFile);

            Process p = new Process();
            p.StartInfo = startInfo;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) Logs.Write(e.Data); };
            p.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) Logs.Write(e.Data); };
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
        }

        public static void SetWoocommerceBulkItems(string csvFile, IEnumerable<BulkItem> items, IEnumerable<Expression<Func<BulkItem, object>>> propertiesToUpdate)
        {
            List<string[]> rows = new List<string[]>();
            //get all rows, including headers
            using (FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamReader reader = new StreamReader(fs);
                CsvConfiguration config = new CsvConfiguration();
                config.Delimiter = ",";
                CsvReader csvReader = new CsvReader(reader, config);

                //get headers
                csvReader.ReadHeader();
                rows.Add(csvReader.FieldHeaders);

                //prepare mapping
                var mapping = new LinkedMappingObject<BulkItem>(csvReader.FieldHeaders, BulkValues._wcMap2);

                //get rows
                while (csvReader.Read())
                    rows.Add(csvReader.CurrentRecord);

                //change values of rows
                foreach (var item in items)
                {
                    foreach (var prop in propertiesToUpdate)
                    {
                        rows[item.RowIndex][mapping.IndexOf(prop)] = Convert.ToString(mapping.GetValue(item, prop));
                    }
                }
            }
            //set all rows and save
            using (FileStream fs = new FileStream(csvFile, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter writer = new StreamWriter(fs);
                CsvConfiguration config = new CsvConfiguration();
                config.Delimiter = ",";

                using (CsvWriter csvWriter = new CsvWriter(writer, config))
                {
                    foreach (var row in rows)
                    {
                        foreach (var x in row)
                        {
                            csvWriter.WriteField(x);
                        }
                        csvWriter.NextRecord();
                    }
                }
            }
        }

        public static void UpdateWoocommerceSingleBulkItem(string csvFile, BulkItem item, IEnumerable<Expression<Func<BulkItem, object>>> propertiesToUpdate)
        {
            List<string[]> rows = new List<string[]>();
            //get all rows, including headers
            using (FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamReader reader = new StreamReader(fs);
                CsvConfiguration config = new CsvConfiguration();
                config.Delimiter = ",";
                CsvReader csvReader = new CsvReader(reader, config);

                //get headers
                csvReader.ReadHeader();
                rows.Add(csvReader.FieldHeaders);

                //prepare mapping
                var mapping = new LinkedMappingObject<BulkItem>(csvReader.FieldHeaders, BulkValues._wcMap2);

                //get rows
                while (csvReader.Read())
                    rows.Add(csvReader.CurrentRecord);

                //change value
                foreach (var prop in propertiesToUpdate)
                {
                    rows[item.RowIndex][mapping.IndexOf(prop)] = Convert.ToString(mapping.GetValue(item, prop));
                }
            }
            //set all rows and save
            using (FileStream fs = new FileStream(csvFile, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter writer = new StreamWriter(fs);
                CsvConfiguration config = new CsvConfiguration();
                config.Delimiter = ",";
                using (CsvWriter csvWriter = new CsvWriter(writer, config))
                {
                    foreach (var row in rows)
                    {
                        foreach (var x in row)
                        {
                            csvWriter.WriteField(x);
                        }
                        csvWriter.NextRecord();
                    }
                }
            }
        }
    }
}
