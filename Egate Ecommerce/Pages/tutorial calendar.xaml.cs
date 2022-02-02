using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects.Calendar;
using Egate_Ecommerce.Objects.Tutorials;
using bolt5.ModalWpf;
using bolt5.CustomMonthlyCalendar;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for tutorial_calendar.xaml
    /// </summary>
    public partial class tutorial_calendar : Page
    {
        public static readonly RoutedEvent SelectJobScheduleEvent = EventManager.RegisterRoutedEvent(nameof(SelectJobSchedule), RoutingStrategy.Bubble, typeof(SelectJobScheduleEventHandler), typeof(tutorial_calendar));
        public event SelectJobScheduleEventHandler SelectJobSchedule
        {
            add { AddHandler(SelectJobScheduleEvent, value); }
            remove { RemoveHandler(SelectJobScheduleEvent, value); }
        }

        private ICollectionView periodItemList;
        private ICollectionView employeeItemList;
        private List<TutorialVideoViewModel> videoList = new List<TutorialVideoViewModel>();
        private List<PeriodCalendarDisplayCollection<TutorialVideoViewModel>> periodList = new List<PeriodCalendarDisplayCollection<TutorialVideoViewModel>>();
        private List<TutorialEmployeeViewModel> employeeList = new List<TutorialEmployeeViewModel>();

        private TutorialEmployeeViewModel selectedEmployee = null;

        public tutorial_calendar()
        {
            InitializeComponent();
            periodItemList = new CollectionViewSource() { Source = periodList }.View;
            calendar.ItemsSource = periodItemList;

            employeeItemList = new CollectionViewSource() { Source = employeeList }.View;
            employeeItemList.SortDescriptions.Add(new SortDescription("EmployeeName", ListSortDirection.Ascending));
            employeeDataGrid.ItemsSource = employeeItemList;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var videoTask = TutorialsHelper.GetVideoList();
            var employeeTask = TutorialsHelper.GetEmployeeList();
            Task.WaitAll(videoTask, employeeTask);

            videoList.Clear();
            videoList.AddRange(videoTask.Result.Where(i => !i.IsPeriodNotApplicable && i.PeriodType != TutorialPeriodType.None));

            employeeList.Clear();
            employeeList.AddRange(employeeTask.Result);
            employeeItemList.Refresh();

            //get video count for employee
            employeeList.ForEach(i => i.VideoCount = videoList.Count(v => i.Id == v.EmployeeAssignedTo?.Id));

            RefreshPeriodCalendarDisplay();
        }

        private void calendar_DisplayMonthChanged(object sender, EventArgs e)
        {
            RefreshPeriodCalendarDisplay();
        }

        private void RefreshPeriodCalendarDisplay()
        {
            periodList.Clear();
            if (selectedEmployee != null)
            {
                var list = videoList.Where(i => i.EmployeeAssignedTo.Id == selectedEmployee.Id);
                var periodCollection = PeriodCalendarHelper.GetPeriodListByDisplayMonth(list, calendar.DisplayMonth.Year, calendar.DisplayMonth.Month);
                periodList.AddRange(periodCollection);
            }
            periodItemList.Refresh();
        }

        private void calendar_DayClick(object sender, DayClickEventArgs e)
        {
            var btn = e.OriginalSource as MonthlyCalendarDayButton;
            if (btn.DataContext != null)
            {
                schedulePopup.DataContext = btn.DataContext;
                schedulePopup.PlacementTarget = btn;
                schedulePopup.IsOpen = true;
            }
        }

        private void PrintSchedule_btn_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            PeriodCalendarDisplayCollection<TutorialVideoViewModel> periodCollection = (sender as FrameworkElement).DataContext as PeriodCalendarDisplayCollection<TutorialVideoViewModel>;
            string xml = web_print.WebPrintHelper.GenerateXmlForScheduleList(periodCollection.Items.Select(i => i.Item));
            string file = web_print.WebPrintHelper.CreatePathForPrintableXmlWithStyleSheet(xml, "schedule");
            web_print.WebPrintHelper.Print(file, "Print Schedule");
        }

        private void AddEmployee_btn_Click(object sender, RoutedEventArgs e)
        {
            var employeeVm = new TutorialEmployeeViewModel();
            var modal = new Modals.tutorialEmployeeAddEdit_modal();
            modal.DataContext = employeeVm;
            if (ModalForm.ShowModal(modal, "Add Employee", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                _ = TutorialsHelper.AddUpdateEmployee(employeeVm);
                employeeList.Add(employeeVm);
                employeeItemList.Refresh();
            }
        }

        private void EditEmployee_btn_Click(object sender, RoutedEventArgs e)
        {
            var employeeVm = (sender as FrameworkElement).DataContext as TutorialEmployeeViewModel;
            var modal = new Modals.tutorialEmployeeAddEdit_modal();
            var clone = employeeVm.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Edit Employee", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(employeeVm);
                _ = TutorialsHelper.AddUpdateEmployee(employeeVm);
                employeeItemList.Refresh();
            }
        }

        private void EmployeeDataGrid_row_Selected(object sender, RoutedEventArgs e)
        {
            selectedEmployee = (sender as FrameworkElement).DataContext as TutorialEmployeeViewModel;
            RefreshPeriodCalendarDisplay();
        }

        private void ScheduleItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TutorialVideoViewModel video = ((sender as FrameworkElement).DataContext as PeriodCalendarDisplay<TutorialVideoViewModel>).Item;
            RaiseEvent(new SelectJobScheduleEventArgs(SelectJobScheduleEvent, video));
        }
    }

    public class SelectJobScheduleEventArgs : RoutedEventArgs
    {
        public TutorialVideoViewModel Video { get; private set; }

        public SelectJobScheduleEventArgs(RoutedEvent routedEvent, TutorialVideoViewModel video) : base(routedEvent)
        {
            this.Video = video;
        }
    }

    public delegate void SelectJobScheduleEventHandler(object sender, SelectJobScheduleEventArgs e);
}
