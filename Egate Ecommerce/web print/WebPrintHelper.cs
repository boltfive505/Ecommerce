using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using Egate_Ecommerce.web_print.objects;

namespace Egate_Ecommerce.web_print
{
    public static class WebPrintHelper
    {
        public static string GenerateXmlForScheduleList(IEnumerable<Objects.Tutorials.TutorialVideoViewModel> list)
        {
            List<ScheduleXml> scheduleList = list.Select(i => new ScheduleXml()
            {
                Title = i.ShortDescriptionSimpleText,
                Category = i.Category?.CategoryName,
                EntryLevel = i.EntryLevel,
                AssignedTo = i.EmployeeAssignedTo?.EmployeeName
            }).ToList();
            return SerializeXml(scheduleList);
        }

        public static string GenerateXmlForShipList(IEnumerable<Objects.KoreaShipList.KoreaShipItemViewModel> shipItemList, Objects.KoreaShipList.KoreaShipSubtotalViewModel shipSubtotal)
        {
            ShipListXml shipXml = new ShipListXml();
            shipXml.ItemList = shipItemList.Select(i => new ShipListXml.ShipItem()
            {
                ItemNumber = i.Sku,
                ItemName = i.ItemName,
                ItemDescription = i.Specification,
                Quantity = i.Quantity ?? 0,
                ImagePath = i.ImagePath
            }).ToList();
            shipXml.Subtotal = new ShipListXml.ShipSubtotal()
            {
                ShipNumber = shipSubtotal.ShippingNumber,
                Quantity = shipSubtotal.PackingQuantity ?? 0,
                ShipBy = shipSubtotal.ShipBy,
                Status = shipSubtotal.Status
            };
            return SerializeXml(shipXml);
        }

        public static string GenerateXmlForBarcode(IEnumerable<Objects.KoreaShipList.KoreaShipItemViewModel> list)
        {
            List<BarcodeXml> barcodesList = new List<BarcodeXml>();
            foreach (var l in list)
            {
                for (int i = 0; i < l.Quantity; i++)
                    barcodesList.Add(new BarcodeXml() { ItemNumber = l.Sku, ItemName = l.ItemName });
            }
            return SerializeXml(barcodesList);
        }

        private static string SerializeXml<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                string xml = Encoding.UTF8.GetString(ms.ToArray());
                return xml;
            }
        }

        public static void Print(string path, string title)
        {
            var print = new Modals.web_printing_modal();
            print.WebPagePath = path;
            bolt5.ModalWpf.ModalForm.ShowModal(print, title);
        }
        
        public static string CreatePathForPrintableXmlWithStylesheetAndWebPagePack(string xml, string styleSheetName, string webpagePackName)
        {
            string zipPackResourcePath = "Egate_Ecommerce.web_print.webpage_packs." + webpagePackName + ".zip";
            string packOutputPath = Path.Combine(Path.GetTempPath(), webpagePackName);
            WebPrintHelper.ExtractZipContentFromResource(zipPackResourcePath, packOutputPath);
            string printOutputPath = Path.Combine(packOutputPath, "index.html");
            DoCreatePathForPrintableXmlWithStyleSheet(xml, styleSheetName, printOutputPath);
            return printOutputPath;
        }

        public static string CreatePathForPrintableXmlWithStyleSheet(string xml, string styleSheetName)
        {
            string tempFile = Path.GetTempFileName();
            DoCreatePathForPrintableXmlWithStyleSheet(xml, styleSheetName, tempFile);
            return tempFile;
        }

        private static void DoCreatePathForPrintableXmlWithStyleSheet(string xml, string styleSheetName, string targetPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XslCompiledTransform xsl = new XslCompiledTransform();
                XmlReaderSettings settings = new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse };
                Uri styleSheetUri = new Uri("pack://application:,,,/Egate Ecommerce;component/web print/stylesheets/" + styleSheetName + ".xsl", UriKind.RelativeOrAbsolute);
                XmlReader reader = XmlReader.Create(Application.GetResourceStream(styleSheetUri).Stream, settings);
                xsl.Load(reader);
                XmlTextWriter writer = new XmlTextWriter(targetPath, Encoding.UTF8);
                xsl.Transform(doc, null, writer);
            }
            catch (Exception ex)
            {
                Logs.WriteExceptionLogs(ex);
                File.WriteAllText(targetPath, "An error occured while processing the print form");
                System.Windows.MessageBox.Show("An error occured while processing the print form", "Web Printing", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //
        //https://ourcodeworld.com/articles/read/629/how-to-create-and-extract-zip-files-compress-and-decompress-zip-with-sharpziplib-with-csharp-in-winforms
        //
        private static void ExtractZipContentFromResource(string zipResourcePath, string outputFolder)
        {
            ZipFile file = null;
            try
            {
                Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(zipResourcePath);
                //FileStream fs = File.OpenRead(FileZipPath);
                file = new ZipFile(resourceStream);

                foreach (ZipEntry zipEntry in file)
                {
                    if (!zipEntry.IsFile)
                    {
                        // Ignore directories
                        continue;
                    }

                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // 4K is optimum
                    byte[] buffer = new byte[4096];
                    Stream zipStream = file.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outputFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch (IOException)
            { }
            finally
            {
                if (file != null)
                {
                    file.IsStreamOwner = true; // Makes close also shut the underlying stream
                    file.Close(); // Ensure we release resources
                }
            }
        }
    }
}
