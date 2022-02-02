using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using HtmlAgilityPack;
using purchase_request.Model;

namespace Egate_Ecommerce.Classes
{
    public static class LuckyRecordsHelper
    {
        public static IEnumerable<lucky> GetLuckyRecordsByKey(string key)
        {
            Uri luckyUrl = new Uri("http://www.lucky2488.com/kor/20man/trans.asp?inf=" + key + "");
            HtmlDocument doc = new HtmlDocument();
            WebClient client = new WebClient();
            doc.Load(client.OpenRead(luckyUrl), Encoding.UTF8, true);

            var table = doc.DocumentNode.SelectSingleNode("table");
            var rows = table.SelectNodes("tr");
            if (rows.Count <= 1) return new List<lucky>(); //return blank list
            //begin fetching records
            var items = rows.Skip(1)
                .Select(r =>
                {
                    var datas = r.SelectNodes("td");
                    lucky lucky = new lucky();
                    lucky.Key = key;
                    lucky.ArriveKorea = DateTime.Parse(datas[0].InnerText.Trim()).ToUnixLong();
                    lucky.Location = datas[1].InnerText.Trim();
                    lucky.ProcessDate = DateTime.Parse(datas[2].InnerText.Trim()).ToUnixLong();
                    lucky.ItemName = datas[3].InnerText.Trim();
                    lucky.Quantity = int.Parse(datas[4].InnerText.Trim());
                    lucky.Weight = decimal.Parse(datas[5].InnerText.Trim());
                    lucky.CBM = datas[6].InnerText.Trim();
                    lucky.Memo1 = datas[7].InnerText.Trim().Replace("&nbsp;", string.Empty);
                    lucky.Memo2 = datas[8].InnerText.Trim().Replace("&nbsp;", string.Empty);
                    lucky.ShippingLabel = datas[9].InnerText.Trim().Replace("&nbsp;", string.Empty);
                    return lucky;
                })
                .Where(l => !string.IsNullOrEmpty(l.ShippingLabel));
            return items;
        }

        public static IEnumerable<lucky> GetLuckyItemsFromClipboard(string key)
        {
            try
            {
                string data = Clipboard.GetText(TextDataFormat.UnicodeText);
                string[] rows = data.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var luckyItems = rows.Select(r =>
                    {
                        string[] fields = r.Split("\t".ToCharArray(), StringSplitOptions.None);

                        var lucky = new lucky();
                        lucky.Key = key;
                        lucky.ArriveKorea = DateTime.Parse(fields[0].Trim()).ToUnixLong();
                        lucky.Location = fields[1].Trim();
                        lucky.ProcessDate = DateTime.Parse(fields[2].Trim()).ToUnixLong();
                        lucky.ItemName = fields[3].Trim();
                        lucky.Quantity = int.Parse(fields[4].Trim());
                        lucky.Weight = decimal.Parse(fields[5].Trim());
                        lucky.CBM = fields[6].Trim();
                        lucky.Memo1 = fields[7].Trim();
                        lucky.Memo2 = fields[8].Trim();
                        lucky.ShippingLabel = fields[9].Trim();
                        return lucky;
                    })
                    .Where(l => !string.IsNullOrEmpty(l.ShippingLabel));
                return luckyItems;
            }
            catch (Exception ex)
            {
                Logs.WriteExceptionLogs(ex);
                MessageBox.Show("Copied data is invalid", "Lucky - Insert from Clipboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }
    }
}
