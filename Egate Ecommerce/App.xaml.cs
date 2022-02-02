using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Threading;
using System.Threading.Tasks;
using Egate_Ecommerce.Quickbooks;
using System.Windows.Controls;
using System.Threading;
using CefSharp;
using CefSharp.Wpf;

namespace Egate_Ecommerce
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (InstanceManager.IsInstanceAlreadyRunning())
            {
                Application.Current.Shutdown();
                return;
            }
            InitializeErrorHandling();
            //LoadTest();
            //return;

            //load splash screen
            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();
            var frame = new DispatcherFrame();
            var t = Task.Run(async () =>
            {
                await Task.Delay(50);
                {
                    InitializeDatabase();
                    InitializeQuickbooks();
                } //7 seconds
                frame.Continue = false;
            });
            Dispatcher.PushFrame(frame);
            t.Wait();

            //initialize cef settings
            CefSettings settings = new CefSettings();
            settings.RemoteDebuggingPort = 8080;
            Cef.Initialize(settings);

            Window main = new MainWindow();
            main.Title = "E-Commerce";
            main.Show();
            splashScreen.Close();
        }

        private void LoadTest()
        {
            Window main = new test();
            main.Title = "E-Commerce";
            main.Show();
        }

        private void InitializeQuickbooks()
        {
            QbPosInventory.Load();
            QbPosMonthlySales.Load();
        }

        private void InitializeDatabase()
        {
            List<Task> tasks = new List<Task>();
            CancellationTokenSource cancelToken = new CancellationTokenSource();

            tasks.Add(InitializeDatabaseAsync<purchase_request.Model.PurchaseRequestModel>(ctx => ctx.Initialize()));
            tasks.Add(InitializeDatabaseAsync<Tutorials.Model.TutorialsModel>(ctx => ctx.Initialize()));
            tasks.Add(InitializeDatabaseAsync<ECommerce.Model.ECommerceModel>(ctx => ctx.Initialize()));
            
            try
            {
                Task.WaitAll(tasks.ToArray(), cancelToken.Token);
            }
            catch (Exception ex)
            {
                cancelToken.Cancel();
                Logs.WriteExceptionLogs(ex);
                MessageBox.Show("Cannot connect to database. This application will now close.");
                Environment.Exit(0);
            }
        }

        private static Task InitializeDatabaseAsync<T>(Action<T> initialize) where T : System.Data.Entity.DbContext, new()
        {
            return Task.Run(() =>
            {
                using (var context = new T())
                {
                    initialize?.Invoke(context);
                }
            });
        }

        private void InitializeErrorHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logs.WriteExceptionLogs(e.Exception);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logs.WriteExceptionLogs(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logs.WriteExceptionLogs(e.ExceptionObject as Exception);
        }

        public static T GetResource<T>(object key)
        {
            return (T)Application.Current.TryFindResource(key);
        }

        public static T GetResource<T>(object key, T fallback)
        {
            var resource = Application.Current.TryFindResource(key);
            if (resource == null)
                resource = fallback;
            return (T)resource;
        }
    }
}
