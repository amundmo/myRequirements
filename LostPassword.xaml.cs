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

namespace myRequirements
{
    public partial class LostPassword : UserControl
    {
        public LostPassword()
        {
            InitializeComponent();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            PageSwitcher ps = this.Parent as PageSwitcher;
            ps.Navigate(new LoginWindow());
        }

        private void LostPasswordEvent(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = System.Windows.Visibility.Visible;
            ServiceReference1.WebServiceClient proxy = new ServiceReference1.WebServiceClient();
            proxy.RecoverLostPasswordByEmailCompleted += new EventHandler<myRequirements.ServiceReference1.RecoverLostPasswordByEmailCompletedEventArgs>(proxy_RecoverLostPasswordByEmailCompleted);
            proxy.RecoverLostPasswordByEmailAsync(LostPasswordEmail.Text);
        }

        void proxy_RecoverLostPasswordByEmailCompleted(object sender, myRequirements.ServiceReference1.RecoverLostPasswordByEmailCompletedEventArgs e)
        {
            try
            {
                ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                InfoMessagePanel.Visibility = System.Windows.Visibility.Visible;

                if (e.Result == "Reset successfull!")
                {
                    InfoMessagePanel.header.Text = e.Result;
                    InfoMessagePanel.message.Text = "Press the 'Ok' button to go back to the main page...";

                    InfoMessagePanel.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButton_Click);
                }
                else
                {
                    InfoMessagePanel.header.Text = "Something went wrong!";
                    InfoMessagePanel.message.Text = e.Result;
                    InfoMessagePanel.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButton_Click2);
                }
            }
            catch (System.Reflection.TargetInvocationException)
            {
                InfoMessagePanel.header.Text = "Something went wrong!";
                InfoMessagePanel.message.Text = "Could not connect to server, please try again later...";
                InfoMessagePanel.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButton_Click2);
            }
            catch { }
        }


        private void okInfoMessageButton_Click(object sender, RoutedEventArgs e) //Go back to the login page
        {
            InfoMessagePanel.Visibility = System.Windows.Visibility.Collapsed;

            PageSwitcher ps = this.Parent as PageSwitcher;
            ps.Navigate(new LoginWindow());
        }

        private void okInfoMessageButton_Click2(object sender, RoutedEventArgs e) //Info message
        {
            InfoMessagePanel.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
