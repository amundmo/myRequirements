using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace myRequirements
{
    public partial class App : Application
    {
        
        private ScrollViewer root;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            HtmlPage.Window.AttachEvent("DOMMouseScroll", OnMouseScroll); // firefox
            HtmlPage.Window.AttachEvent("onmousewheel", OnMouseScroll);
            HtmlPage.Document.AttachEvent("onmousewheel", OnMouseScroll); // ie

            InitializeComponent();
        }

        private void OnMouseScroll(object sender, HtmlEventArgs args)
        {
            double delta = 0;
            ScriptObject e = args.EventObject; // safari & firefox
            if (e.GetProperty("detail") != null)
            {
                delta = (((double)e.GetProperty("detail")) * -100) * 7;
            }
            else if (e.GetProperty("wheelDelta") != null) // ie && Opera
            {
                delta = ((double)e.GetProperty("wheelDelta")) * 7;
            }
            //if scroller is recognized update it
            if (root != null)
            {
                root.ScrollToVerticalOffset(root.VerticalOffset + delta * -1 * 0.1);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            root = new ScrollViewer();
            this.RootVisual = root;
            root.Background = new SolidColorBrush(Color.FromArgb(230, 230, 230, 255));

            root.Content = new PageSwitcher();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
