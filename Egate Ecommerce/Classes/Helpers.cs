using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Egate_Ecommerce
{
    public static class Helpers
    {
        public static byte[] GetBarcodeData(this string value, int width = 150, int height = 65, ZXing.BarcodeFormat format = ZXing.BarcodeFormat.CODE_128)
        {
            try
            {
                ZXing.BarcodeWriter b = new ZXing.BarcodeWriter();
                b.Format = format;
                b.Options.PureBarcode = true;
                b.Options.Width = width;
                b.Options.Height = height;
                var bitmap = b.Write(value);
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] data = ms.ToArray();
                    return data;
                }
            }
            catch (Exception ex)
            {
                Logs.WriteExceptionLogs(ex);
                return new byte[0];
            }
        }

        public static void CopyToClipboard(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Clipboard not working. Try again", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static byte[] GetImageDataFromFile(string file)
        {
            byte[] data = null;
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(file);
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    data = ms.ToArray();
                }
            }
            catch
            { }
            return data;
        }

        public static string GetAdminPassword()
        {
            string file = FileHelper.GetFile("admin pw.txt", "data");
            if (!File.Exists(file))
                File.WriteAllText(file, "123456");
            return File.ReadAllText(file);
        }

        public static void SaveImageFileAsJpg(string srcFile, string destFile)
        {
            BitmapFrame frame = BitmapDecoder.Create(new Uri(srcFile, UriKind.Absolute), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            int imgWidth = frame.PixelWidth;
            int imgHeight = frame.PixelHeight;
            Rect rect = new Rect(0, 0, imgWidth, imgHeight);
            BitmapSource bgFrame = CreateBitmapSource(imgWidth, imgHeight, Color.FromArgb(255, 255, 255, 255));

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawImage(bgFrame, rect);
                dc.DrawImage(frame, rect);
            }
            RenderTargetBitmap bmp = new RenderTargetBitmap(imgWidth, imgHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(dv);

            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(fs);
            }
        }

        private static BitmapSource CreateBitmapSource(int width, int height, Color color)
        {
            //int stride = width / 8;
            int stride = ((width * 32 + 31) & ~31) / 8;
            byte[] pixels = new byte[height * stride];

            List<Color> colors = new List<Color>();
            colors.Add(color);
            BitmapPalette myPalette = new BitmapPalette(colors);

            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);

            return image;
        }
    }
}
