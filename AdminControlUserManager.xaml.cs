using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using myRequirements.ServiceReference1;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using myRequirements.Data;

namespace myRequirements
{
	public partial class AdminControlUserManager : UserControl
	{
        private ObservableCollection<User> changedUsers;
        private int delteUserID;
        private ServiceReference1.WebServiceClient proxy;

		public AdminControlUserManager()
		{
			// Required to initialize variables
            InitializeComponent();
            proxy = new ServiceReference1.WebServiceClient();
            changedUsers = new ObservableCollection<User>();
            updateUsersFromProxy();


            proxy.getAllUsersCompleted += new EventHandler<myRequirements.ServiceReference1.getAllUsersCompletedEventArgs>(proxy_getAllUsersCompleted);
            proxy.UpdateUsersCompleted += new EventHandler<myRequirements.ServiceReference1.UpdateUsersCompletedEventArgs>(proxy_UpdateUsersCompleted);
            InfoMessagePanel.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButtonErrorOnUpdateUser_Click);
            QuestionInfoMessageBox.okInfoMessageButotn.Click += new RoutedEventHandler(okMessageClickedDeleteUserOK);
            QuestionInfoMessageBox.cancelInfoMessageButton.Click += new RoutedEventHandler(cancelInfoMessageButton_Click);
            proxy.DeleteUserCompleted += new EventHandler<DeleteUserCompletedEventArgs>(proxy_DeleteUserCompleted);
            InfoMessagePanel.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButotn_Click2);
            proxy.OverrideLoginAsCompleted += new EventHandler<OverrideLoginAsCompletedEventArgs>(proxy_OverrideLoginAsCompleted);
            proxy.getCurrentUserInformationCompleted += new EventHandler<getCurrentUserInformationCompletedEventArgs>(proxy_getCurrentUserInformationCompleted);
            proxy.GetAllProjectsCompleted += new EventHandler<GetAllProjectsCompletedEventArgs>(proxy_GetAllProjectsCompleted);

        }

        public void updateUsersFromProxy()
        {

            ProgressBar.Visibility = Visibility.Visible;


            proxy.getAllUsersAsync(Session.Instance.SessionID);



        }

        void proxy_getAllUsersCompleted(object sender, myRequirements.ServiceReference1.getAllUsersCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                UserGrid.ItemsSource = e.Result;
                foreach (ServiceReference1.User user in e.Result)
                {
                    user.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(user_PropertyChanged);
                    user.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(user_PropertyChanged);
                }

            }
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        void user_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ServiceReference1.User changedUser = sender as ServiceReference1.User;

            if (!changedUsers.Contains(changedUser))
            {
                changedUsers.Add(sender as ServiceReference1.User);
                SaveUserButton.IsEnabled = true;
            }
            
        }

        private void saveUsersButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;

            proxy.UpdateUsersAsync(Session.Instance.SessionID, changedUsers);
        }

        void proxy_UpdateUsersCompleted(object sender, myRequirements.ServiceReference1.UpdateUsersCompletedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Collapsed;
            SaveUserButton.IsEnabled = false;
            changedUsers.Clear();

            if (e.Result != null && 
                !e.Result.Equals("Successfully changed users"))
            {
                updateUsersFromProxy();
                InfoMessagePanel.Visibility = Visibility.Visible;
                InfoMessagePanel.header.Text = "Something went wrong";
                InfoMessagePanel.message.Text = e.Result;
            }
        }

        void okInfoMessageButtonErrorOnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            InfoMessagePanel.Visibility = Visibility.Collapsed;
        }

        private void cancelSaveUsersButton_Click(object sender, RoutedEventArgs e)
        {
            changedUsers.Clear();
            updateUsersFromProxy();

        }

        private void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            delteUserID = Convert.ToInt32(button.Tag);
            QuestionInfoMessageBox.Visibility = Visibility.Visible;
            QuestionInfoMessageBox.header.Text = "Are you sure?";
            QuestionInfoMessageBox.message.Text = "Are you sure you want to delete this user?";

        }

        void cancelInfoMessageButton_Click(object sender, RoutedEventArgs e)
        {
            QuestionInfoMessageBox.Visibility = Visibility.Collapsed;
        }

        void okMessageClickedDeleteUserOK(object sender, RoutedEventArgs e)
        {
            QuestionInfoMessageBox.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Visible;
            proxy.DeleteUserAsync(Session.Instance.SessionID, delteUserID);
        }

        void proxy_DeleteUserCompleted(object sender, DeleteUserCompletedEventArgs e)
        {
            if (e.Result != "Deleted user successfully")
            {
                InfoMessagePanel.Visibility = Visibility.Visible;
                InfoMessagePanel.header.Text = "Something went wrong";
                InfoMessagePanel.message.Text = e.Result;
            }

            updateUsersFromProxy();
        }

        void okInfoMessageButotn_Click2(object sender, RoutedEventArgs e)
        {
            InfoMessagePanel.Visibility = Visibility.Collapsed;
        }

        private void LoginAsClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProgressBar.Visibility = Visibility.Visible;
            
            proxy.OverrideLoginAsAsync(Session.Instance.SessionID, Convert.ToInt32(b.Tag));
        }

        void proxy_OverrideLoginAsCompleted(object sender, OverrideLoginAsCompletedEventArgs e)
        {
            proxy.getCurrentUserInformationAsync(Session.Instance.SessionID);
        }

        void proxy_getCurrentUserInformationCompleted(object sender, getCurrentUserInformationCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Session.Instance.Userdata = e.Result as ServiceReference1.User;
            }

            proxy.GetAllProjectsAsync(Session.Instance.SessionID);

            
        }

       void proxy_GetAllProjectsCompleted(object sender, GetAllProjectsCompletedEventArgs e)
        {
            try
            {
                foreach (Project p in e.Result)
                {
                    if (p.isPrimaryProject == true)
                    {
                        Data.SelectedProject.Instance.Project = p;
                        Data.SelectedProject.Instance.Name = p.Name; //Notify when name changes...
                    }
                }
                ProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

	}
}