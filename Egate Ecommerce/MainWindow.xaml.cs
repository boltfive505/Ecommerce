using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Egate_Ecommerce
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty PageTabNameProperty = DependencyProperty.RegisterAttached("PageTabName", typeof(PageTab), typeof(MainWindow), new PropertyMetadata(PageTab.None));
        public static readonly RoutedEvent ChangeTabEvent = EventManager.RegisterRoutedEvent("ChangeTab", RoutingStrategy.Bubble, typeof(PageTabEventHandler), typeof(MainWindow));

        public static PageTab GetPageTabName(DependencyObject obj)
        {
            return (PageTab)obj.GetValue(PageTabNameProperty);
        }

        public static void SetPageTabName(DependencyObject obj, PageTab value)
        {
            obj.SetValue(PageTabNameProperty, value);
        }

        public static void AddChangeTabHandler(DependencyObject obj, PageTabEventHandler handler)
        {
            UIElement element = obj as UIElement;
            element.AddHandler(MainWindow.ChangeTabEvent, handler);
        }

        public static void RemoveChangeTabHandler(DependencyObject obj, PageTabEventHandler handler)
        {
            UIElement element = obj as UIElement;
            element.RemoveHandler(MainWindow.ChangeTabEvent, handler);
        }

        public MainWindow()
        {
            InstanceManager.AddWindowInstanceHook(this);
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public static void ChangeTab(object sender, PageTab pageTab)
        {
            UIElement element = sender as UIElement;
            if (element != null)
                element.RaiseEvent(new PageTabEventArgs(MainWindow.ChangeTabEvent, pageTab));
        }

        private void TabControl_ChangeTab(object sender, PageTabEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            object tabPage = tabControl.Items.OfType<TabItem>().FirstOrDefault(i => MainWindow.GetPageTabName(i) == e.PageTab);
            if (tabPage != null)
                tabControl.SelectedItem = tabPage;
        }
    }

    public class PageTabEventArgs : RoutedEventArgs
    {
        public PageTab PageTab { get; set; }

        public PageTabEventArgs(RoutedEvent routedEvent, PageTab pageTab) : base(routedEvent)
        {
            this.PageTab = pageTab;
        }
    }

    public delegate void PageTabEventHandler(object sender, PageTabEventArgs e);

    public enum PageTab
    {
        None,
        NonInventory,
        CreateRequest,
        ProcessOrder,
        StockPreparation,
        Packing,
        ShippingInformation,
        ReceivingDelivery,
        ReceivedItems,
        LuckyMain,
        LuckyExcel,
        ShipList,
        Barcodes,
        Tutorials,
        Calendar
    }
}
