using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using purchase_request.Model;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Win32;
using System.IO;
using HtmlAgilityPack;
using System.Net;
using Egate_Ecommerce.web_print;

namespace Egate_Ecommerce
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICollectionView Items { get; set; }

        public test()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string xmlFile = @"C:\Users\Graphic Designer\Documents\Visual Studio 2019\Projects\purchase request\purchase request\web print\stylesheets\barcodes test.xml";
            string xml = File.ReadAllText(xmlFile);
            string file = WebPrintHelper.CreatePathForPrintableXmlWithStylesheetAndWebPagePack(xml, "barcodeLabel", "barcode label");
            WebPrintHelper.Print(file, "print barcode");
        }
    }
}
