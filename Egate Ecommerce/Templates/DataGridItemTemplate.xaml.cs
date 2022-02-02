using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using Microsoft.Win32;
using Egate_Ecommerce.Objects.MobileInventory;

namespace Egate_Ecommerce.Templates
{
    /// <summary>
    /// Interaction logic for DataGridItemTemplate.xaml
    /// </summary>
    public partial class DataGridItemTemplate : UserControl
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(DataGridItemTemplate));
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSizeProperty = DependencyProperty.Register("ImageSize", typeof(double), typeof(DataGridItemTemplate), new PropertyMetadata(60.0d));
        public double ImageSize
        {
            get { return (double)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DataGridItemTemplate));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty DepartmentProperty = DependencyProperty.Register("Department", typeof(string), typeof(DataGridItemTemplate));
        public string Department
        {
            get { return (string)GetValue(DepartmentProperty); }
            set { SetValue(DepartmentProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(DataGridItemTemplate));
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty ShowQuantityProperty = DependencyProperty.Register("ShowQuantity", typeof(bool), typeof(DataGridItemTemplate), new PropertyMetadata(true));
        public bool ShowQuantity
        {
            get { return (bool)GetValue(ShowQuantityProperty); }
            set { SetValue(ShowQuantityProperty, value); }
        }

        public static readonly DependencyProperty QuantityProperty = DependencyProperty.Register("Quantity", typeof(int), typeof(DataGridItemTemplate));
        public int Quantity
        {
            get { return (int)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }

        public static readonly DependencyProperty RequestedQuantityProperty = DependencyProperty.Register("RequestedQuantity", typeof(int), typeof(DataGridItemTemplate));
        public int RequestedQuantity
        {
            get { return (int)GetValue(RequestedQuantityProperty); }
            set { SetValue(RequestedQuantityProperty, value); }
        }

        public static readonly DependencyProperty PreparedQuantityProperty = DependencyProperty.Register("PreparedQuantity", typeof(int), typeof(DataGridItemTemplate));
        public int PreparedQuantity
        {
            get { return (int)GetValue(PreparedQuantityProperty); }
            set { SetValue(PreparedQuantityProperty, value); }
        }

        public static readonly DependencyProperty PackedQuantityProperty = DependencyProperty.Register("PackedQuantity", typeof(int), typeof(DataGridItemTemplate));
        public int PackedQuantity
        {
            get { return (int)GetValue(PackedQuantityProperty); }
            set { SetValue(PackedQuantityProperty, value); }
        }

        public static readonly DependencyProperty ShippedQuantityProperty = DependencyProperty.Register("ShippedQuantity", typeof(int), typeof(DataGridItemTemplate));
        public int ShippedQuantity
        {
            get { return (int)GetValue(ShippedQuantityProperty); }
            set { SetValue(ShippedQuantityProperty, value); }
        }

        public static readonly DependencyProperty OnTheWayQuantityProperty = DependencyProperty.Register("OnTheWayQuantity", typeof(int), typeof(DataGridItemTemplate));
        public int OnTheWayQuantity
        {
            get { return (int)GetValue(OnTheWayQuantityProperty); }
            set { SetValue(OnTheWayQuantityProperty, value); }
        }

        public static readonly DependencyProperty ReorderQuantityProperty = DependencyProperty.Register("ReorderQuantity", typeof(decimal), typeof(DataGridItemTemplate));
        public decimal ReorderQuantity
        {
            get { return (decimal)GetValue(ReorderQuantityProperty); }
            set { SetValue(ReorderQuantityProperty, value); }
        }

        public static readonly DependencyProperty SalesQuantityProperty = DependencyProperty.Register("SalesQuantity", typeof(decimal), typeof(DataGridItemTemplate));
        public decimal SalesQuantity
        {
            get { return (decimal)GetValue(SalesQuantityProperty); }
            set { SetValue(SalesQuantityProperty, value); }
        }

        public static readonly DependencyProperty MobileInventoryItemProperty = DependencyProperty.Register("MobileInventoryItem", typeof(MobileInventoryItem), typeof(DataGridItemTemplate));
        public MobileInventoryItem MobileInventoryItem
        {
            get { return (MobileInventoryItem)GetValue(MobileInventoryItemProperty); }
            set { SetValue(MobileInventoryItemProperty, value); }
        }

        public static readonly DependencyProperty NotesProperty = DependencyProperty.Register("Notes", typeof(string), typeof(DataGridItemTemplate));
        public string Notes
        {
            get { return (string)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }

        public static readonly DependencyProperty ShowNotesProperty = DependencyProperty.Register("ShowNotes", typeof(bool), typeof(DataGridItemTemplate));
        public bool ShowNotes
        {
            get { return (bool)GetValue(ShowNotesProperty); }
            set { SetValue(ShowNotesProperty, value); }
        }

        public static readonly DependencyProperty BottomAreaContentProperty = DependencyProperty.Register("BottomAreaContent", typeof(object), typeof(DataGridItemTemplate), new FrameworkPropertyMetadata(null));
        public object BottomAreaContent
        {
            get { return GetValue(BottomAreaContentProperty); }
            set { SetValue(BottomAreaContentProperty, value); }
        }

        public static readonly DependencyProperty ImageAreaContentProperty = DependencyProperty.Register("ImageAreaContent", typeof(object), typeof(DataGridItemTemplate), new FrameworkPropertyMetadata(null));
        public object ImageAreaContent
        {
            get { return GetValue(ImageAreaContentProperty); }
            set { SetValue(ImageAreaContentProperty, value); }
        }

        public DataGridItemTemplate()
        {
            InitializeComponent();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
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

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Replace Image";
            open.Multiselect = false;
            open.Filter = "Image File|*.png;*.jpg;*.jpeg";
            if (open.ShowDialog() == true)
            {
                Helpers.SaveImageFileAsJpg(open.FileName, ImageSource);
            }
        }
    }
}
