using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InputsApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        protected override async void OnStartup(StartupEventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

           
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.File("logs/log-.txt",
             outputTemplate: "----------{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            , rollingInterval: RollingInterval.Month)
            .CreateLogger();

            base.OnStartup(e);

        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                LogContext.PushProperty("ErrorMessage", exception.Message);
                Log.Error(exception, "An error occurred.");
                MessageBox.Show("Oops! Something went wrong. We're on it! Please restart the app or update if available. If the issue persists, contact support at afkometal@gmail.com. Thanks for your patience!");
                Log.CloseAndFlush();
                Environment.Exit(1);
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception exception = e.Exception;
            if (exception != null)
            {
                LogContext.PushProperty("ErrorMessage", exception.Message);
                Log.Error(exception, "An error occurred.");
                MessageBox.Show("Oops! Something went wrong. We're on it! Please restart the app or update if available. If the issue persists, contact support at afkometal@gmail.com. Thanks for your patience!");
                Log.CloseAndFlush();
                Environment.Exit(1);

            }
        }
    }
}
