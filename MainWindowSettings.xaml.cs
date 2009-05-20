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
using myRequirements.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace myRequirements
{
    public partial class MainWindowSettings : UserControl
    {
        private WebServiceClient proxy;
        private myRequirements.ServiceReference1.Project selectedProject;
        private ObservableCollection<Project> dataset;
        private int deleteProjectID;

        public MainWindowSettings()
        {
            // Required to initialize variables
            InitializeComponent();

            proxy = new WebServiceClient();

            proxy.GetAllProjectsCompleted += new EventHandler<GetAllProjectsCompletedEventArgs>(proxy_GetAllProjectsCompleted);
            proxy.getStatisticsByProjectIDCompleted += new EventHandler<getStatisticsByProjectIDCompletedEventArgs>(proxy_getStatisticsByProjectIDCompleted);
            proxy.UpdateProjectCompleted += new EventHandler<UpdateProjectCompletedEventArgs>(proxy_UpdateProjectCompleted);
            proxy.SetActiveProjectCompleted += new EventHandler<SetActiveProjectCompletedEventArgs>(proxy_SetActiveProjectCompleted);
            questionInfoMessgeBox.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButotn_Click);
            questionInfoMessgeBox.cancelInfoMessageButton.Click += new RoutedEventHandler(cancelInfoMessageButton_Click);
            proxy.DeleteProjectCompleted += new EventHandler<DeleteProjectCompletedEventArgs>(proxy_DeleteProjectCompleted);
            proxy.CreateNewProjectCompleted += new EventHandler<CreateNewProjectCompletedEventArgs>(proxy_CreateNewProjectCompleted);


            questionInfoMessgeBox.Visibility = Visibility.Collapsed;
            saveMessage.Visibility = Visibility.Collapsed;
            updateProjects();
        }


        public void updateProjects()
        {
            ProgressBar.Visibility = Visibility.Visible;

            proxy.GetAllProjectsAsync(Session.Instance.SessionID);
        }

        void proxy_GetAllProjectsCompleted(object sender, GetAllProjectsCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    int selItem = leftList.SelectedIndex;
                    dataset = e.Result;
                    leftList.ItemsSource = dataset;

                    if (selItem == -1 && e.Result.Count > 0)
                        leftList.SelectedIndex = 0;
                    else
                        leftList.SelectedIndex = selItem;


                    if (dataset.Count == 0)
                        infoPanel.Visibility = Visibility.Collapsed;
                    else
                        infoPanel.Visibility = Visibility.Visible;


                    updateSelectedProject();

                }
                ProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void updateSelectedProject()
        {
            foreach (Project p in dataset)
            {
                if (p.isPrimaryProject == true)
                {
                    Data.SelectedProject.Instance.Project = p;
                    Data.SelectedProject.Instance.Name = p.Name; //Notify when name changes...
                }
            }
        }



        private void leftList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBox lb = sender as ListBox;
                selectedProject = lb.SelectedItem as Project;

                Name.DataContext = selectedProject;
                Description.DataContext = selectedProject;

                ProgressBar.Visibility = Visibility.Visible;

                numPubSecReq.Text = "";
                numSecReq.Text = "";
                numSecActivitiesSug.Text = "";

                proxy.getStatisticsByProjectIDAsync(Session.Instance.SessionID, selectedProject.ProjectID);
            }
            catch (Exception)
            {
            }
        }

        void proxy_getStatisticsByProjectIDCompleted(object sender, getStatisticsByProjectIDCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                numPubSecReq.Text = e.Result.numPubSeqReq + "";
                numSecReq.Text = e.Result.numSeqReq + "";
                numSecActivitiesSug.Text = e.Result.numSuggestedSeqActivities + "";
            }
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void SaveProject(object sender, RoutedEventArgs e)
        {

            saveMessage.Visibility = Visibility.Collapsed;


            if (selectedProject != null)
            {
                ProgressBar.Visibility = Visibility.Visible;
                proxy.UpdateProjectAsync(Session.Instance.SessionID, selectedProject);
            }
            else
            {
                saveMessage.Text = "Did not find any selected project";
                saveMessage.Visibility = Visibility.Visible;
            }

        }

        void proxy_UpdateProjectCompleted(object sender, UpdateProjectCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null)
                {
                    saveMessage.Text = "Something went wrong, please try again.";
                }
                else
                {
                    saveMessage.Text = "Saved successfully!";
                }
                saveMessage.Visibility = Visibility.Visible;
                updateProjects();

            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                saveMessage.Text = "Something went wrong, please try again.";
                saveMessage.Visibility = Visibility.Visible;
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBar.Visibility = Visibility.Visible;

                int selProjectID = Convert.ToInt32(((RadioButton)sender).Name);

                proxy.SetActiveProjectAsync(Session.Instance.SessionID, selProjectID);
            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }

        }

        void proxy_SetActiveProjectCompleted(object sender, SetActiveProjectCompletedEventArgs e)
        {
            updateProjects();
        }

        private void DeleteProject(object sender, RoutedEventArgs e)
        {

            deleteProjectID = Convert.ToInt32(((Button)sender).Tag);
            questionInfoMessgeBox.header.Text = "Are you sure?";
            questionInfoMessgeBox.message.Text = "Are you sure you want to delete this project?";

            questionInfoMessgeBox.Visibility = Visibility.Visible;
        }

        void cancelInfoMessageButton_Click(object sender, RoutedEventArgs e)
        {
            questionInfoMessgeBox.Visibility = Visibility.Collapsed;
        }

        void okInfoMessageButotn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (leftList.Items.Count == 1)
                    infoPanel.Visibility = Visibility.Collapsed;

                questionInfoMessgeBox.Visibility = Visibility.Collapsed;
                ProgressBar.Visibility = Visibility.Visible;

                foreach (Project p in dataset)
                {
                    if (p.ProjectID == deleteProjectID)
                    {
                        dataset.Remove(p);
                        if (p.isPrimaryProject == true)
                            Data.SelectedProject.Instance.Name = "";
                        break;
                    }
                }

                proxy.DeleteProjectAsync(Session.Instance.SessionID, deleteProjectID);
            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_DeleteProjectCompleted(object sender, DeleteProjectCompletedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void CreateNewProject(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            bool isPrimary;
            if (dataset.Count > 0)
                isPrimary = false;
            else
                isPrimary = true;

            myRequirements.ServiceReference1.Project newProject = new Project { Name = "New Project", isPrimaryProject = isPrimary };

            proxy.CreateNewProjectAsync(Session.Instance.SessionID, newProject);
        }

        void proxy_CreateNewProjectCompleted(object sender, CreateNewProjectCompletedEventArgs e)
        {
            updateProjects();
        }

    }
}