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
using System.Linq;


namespace myRequirements
{
	public partial class MainWindowPrivateRequirements : UserControl
	{
        private WebServiceClient proxy;
        private RequirementsAndPatternsAndCategoriesAndPrivRequirements dataSet;
        private List<Data.SearchableData> dataSetSearchable;
        private Data.SearchableData selectedRequirement;
        private ListBox selectedListBox;

        private String heigh = "Heigh";
        private String low = "Low";
        private String medium = "Medium";
        List<String> risks;
        private bool isOdd = false;

       
		public MainWindowPrivateRequirements()
		{
			// Required to initialize variables
			InitializeComponent();

            proxy = new WebServiceClient();

            proxy.getAllDataCompleted += new EventHandler<getAllDataCompletedEventArgs>(proxy_getAllDataCompleted);
            proxy.UpdatePSRPCompleted += new EventHandler<UpdatePSRPCompletedEventArgs>(proxy_UpdatePSRPCompleted);
            proxy.createPrivateRequirementByRequirementIDCompleted += new EventHandler<createPrivateRequirementByRequirementIDCompletedEventArgs>(proxy_createPrivateRequirementByRequirementIDCompleted);
            proxy.UpdatePSRPCompleted += new EventHandler<UpdatePSRPCompletedEventArgs>(proxy_UpdatePSRPCompleted2);
            proxy.toggleActiveRequirementCompleted += new EventHandler<toggleActiveRequirementCompletedEventArgs>(proxy_toggleActiveRequirementCompleted);
            proxy.toggleActivePrivateRequirementCompleted += new EventHandler<toggleActivePrivateRequirementCompletedEventArgs>(proxy_toggleActivePrivateRequirementCompleted);
            proxy.CreateNewPSRPCompleted += new EventHandler<CreateNewPSRPCompletedEventArgs>(proxy_CreateNewPSRPCompleted);

            risks = new List<String>();
            risks.Add(heigh);
            risks.Add(medium);
            risks.Add(low);

            ProgressBar.Visibility = Visibility.Collapsed;
            HideInfoPanels();
		}

        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            updateLeftList();
        }

        private void HideInfoPanels()
        {
            PrivateSecurityRequirement.Visibility = Visibility.Collapsed;
            SecurityRequirement.Visibility = Visibility.Collapsed;
            SecurityPatterns.Visibility = Visibility.Collapsed;
            PrivateSecurityRequirementEdit.Visibility = Visibility.Collapsed;
        }


        public void updateLeftList()
        {
            ProgressBar.Visibility = Visibility.Visible;

            proxy.getAllDataAsync(Session.Instance.SessionID);
        }

        void proxy_getAllDataCompleted(object sender, getAllDataCompletedEventArgs e)
        {
            try{
                dataSet = e.Result as RequirementsAndPatternsAndCategoriesAndPrivRequirements;

                //update requirement list
                int selItem = leftList.SelectedIndex;
                
                dataSetSearchable = populatePrivateRequirements();

                dataSetSearchable.Sort(delegate(Data.SearchableData d1, Data.SearchableData d2) { return d1.Priority.CompareTo(d2.Priority); });


                leftList.ItemsSource = dataSetSearchable;

                if (selItem == -1 && dataSetSearchable.Count > 0)
                    leftList.SelectedIndex = 0;
                else
                    leftList.SelectedIndex = selItem;

                ProgressBar.Visibility = Visibility.Collapsed;
            }
            catch(Exception){
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }



        private List<Data.SearchableData> populatePrivateRequirements()
        {
            try
            {
                var preq = (from r in dataSet.PrivRequirements
                            from rel in dataSet.RelationProjectPrivateSecurityRequirement
                            where rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID &&
                            rel.PrivateRequirementID == r.Requirement.PrivateRequirementsID
                            select r.Requirement).ToList<PrivateRequirement>();

                
                //Add data that does not exist in the private requirement...
                List<Data.SearchableData> data = new List<Data.SearchableData>();
                foreach (PrivateRequirement pr in preq)
                {
                    Data.SearchableData newData = new Data.SearchableData();

                    //Because of the cast, we need to check if the value is null.
                    if (pr.SelectedExample != null)
                        newData.selectedExample = (int)pr.SelectedExample;
                    if (pr.Priority != null)
                        newData.Priority = (int)pr.Priority;

                    if (pr.RequirementID != null)
                    {
                        var req = (from r in dataSet.Requirements
                                   where r.Requirement.RequirementID == pr.RequirementID
                                   select r.Requirement).FirstOrDefault<Requirement>();
                        newData.ObjectType = (int)Data.SearchableData.types.requirement;

                        newData.ID = req.RequirementID;
                        newData.Name = req.Name;
                        newData.Aliases = req.Aliases;
                        newData.Description = req.Description;
                        newData.CommonAttacks = req.CommonAttacks;
                        newData.Issues = req.Issues;
                        newData.Problem = req.Problem;
                        newData.Solution = req.Solution;
                        newData.References = req.References;
                        newData.Domain = req.Domain;
                        newData.Examples = req.Examples;
                        newData.SuggestAsPublic = false;
                    }
                    else
                    {
                        //Populate newData
                        newData.ID = pr.PrivateRequirementsID;
                        newData.Name = pr.Name;
                        newData.Problem = pr.Problem;
                        newData.Solution = pr.Solution;
                        newData.Aliases = pr.Aliases;
                        newData.Description = pr.Description;
                        newData.CommonAttacks = pr.CommonAttacks;
                        newData.Issues = pr.Issues;
                        newData.References = pr.References;
                        newData.Domain = pr.Domain;
                        newData.Examples = pr.Examples;
                        newData.ObjectType = (int)Data.SearchableData.types.privateRequirement;
                        
                        if(pr.SuggestAsPublic != null)
                            newData.SuggestAsPublic = (bool)pr.SuggestAsPublic; 
                    }

                    Example exampleText = null;
                    if (newData.Examples != null) //Get text for exampleText(To show in leftList)
                    {
                        exampleText = (from example in newData.Examples
                                       where example.ExampleID == pr.SelectedExample
                                       select example).FirstOrDefault<Example>();
                    }
                    else
                    {
                        newData.Examples = new ObservableCollection<Example>();
                    }

                    if (exampleText != null)
                    {
                        newData.SelectedRequirementText = exampleText.Name +" "+ exampleText.ExampleID;
                    }
                    else
                    {
                        newData.SelectedRequirementText = "No selected example";
                    }

                    if (newData.Priority == 1)
                        newData.PriorityText = "High";
                    else if (newData.Priority == 2)
                        newData.PriorityText = "Medium";
                    else if (newData.Priority == 3)
                        newData.PriorityText = "Low";
                    else
                        newData.PriorityText = "Not set";


                    data.Add(newData);
                }
                return data;
            }
            catch (Exception)
            {
                return null;
            }
           
          
        }

        private void ToggleSecurityRequirement(object sender, RoutedEventArgs e)
        {
            try
            {
                Button b1 = sender as Button;

                Grid g1 = b1.Parent as Grid;
                ListBox l1 = g1.FindName("ListBoxChild") as ListBox;


                if (b1.Content.Equals("+"))
                {
                    List<Pattern> reqPatterns = getRelatedItemsByRequirementID(Convert.ToInt32(b1.Tag));
                    l1.ItemsSource = reqPatterns;

                }

                ChangeButtonStatus(b1, l1);
            }
            catch (Exception) { }
        }

        private List<Pattern> getRelatedItemsByRequirementID(int parentID)
        {
            List<Pattern> reqPatterns = null;
            if (dataSetSearchable != null)
            {
                var searchObject = (from reqs in dataSetSearchable
                                    where reqs.ID == parentID
                                    select reqs).FirstOrDefault<Data.SearchableData>();

                if (searchObject.ObjectType == (int)Data.SearchableData.types.requirement)
                {
                    reqPatterns = (from p in dataSet.Patterns
                                   from pr in dataSet.RelationPatternRequirement
                                   where pr.RequirementID == parentID &&
                                   pr.PatternID == p.PatternID
                                   select p).ToList<Pattern>();
                }
                else if (searchObject.ObjectType == (int)Data.SearchableData.types.privateRequirement)
                {
                    reqPatterns = (from p in dataSet.Patterns
                                   from pr in dataSet.RelationPatternPrivateSecurityRequirement
                                   where pr.PrivateRequirementID == parentID &&
                                   pr.PatternID == p.PatternID
                                   select p).ToList<Pattern>();
                }
            }
            return reqPatterns;
        }

        private void ToggleDesignPattern(object sender, RoutedEventArgs e)
        {
            try
            {
                Button b1 = sender as Button;

                Grid g1 = b1.Parent as Grid;
                ListBox l1 = g1.FindName("ListBoxChildChild") as ListBox;


                if (b1.Content.Equals("+"))
                {
                    var req = (from r in dataSet.Requirements
                               from pr in dataSet.RelationPatternRequirement
                               where r.Requirement.RequirementID == pr.RequirementID &&
                               pr.PatternID == Convert.ToInt32(b1.Tag)
                               select r.Requirement).ToList<Requirement>();

                    l1.ItemsSource = req;

                }

                ChangeButtonStatus(b1, l1);
            }
            catch (Exception) { }
        }

        private static void ChangeButtonStatus(Button b1, ListBox l1)
        {

            if (l1.Visibility == Visibility.Visible)
            {
                l1.Visibility = Visibility.Collapsed;
                b1.Content = "+";
            }
            else
            {

                l1.Visibility = Visibility.Visible;
                b1.Content = "-";
            }
        }

        private void leftList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try 
	        {
                if (e.AddedItems.Count > 0)  //Only when focus is gained, not lost.
                {
                    ListBox lb = sender as ListBox;
                    selectedRequirement = lb.SelectedItem as Data.SearchableData;

                    requirementID.DataContext = selectedRequirement;
                    RequirementName.DataContext = selectedRequirement;
                    RequirementAliases.DataContext = selectedRequirement;
                    RequirementDescription.DataContext = selectedRequirement;
                    RequirementProblem.DataContext = selectedRequirement;
                    RequirementSolution.DataContext = selectedRequirement;
                    RequirementPriority.DataContext = selectedRequirement;
                    References_PSRP.DataContext = selectedRequirement;
                    Domain_PSRP.DataContext = selectedRequirement;
                    suggestAsPublicCheckBox.DataContext = selectedRequirement;

                    foreach (Example ex in selectedRequirement.Examples)
                    {
                        ex.Name = ex.Name + " " + ex.ExampleID;
                    }

                    RequirementExamples.ItemsSource = selectedRequirement.Examples;



                    if (selectedListBox != null && !selectedListBox.Equals(lb))
                    {
                        selectedListBox.SelectedIndex = -1;
                    }
                    selectedListBox = lb;

                    HideInfoPanels();
                    PrivateSecurityRequirement.Visibility = Visibility.Visible;
                }
	        }
	        catch (Exception)
	        {
	        }
        }

        private void requirementInfoExamples_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            Example ex = lb.SelectedItem as Example;

            //Update database with id. 
            if (ex != null && ex.ExampleID != selectedRequirement.selectedExample)
            {
                PrivateRequirment_PatternsAndCategoriesAndExamples privRequirementChell = new PrivateRequirment_PatternsAndCategoriesAndExamples();
                if (selectedRequirement.ObjectType == (int)Data.SearchableData.types.privateRequirement)
                {
                    privRequirementChell.Requirement = (from pr in dataSet.PrivRequirements
                                                        where pr.Requirement.PrivateRequirementsID == selectedRequirement.ID
                                                        select pr.Requirement).SingleOrDefault<PrivateRequirement>();

                }
                else if (selectedRequirement.ObjectType == (int)Data.SearchableData.types.requirement)
                {
                    privRequirementChell.Requirement = (from pr in dataSet.PrivRequirements
                                                        from rel in dataSet.RelationProjectPrivateSecurityRequirement
                                                        where pr.Requirement.RequirementID == selectedRequirement.ID &&
                                                        rel.PrivateRequirementID == pr.Requirement.PrivateRequirementsID &&
                                                        rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID
                                                        select pr.Requirement).FirstOrDefault<PrivateRequirement>();
                }

                privRequirementChell.Requirement.SelectedExample = ex.ExampleID;

                proxy.UpdatePSRPAsync(Session.Instance.SessionID, privRequirementChell);

                selectedRequirement.SelectedRequirementText = ex.Name;
                selectedRequirement.selectedExample = ex.ExampleID;
            }
        }

        void proxy_UpdatePSRPCompleted(object sender, UpdatePSRPCompletedEventArgs e)
        {
            //ok
        }

        private void RequiremenListItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (selectedRequirement != null)
            {
                TextBlock tb = sender as TextBlock;
                Example ex = tb.Tag as Example;
                if (ex.ExampleID == selectedRequirement.selectedExample)
                    RequirementExamples.SelectedItem = ex;
            }

            TextBlock tb1 = sender as TextBlock;
            Border b = tb1.Parent as Border;

            if (isOdd)
                b.Background = lightGrey;

            isOdd = !isOdd;

        }

        private void ListBoxChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) //Only when focus is gained, not lost.
            {
                ListBox lb = sender as ListBox;
                Pattern pat = lb.SelectedItem as Pattern;

                HideInfoPanels();
                SecurityPatterns.Visibility = Visibility.Visible;

                DesignID.DataContext = pat;
                DesignName.DataContext = pat;
                DesignProblem.DataContext = pat;
                DesignSolution.DataContext = pat;
                DesignDescription.DataContext = pat;
                DesignAliases.DataContext = pat;
                References_SDP.DataContext = pat;
                Domains_SDP.DataContext = pat;



                if (selectedListBox != null && !selectedListBox.Equals(lb))
                {
                    selectedListBox.SelectedIndex = -1;
                }
                selectedListBox = lb;
            }
        }

        private void ListBoxChildChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) //Only when focus is gained, not lost.
            {
                ListBox lb = sender as ListBox;
                Requirement req = lb.SelectedItem as Requirement;

                //Hide other windows
                HideInfoPanels();
                //Show req window
                SecurityRequirement.Visibility = Visibility.Visible;

                requirementID1.DataContext = req;
                RequirementName1.DataContext = req;
                RequirementProblem1.DataContext = req;
                RequirementSolution1.DataContext = req;
                RequirementDescription1.DataContext = req;
                RequirementAliases1.DataContext = req;
                References_SRP.DataContext = req;
                Domains_SRP.DataContext = req;

                RequirementExamples1.ItemsSource = req.Examples;

                if (selectedListBox != null && !selectedListBox.Equals(lb))
                {
                    selectedListBox.SelectedIndex = -1;
                }
                selectedListBox = lb;
            }
        }

        private void EditPrivateRequirement(object sender, RoutedEventArgs e)
        {
            PrivateSecurityRequirement.Visibility = Visibility.Collapsed;
            PrivateSecurityRequirementEdit.Visibility = Visibility.Visible;


            requirementID2.DataContext = selectedRequirement;
            RequirementName2.DataContext = selectedRequirement;
            RequirementProblem2.DataContext = selectedRequirement;
            RequirementSolution2.DataContext = selectedRequirement;
            RequirementDescription2.DataContext = selectedRequirement;
            RequirementAliases2.DataContext = selectedRequirement;
            RequirementExamples2.ItemsSource = selectedRequirement.Examples;
            RequirementEditReferences.DataContext = selectedRequirement;
            RequirementEditDomain.DataContext = selectedRequirement;

            

            RequirementPriorityEdit.ItemsSource = risks;

            if (selectedRequirement.Priority == 1)
                RequirementPriorityEdit.SelectedItem = heigh;
            else if(selectedRequirement.Priority == 2)
                RequirementPriorityEdit.SelectedItem = medium;
            else if(selectedRequirement.Priority == 3)
                RequirementPriorityEdit.SelectedItem = low;


        }

        private void SavePrivateRequirementEdit(object sender, RoutedEventArgs e)
        {
            PrivateSecurityRequirementEdit.Visibility = Visibility.Collapsed;
            PrivateSecurityRequirement.Visibility = Visibility.Visible;

  
                PrivateRequirment_PatternsAndCategoriesAndExamples privRequirementChell = new PrivateRequirment_PatternsAndCategoriesAndExamples();
                if (selectedRequirement.ObjectType == (int)Data.SearchableData.types.privateRequirement)
                {
                    privRequirementChell.Requirement = (from pr in dataSet.PrivRequirements
                                                        where pr.Requirement.PrivateRequirementsID == selectedRequirement.ID
                                                        select pr.Requirement).SingleOrDefault<PrivateRequirement>();

                    updateSelectedRequirement(privRequirementChell);
                }
                else if (selectedRequirement.ObjectType == (int)Data.SearchableData.types.requirement)
                {
                    privRequirementChell.Requirement = (from pr in dataSet.PrivRequirements
                                                        from rel in dataSet.RelationProjectPrivateSecurityRequirement
                                                        where pr.Requirement.RequirementID == selectedRequirement.ID &&
                                                        rel.PrivateRequirementID == pr.Requirement.PrivateRequirementsID &&
                                                        rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID
                                                        select pr.Requirement).FirstOrDefault<PrivateRequirement>();

                    //Because the type is requirement, we want to create a private requirement
                    ProgressBar.Visibility = Visibility.Visible;
                    
                    proxy.createPrivateRequirementByRequirementIDAsync(Session.Instance.SessionID, selectedRequirement.ID, privRequirementChell.Requirement.PrivateRequirementsID);
                }


   

        }

        void proxy_createPrivateRequirementByRequirementIDCompleted(object sender, createPrivateRequirementByRequirementIDCompletedEventArgs e)
        {
            PrivateRequirment_PatternsAndCategoriesAndExamples privRequirementChell = new PrivateRequirment_PatternsAndCategoriesAndExamples();
            privRequirementChell.Requirement = (from pr in dataSet.PrivRequirements
                                                from rel in dataSet.RelationProjectPrivateSecurityRequirement
                                                where pr.Requirement.RequirementID == selectedRequirement.ID &&
                                                rel.PrivateRequirementID == pr.Requirement.PrivateRequirementsID &&
                                                rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID
                                                select pr.Requirement).FirstOrDefault<PrivateRequirement>();

            updateSelectedRequirement(privRequirementChell);
        }

        private void updateSelectedRequirement(PrivateRequirment_PatternsAndCategoriesAndExamples privRequirementChell)
        {
            privRequirementChell.Requirement.Name = selectedRequirement.Name;
            privRequirementChell.Requirement.Issues = selectedRequirement.Issues;
            privRequirementChell.Requirement.Problem = selectedRequirement.Problem;
            privRequirementChell.Requirement.Solution = selectedRequirement.Solution;
            privRequirementChell.Requirement.Aliases = selectedRequirement.Aliases;
            privRequirementChell.Requirement.CommonAttacks = selectedRequirement.CommonAttacks;
            privRequirementChell.Requirement.Description = selectedRequirement.Description;
            privRequirementChell.Requirement.References = selectedRequirement.References;
            privRequirementChell.Requirement.Domain = selectedRequirement.Domain;
            privRequirementChell.Requirement.SuggestAsPublic = selectedRequirement.SuggestAsPublic;
            privRequirementChell.Requirement.RequirementID = null;


            ObservableCollection<Example> exampleList = new ObservableCollection<Example>();
            foreach (Example ex in selectedRequirement.Examples)
            {

                if (ex.ExampleID != 0 && ex.RequirementID != null)
                {
                    exampleList.Add(new Example
                    {
                        Name = ex.Name,
                        PrivateRequirementID = privRequirementChell.Requirement.PrivateRequirementsID
                    });
                }
                else
                {
                    ex.PrivateRequirementID = privRequirementChell.Requirement.PrivateRequirementsID;
                    exampleList.Add(ex);
                }
            }

            privRequirementChell.Examples = exampleList;

            String selItem = RequirementPriorityEdit.SelectedItem as String;

            if (selItem != null)
            {
                if (selItem.Equals("Heigh"))
                    privRequirementChell.Requirement.Priority = 1;
                else if (selItem.Equals("Medium"))
                    privRequirementChell.Requirement.Priority = 2;
                else if (selItem.Equals("Low"))
                    privRequirementChell.Requirement.Priority = 3;
            }
            //privRequirementChell.Requirement.Priority = 1; //Default priority..

            ProgressBar.Visibility = Visibility.Visible;
            
            proxy.UpdatePSRPAsync(Session.Instance.SessionID, privRequirementChell);

        }


        void proxy_UpdatePSRPCompleted2(object sender, UpdatePSRPCompletedEventArgs e)
        {
            updateLeftList();
        }

        private void CancelPrivateRequirementEdit(object sender, RoutedEventArgs e)
        {
            PrivateSecurityRequirementEdit.Visibility = Visibility.Collapsed;
            PrivateSecurityRequirement.Visibility = Visibility.Visible;

            populatePrivateRequirements();
        }

        private void ShowSaveNote(object sender, MouseEventArgs e)
        {
            saveNote.Visibility = Visibility.Visible;
        }

        private void CollapsSaveNote(object sender, MouseEventArgs e)
        {
            saveNote.Visibility = Visibility.Collapsed;
        }

        private void CreateNewRequirementExample(object sender, RoutedEventArgs e)
        {
            selectedRequirement.Examples.Add(new Example());
        }

        private void RemovePrivateSecurityRequirement(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;

            try
            {

                if (leftList.Items.Count == 1)
                    HideInfoPanels();

                if (selectedRequirement.ObjectType == (int)Data.SearchableData.types.requirement)
                {

                    proxy.toggleActiveRequirementAsync(Session.Instance.SessionID, Data.SelectedProject.Instance.Project.ProjectID, selectedRequirement.ID);
                }
                else if (selectedRequirement.ObjectType == (int)Data.SearchableData.types.privateRequirement)
                {
 
                    proxy.toggleActivePrivateRequirementAsync(Session.Instance.SessionID, Data.SelectedProject.Instance.Project.ProjectID, selectedRequirement.ID);
                }



            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_toggleActivePrivateRequirementCompleted(object sender, toggleActivePrivateRequirementCompletedEventArgs e)
        {
            updateLeftList() ;
        }

        void proxy_toggleActiveRequirementCompleted(object sender, toggleActiveRequirementCompletedEventArgs e)
        {
            updateLeftList();
        }

        private void CreateNewPrivateRequirement(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;

            proxy.CreateNewPSRPAsync(Session.Instance.SessionID,  new PrivateRequirement(), Data.SelectedProject.Instance.Project.ProjectID);
        }

        void proxy_CreateNewPSRPCompleted(object sender, CreateNewPSRPCompletedEventArgs e)
        {
            updateLeftList();
        }

        private void LeftListItemLoaded(object sender, RoutedEventArgs e)
        {
            Border b = sender as Border;
            Button button = b.FindName("toggleSecurityRequirementButton") as Button;
            List<Pattern> reqPatterns = getRelatedItemsByRequirementID(Convert.ToInt32(button.Tag));

            if (!(reqPatterns.Count > 0))
            {
                button.IsEnabled = false;
            }


        }

        private void Duplicate_Click(object sender, RoutedEventArgs e)
        {
            
        }
        
	}
}