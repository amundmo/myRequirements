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
using myRequirements.ServiceReference1;

namespace myRequirements
{
    public partial class RegisterNewUser : UserControl
    {
        public RegisterNewUser()
        {
            InitializeComponent();
        }

        private void click_goBackToMainMenu(object sender, RoutedEventArgs e)
        {
            PageSwitcher ps = this.Parent as PageSwitcher;
            ps.Navigate(new LoginWindow());
        }

        private void registerNewUser(object sender, RoutedEventArgs e)
        {
            errorTextBlock.Text = "";
            Boolean foundError = false;
            VisualStateManager.GoToState(this, "VisualState", true);
            VisualStateManager.GoToState(this, "VisualState2", true);
            VisualStateManager.GoToState(this, "VisualState4", true);
            VisualStateManager.GoToState(this, "VisualState6", true);
            VisualStateManager.GoToState(this, "VisualState8", true);
            VisualStateManager.GoToState(this, "VisualState10", true);


            if (fullName.Text.Equals(""))
            {
                VisualStateManager.GoToState(this, "VisualState1", true);
                errorTextBlock.Text += "- You need to set Full name\n";
                foundError = true;
            }

            if (username.Text.Equals(""))
            {
                VisualStateManager.GoToState(this, "VisualState3", true);
                errorTextBlock.Text += "- You need to set Username\n";
                foundError = true;
            }

            if (email.Text.Equals(""))
            {
                VisualStateManager.GoToState(this, "VisualState5", true);
                errorTextBlock.Text += "- You need to set e-mail\n";
                foundError = true;
            }

            if (country.Text.Equals(""))
            {
                VisualStateManager.GoToState(this, "VisualState7", true);
                errorTextBlock.Text += "- You need to set country\n";
                foundError = true;
            }
            
            if (password.Password.Equals(""))
            {
                VisualStateManager.GoToState(this, "VisualState9", true);
                errorTextBlock.Text += "- You need to set password\n";
                foundError = true;
            }

            if (cpassword.Password.Equals(""))
            {
                VisualStateManager.GoToState(this, "VisualState9", true);
                errorTextBlock.Text += "- You need to confirm password\n";
                foundError = true;
            }
            else{
                if(!password.Password.Equals(cpassword.Password)){
                    VisualStateManager.GoToState(this, "VisualState9", true);
                    errorTextBlock.Text += "- The passwords do not match\n";
                    foundError = true;
                }

                if(password.Password.Length < 8){
                    VisualStateManager.GoToState(this, "VisualState9", true);
                    errorTextBlock.Text += "- The password needs to be at least 8 characters\n";
                    foundError = true;
                }
            }

            if (!foundError)
            {
                progressBar.Visibility = Visibility.Visible;
                User user = new User() { Fullname = fullName.Text, Username = username.Text, Password = password.Password, Email = email.Text ,Country = country.Text};
                ServiceReference1.WebServiceClient proxy = new ServiceReference1.WebServiceClient();

                proxy.CreateNewUserCompleted += new EventHandler<CreateNewUserCompletedEventArgs>(proxy_CreateNewUserCompleted);
                proxy.CreateNewUserAsync(user);
            }
        }

        void proxy_CreateNewUserCompleted(object sender, CreateNewUserCompletedEventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
            String headerText = "Something went wrong.";
            try
            {
                infoMessagePanelControl.Visibility = System.Windows.Visibility.Visible;
                if (e.Error == null && e.Result.Equals("Registration successfull")) //logged in ok
                {
                    infoMessagePanelControl.header.Text = "Registration successfull!";
                    infoMessagePanelControl.message.Text = "You have now successfully created a new user! Click the \"Ok\" button to go back to the main page";
                    infoMessagePanelControl.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButotn_Click);

                }
                else
                {
                    if (e.Result.Equals("Username already exsists."))
                    {
                        VisualStateManager.GoToState(this, "VisualState3", true);
                    }
                    else if (e.Result.Equals("Email is not valid."))
                    {
                        VisualStateManager.GoToState(this, "VisualState5", true);
                    }
                    else if (e.Result.Equals("Username or email already exsists."))
                    {
                        VisualStateManager.GoToState(this, "VisualState3", true);
                        VisualStateManager.GoToState(this, "VisualState5", true);
                    }

                    showErrorMessage(headerText, e.Result);
                }
            }
            catch (System.Reflection.TargetInvocationException)
            {
                showErrorMessage("Something went wrong!", "Could not connect to server, please try again later...");
            }
            catch (Exception)
            {
                showErrorMessage(headerText, e.Result);
            }
        }

        private void showErrorMessage(String headerText, String messageText)
        {
            infoMessagePanelControl.header.Text = headerText;
            infoMessagePanelControl.message.Text = messageText;
            infoMessagePanelControl.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButotn_Click2);
        }


     
        private void okInfoMessageButotn_Click(object sender, RoutedEventArgs e) //Go back to login page
        {
            infoMessagePanelControl.Visibility = System.Windows.Visibility.Collapsed;

            PageSwitcher ps = this.Parent as PageSwitcher;
            ps.Navigate(new LoginWindow());
        }

        private void okInfoMessageButotn_Click2(object sender, RoutedEventArgs e) //Just collaps message box
        {
            infoMessagePanelControl.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
