using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using Microsoft.Win32;

namespace Egate_Ecommerce.Templates
{
    [TemplatePart(Name = "PART_DownloadButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ReplaceButton", Type = typeof(Button))]
    public class ImagePreviewPopup : Control
    {
        private const string PART_DOWNLOAD_BUTTON = "PART_DownloadButton";
        private const string PART_REPLACE_BUTTON = "PART_ReplaceButton";

        private Button downloadBtn;
        private Button replaceBtn;

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(ImagePreviewPopup));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(PlacementMode), typeof(ImagePreviewPopup), new PropertyMetadata(PlacementMode.Right));
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(string), typeof(ImagePreviewPopup));
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        static ImagePreviewPopup()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagePreviewPopup), new FrameworkPropertyMetadata(typeof(ImagePreviewPopup)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            downloadBtn = this.GetTemplateChild(PART_DOWNLOAD_BUTTON) as Button;
            replaceBtn = this.GetTemplateChild(PART_REPLACE_BUTTON) as Button;

            downloadBtn.Click += DownloadBtn_Click;
            replaceBtn.Click += ReplaceBtn_Click;
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(ImageSource)) return; //check if file exists

            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Save Image As";
            save.FileName = Path.GetFileName(ImageSource);
            save.Filter = "Any File|*.*";
            if (save.ShowDialog() == true)
            {
                File.Copy(ImageSource, save.FileName, true);
            }
        }

        private void ReplaceBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Replace Image";
            open.Multiselect = false;
            open.Filter = "Image File|*.png;*.jpg;*.jpeg";
            if (open.ShowDialog() == true)
            {
                Helpers.SaveImageFileAsJpg(open.FileName, ImageSource);
                Converters.BitmapImageConverter.ClearCache(ImageSource);
            }
        }
    }
}
