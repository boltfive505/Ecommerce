using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using Egate_Ecommerce.Classes;

namespace Egate_Ecommerce.Objects
{
    public class ItemPicturesItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ItemNumber { get; set; }
        public string ImageNameSuffix { get; set; }
        public string ImagePath { get { return _imagePathCallback == null ? GetImagePathOther() : _imagePathCallback.Invoke(ItemNumber, ImageNameSuffix); } }

        public bool CanReplace { get; set; }
        public bool IsImageExists { get { return File.Exists(ImagePath); } }

        [CloneCopyIgnore]
        public RelayCommand DownloadCommand { get; set; }
        [CloneCopyIgnore]
        public RelayCommand ReplaceCommand { get; set; }

        private Func<string, string, string> _imagePathCallback;

        public ItemPicturesItem()
        {
            DownloadCommand = new RelayCommand(DownloadImage);
            ReplaceCommand = new RelayCommand(ReplaceImage);
        }

        public ItemPicturesItem(string itemNumber, string imageNameSuffix, bool canReplace = true) : this(itemNumber, imageNameSuffix, null, canReplace)
        { }

        public ItemPicturesItem(string itemNumber, string imageNameSuffix, Func<string, string, string> imagePathCallback, bool canReplace = true) : this()
        {
            this.ItemNumber = itemNumber;
            this.ImageNameSuffix = imageNameSuffix;
            this._imagePathCallback = imagePathCallback;
            this.CanReplace = canReplace;
        }

        private string GetImagePathOther()
        {
            string dir = @".\uploads\other item pictures";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, string.Format("{0}_{1}.jpg", ItemNumber, ImageNameSuffix));
        }

        private void DownloadImage(object obj)
        {
            if (!File.Exists(ImagePath)) return; //check if file exists

            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Save Image As";
            save.FileName = Path.GetFileName(ImagePath);
            save.Filter = "Any File|*.*";
            if (save.ShowDialog() == true)
            {
                File.Copy(ImagePath, save.FileName, true);
            }
        }

        private void ReplaceImage(object obj)
        {
            if (!CanReplace) return;
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Replace Image";
            open.Multiselect = false;
            open.Filter = "Image File|*.png;*.jpg;*.jpeg";
            if (open.ShowDialog() == true)
            {
                //delete old image
                if (File.Exists(ImagePath))
                    File.Delete(ImagePath);
                //save new image
                Helpers.SaveImageFileAsJpg(open.FileName, ImagePath);
                Converters.BitmapImageConverter.ClearCache(ImagePath);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImagePath))); //refresh binding
            }
        }
    }
}
