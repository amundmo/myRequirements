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
using System.Collections.ObjectModel;
using myRequirements.Data;

namespace myRequirements
{
    public partial class MainWindow : UserControl
    {
        private MainWindowSearch mainWindowSearch;
        private MainWindowPrivateRequirements mainWindowPrivateRequirements;
        private MainWindowSettings mainWindowSettings;
        private AdminControl adminControl;

        public MainWindow()
        {

            mainWindowSearch = new MainWindowSearch();
            mainWindowPrivateRequirements = new MainWindowPrivateRequirements();
            mainWindowSettings = new MainWindowSettings();

            InitializeComponent();

            My_Requirements.Visibility = Visibility.Collapsed;
            AdminControl.Visibility = Visibility.Collapsed;

            Search.Content = mainWindowSearch;
            My_Requirements.Content = mainWindowPrivateRequirements;
            Settings.Content = mainWindowSettings;
         

            Data.SelectedProject.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Instance_PropertyChanged);
            Data.Session.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Instance_PropertyChanged2);
        }

        void Instance_PropertyChanged2(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Userdata"))
            {
                setUserData(Session.Instance.Userdata.Fullname);
            }
        }

        public void setUserData(String name)
        {
            LoggedInUsername.Text = name;

            if (Session.Instance.Userdata != null &&
        Session.Instance.Userdata.Userlevel >= 2)
            {
                adminControl = new AdminControl();
                AdminControl.Content = adminControl;
                AdminControl.Visibility = Visibility.Visible;
            }
        }

        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (SelectedProject.Instance.Project == null || String.IsNullOrEmpty(SelectedProject.Instance.Name))
            {
                My_Requirements.Visibility = Visibility.Collapsed;
            }
            else
            {
                My_Requirements.Header = SelectedProject.Instance.Project.Name.Trim();
                My_Requirements.Visibility = Visibility.Visible;
                updateSearchData();
            }
            
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            PageSwitcher ps = this.Parent as PageSwitcher;
            ps.Navigate(new LoginWindow());
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl cntr = sender as TabControl;
            TabItem tabItem = cntr.SelectedItem as TabItem;

            if (tabItem.Name == "Search")
            {
                //mainWindowSearch.getAllSearchData();
            }
            else if (tabItem.Name == "My_Requirements")
            {
                if(mainWindowPrivateRequirements != null)
                    mainWindowPrivateRequirements.updateLeftList();
            }
            else if (tabItem.Name == "Settings")
            {
                mainWindowSettings.updateProjects();
            }
            else if (tabItem.Name == "AdminControl")
            {

            }
        }

         public void updateSearchData(){

            mainWindowSearch.getAllSearchData();
        }
    }
}


   

        //private void mySecurityRequirementListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //ListBox lb = (ListBox)sender;
            
        //    //if(selectedListBox != null && !selectedListBox.Equals(lb)){
        //    //    selectedListBox.SelectedIndex = -1;
        //    //}
        //    //selectedListBox = lb;


        //    //selectedSecurityRequirement = (SecurityRequirement)lb.SelectedItem;

        //    //if (selectedSecurityRequirement != null)
        //    //{
        //    //    requirementName.DataContext = selectedSecurityRequirement;
        //    //    requirementAbstract.DataContext = selectedSecurityRequirement;
        //    //    requirementInfoExamples.ItemsSource = selectedSecurityRequirement.Examples;

        //    //    requirementEditName.DataContext = selectedSecurityRequirement;
        //    //    requirementEditAbstract.DataContext = selectedSecurityRequirement;
        //    //    requirementInfoExamples1.ItemsSource = selectedSecurityRequirement.Examples;

        //    //}


        //    //if (requirementInfoGrid.Visibility == System.Windows.Visibility.Collapsed &&
        //    //    requirementEditInfoGrid.Visibility == System.Windows.Visibility.Collapsed)
        //    //{
        //    //    requirementInfoGrid.Visibility = System.Windows.Visibility.Visible;
        //    //}

            
        //}

        //private void requirementInfoExamples_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //ListBox lb = (ListBox)sender;
        //    //String selectedExample = (String)lb.SelectedItem;
        //    //Boolean namealreadyAdded = false;

        //    //if (selectedExample != null)
        //    //{

        //    //    foreach (String example in selectedSecurityRequirement.Examples)
        //    //    {
        //    //        if (example.Equals(selectedSecurityRequirement.Name))
        //    //        {
        //    //            namealreadyAdded = true;
        //    //        }

        //    //    }
        //    //    if (!namealreadyAdded)
        //    //        selectedSecurityRequirement.Examples.Add(selectedSecurityRequirement.Name);

        //    //    selectedSecurityRequirement.Name = selectedExample;
        //    //}

        //}


        //private void editPrivateRequirementEvent(object sender, RoutedEventArgs e)
        //{          
        //    requirementInfoGrid.Visibility = System.Windows.Visibility.Collapsed;
        //    requirementEditInfoGrid.Visibility = System.Windows.Visibility.Visible;
            
        //}

        //private void doneEditPrivateSecurityRequirement(object sender, RoutedEventArgs e)
        //{
        //    requirementEditInfoGrid.Visibility = System.Windows.Visibility.Collapsed;
        //    requirementInfoGrid.Visibility = System.Windows.Visibility.Visible;
        //}


        //private void ListBoxExpander_Click(object sender, RoutedEventArgs e)
        //{
        //    Button b1 = sender as Button;
        //    Grid g1 = b1.Parent as Grid;
        //    ListBox l1 = g1.FindName("PrivateRequirementListBoxChild") as ListBox;


        //    if (l1.Visibility == Visibility.Visible)
        //        l1.Visibility = Visibility.Collapsed;
        //    else
        //        l1.Visibility = Visibility.Visible;



        //    //Populate list...

        //    //This is wrong! I'm populating the wrong list...
        //    SearchResults searchResults = new SearchResults();

        //    SecurityRequirement secReq1 = new SecurityRequirement();
        //    SecurityRequirement secReq2 = new SecurityRequirement();

        //    secReq1.Name = "The need-to-know principle can be enforced with user access controls and authorisation procedures.";
        //    secReq1.AbstractText = "User access and authorisation provides protection against unauthorised access through user identification, authorisation and restricting access to information and functions needed by staff members to undertake their duties.";

        //    secReq1.Examples.Add("Another example 1");
        //    secReq1.Examples.Add("Another example 2 - with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text");

        //    secReq2.Name = "Access to an agency’s system can be controlled by insisting on an authentication procedure to establish, with some minimum degree of confidence, the identity of the system user.";
        //    secReq2.AbstractText = "Implementing password selection policies and password management practices prevents system users and systems from having authentication information easily subverted by brute force attacks against weak authenticators.";

        //    secReq2.Examples.Add("Another example 1");
        //    secReq2.Examples.Add("Another example 2 - with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text");

        //    //searchResult.privateSecurityRequirements = new ObservableCollection<SecurityRequirement>();
        //    searchResults.Results = new ObservableCollection<ISearchableObject>();
        //    searchResults.Results.Add(secReq1);
        //    searchResults.Results.Add(secReq2);

        //    l1.ItemsSource = searchResults.Results;
         

        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog fDialog = new OpenFileDialog();
        //    fDialog.Multiselect = false;

        //    if ((bool)fDialog.ShowDialog())
        //    {
        //        //addSDPFileName.Text = fDialog.File.Name;

        //    }
        //}

