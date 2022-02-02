using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace Egate_Ecommerce.Converters
{
    public class BitmapImageConverter : IValueConverter
    {
        public int DecodePixedWidth { get; set; }
        public int DecodePixelHeight { get; set; }

        private static List<ImageItem> _imageCache = new List<ImageItem>();

        public BitmapImageConverter()
        {
            DecodePixedWidth = -1;
            DecodePixelHeight = -1;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;
            ImageItem item = _imageCache.FirstOrDefault(i => i.Path == path && i.DecodePixedWidth == this.DecodePixedWidth && i.DecodePixelHeight == this.DecodePixelHeight);
            if (item == null)
            {
                item = new ImageItem(path, this.DecodePixedWidth, this.DecodePixelHeight);
                _imageCache.Add(item);
            }
            return item.Image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static void ClearCache(string path)
        {
            _imageCache.RemoveAll(i => !string.IsNullOrEmpty(i.Path) && Path.GetFullPath(i.Path) == Path.GetFullPath(path));
        }

        private class ImageItem
        {
            public string Path { get; set; }
            public int DecodePixedWidth { get; set; }
            public int DecodePixelHeight { get; set; }
            public BitmapImage Image { get; set; }

            private static BitmapImage _defaultImage;

            public ImageItem(string path, int width, int height)
            {
                this.Path = path;
                this.DecodePixedWidth = width;
                this.DecodePixelHeight = height;
                CreateImage();
            }

            private void CreateImage()
            {
                if (File.Exists(Path))
                {
                    Image = new BitmapImage();
                    Image.BeginInit();
                    if (this.DecodePixedWidth >= 0) Image.DecodePixelWidth = this.DecodePixedWidth;
                    if (this.DecodePixelHeight >= 0) Image.DecodePixelHeight = this.DecodePixelHeight;
                    Image.CacheOption = BitmapCacheOption.OnLoad;
                    Image.UriSource = new Uri(Path, UriKind.RelativeOrAbsolute);
                    try
                    {
                        Image.EndInit();
                    }
                    catch (FileFormatException)
                    {
                        Image = null;
                    }
                }
                if (Image == null)
                    Image = GetDefaultImage();
            }

            private static BitmapImage GetDefaultImage()
            {
                if (_defaultImage == null)
                {
                    _defaultImage = new BitmapImage();
                    _defaultImage.BeginInit();
                    _defaultImage.CacheOption = BitmapCacheOption.OnLoad;
                    _defaultImage.UriSource = new Uri("pack://application:,,,/Egate Ecommerce;component/res/no image available.jpg", UriKind.RelativeOrAbsolute);
                    _defaultImage.EndInit();
                }
                return _defaultImage;
            }
        }
    }
}
