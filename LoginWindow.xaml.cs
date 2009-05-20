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
using myRequirements.Data;

namespace myRequirements
{
    public partial class LoginWindow : UserControl
    {
        private ServiceReference1.WebServiceClient proxy;
        private MainWindow mainWindow;
        public LoginWindow()
        {
            InitializeComponent();

            proxy = new ServiceReference1.WebServiceClient();

            proxy.getCurrentUserInformationCompleted += new EventHandler<myRequirements.ServiceReference1.getCurrentUserInformationCompletedEventArgs>(proxy_getCurrentUserInformationCompleted);
            proxy.LoginCompleted += new EventHandler<myRequirements.ServiceReference1.LoginCompletedEventArgs>(proxy_LoginCompleted);
        }

        private void LoginEvent(object sender, RoutedEventArgs e)
        {
            errorMessage.Visibility = System.Windows.Visibility.Collapsed;

            if (!usernameTextBox.Text.Equals("") && !passwordTextBox.Equals(""))
            {
                ProgressBarControl.Visibility = Visibility.Visible;
                proxy.LoginAsync(usernameTextBox.Text, passwordTextBox.Password);
            }
            else
                errorMessage.Visibility = Visibility.Visible;



        }

        void proxy_LoginCompleted(object sender, myRequirements.ServiceReference1.LoginCompletedEventArgs e)
        {
            try
            {
                ProgressBarControl.Visibility = Visibility.Collapsed;
                if (e.Result != null)
                {
                    Session.Instance.SessionID = e.Result; //Need to set the sessionid before initiating the mainwindow!
                    mainWindow = new MainWindow();
                    
                    proxy.getCurrentUserInformationAsync(e.Result);

                    PageSwitcher ps = this.Parent as PageSwitcher;
                    ps.Navigate(mainWindow);

                }
                else
                {
                    errorMessage.Text = "Wrong username or password!";
                    errorMessage.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch (System.Reflection.TargetInvocationException)
            {
                errorMessage.Text = "Could not connect to server.";
                errorMessage.Visibility = System.Windows.Visibility.Visible;
            }
            catch { }


        }

        void proxy_getCurrentUserInformationCompleted(object sender, myRequirements.ServiceReference1.getCurrentUserInformationCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Session.Instance.Userdata = e.Result as ServiceReference1.User;
                //mainWindow.setUserData(Session.Instance.Userdata.Fullname);
            }
        }

        private void LostPasswordEvent(object sender, RoutedEventArgs e)
        {
            PageSwitcher ps = this.Parent as PageSwitcher;
            ps.Navigate(new LostPassword());
        }

        private void RegisterNewUserEvent(object sender, RoutedEventArgs e)
        {
            PageSwitcher ps = this.Parent as PageSwitcher;
            ps.Navigate(new RegisterNewUser());
        }
    }
}
