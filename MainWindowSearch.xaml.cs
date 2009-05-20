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
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace myRequirements
{
	public partial class MainWindowSearch : UserControl
	{
        private ServiceReference1.WebServiceClient proxy;
        private RequirementsAndPatternsAndCategoriesAndPrivRequirements dataSet;
        private List<Data.SearchableData> searchResult;
        private ListBox selectedListBox;
        private TextChangedEventHandler textChangedEventHandler;
        private List<SearchableData> tmpSelectedList;
        private bool isOdd = false;

		public MainWindowSearch()
		{
			// Required to initialize variables
			InitializeComponent();
            proxy = new WebServiceClient();
            tmpSelectedList = new List<SearchableData>();

            dataSet = new RequirementsAndPatternsAndCategoriesAndPrivRequirements();
            searchResult = new List<Data.SearchableData>();

            proxy.getAllDataCompleted += new EventHandler<getAllDataCompletedEventArgs>(proxy_getAllDataCompleted);
            proxy.toggleActiveRequirementCompleted += new EventHandler<toggleActiveRequirementCompletedEventArgs>(proxy_toggleActiveRequirementCompleted);
            proxy.toggleActivePrivateRequirementCompleted += new EventHandler<toggleActivePrivateRequirementCompletedEventArgs>(proxy_toggleActivePrivateRequirementCompleted);

            textChangedEventHandler = new TextChangedEventHandler(searchString_TextChanged);
            searchString.TextChanged += textChangedEventHandler;

            getAllSearchData();

            //Hide info panels
            hideInfoPanels();
		}

        void searchString_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateSearchList();
        }

        public void updateSearchList(){
            try
            {
                int selItem = leftList.SelectedIndex;
                List<Data.SearchableData> result = null;

                if (String.IsNullOrEmpty(searchString.Text))
                {
                    result = (from c in searchResult
                              where (int)c.ObjectType == (int)Data.SearchableData.types.threatCategory
                              orderby c.Name
                              select c).ToList<Data.SearchableData>();

                    leftList.ItemsSource = result;

                }
                else
                {
                    result = (from p in searchResult
                                            where 
                                            (!String.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(searchString.Text.ToLower()) ||
                                            !String.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchString.Text.ToLower()) ||
                                            !String.IsNullOrEmpty(p.Problem) && p.Problem.ToLower().Contains(searchString.Text.ToLower()) ||
                                            !String.IsNullOrEmpty(p.Aliases) && p.Aliases.ToLower().Contains(searchString.Text.ToLower()) ||
                                            !String.IsNullOrEmpty(p.References) && p.References.ToLower().Contains(searchString.Text.ToLower()) ||
                                            !String.IsNullOrEmpty(p.Domain) && p.Domain.ToLower().Contains(searchString.Text.ToLower()) ||
                                            !String.IsNullOrEmpty(p.Solution) && p.Solution.ToLower().Contains(searchString.Text.ToLower()) ||
                                            !String.IsNullOrEmpty(p.ID+"") && (p.ID+"").Contains(searchString.Text))
                                            where SRPCheckBox.IsChecked == true && ((int)p.ObjectType == (int)Data.SearchableData.types.requirement) ||
                                            SDPCheckBox.IsChecked == true && ((int)p.ObjectType == (int)Data.SearchableData.types.pattern) ||
                                            CategoriesCheckBox.IsChecked == true && ((int)p.ObjectType == (int)Data.SearchableData.types.threatCategory) ||
                                            PSRPCheckBox.IsChecked == true && ((int)p.ObjectType == (int)Data.SearchableData.types.privateRequirement)
                                            orderby p.Name
                                            select p).ToList<Data.SearchableData>();
                    

                    leftList.ItemsSource = result;
                 }

                if (selItem == -1 && result.Count > 0)
                    leftList.SelectedIndex = 0;
                else
                    leftList.SelectedIndex = selItem;
            }
            catch (Exception)
                { }

            ProgressBar.Visibility = Visibility.Collapsed;
        }



        public void getAllSearchData()
        {
            try
            {
                ProgressBar.Visibility = Visibility.Visible;

                proxy.getAllDataAsync(Session.Instance.SessionID);
            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }

        }

        void proxy_getAllDataCompleted(object sender, getAllDataCompletedEventArgs e)
        {
            try
            {
                dataSet = e.Result as RequirementsAndPatternsAndCategoriesAndPrivRequirements;

                populateSearchResult();
                updateSearchList();
            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void populateSearchData(List<Data.SearchableData> searchableData, ObservableCollection<ServiceReference1.Requirment_PatternsAndCategoriesAndExamples> r, ObservableCollection<Pattern> p, ObservableCollection<Categorie> c, ObservableCollection<ServiceReference1.PrivateRequirment_PatternsAndCategoriesAndExamples> pr)
        {
            if (r != null)
            {
                foreach (ServiceReference1.Requirment_PatternsAndCategoriesAndExamples req in r)
                {
                    bool isSelected = false;
                    try
                    {
                        var tmppreq = (from preq in pr
                                       from rel in dataSet.RelationProjectPrivateSecurityRequirement
                                       where rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID &&
                                       rel.PrivateRequirementID == preq.Requirement.PrivateRequirementsID &&
                                       preq.Requirement.RequirementID == req.Requirement.RequirementID
                                    select preq.Requirement).Single<PrivateRequirement>();

                        if (tmppreq != null)
                            isSelected = true;
                    }
                    catch (Exception) { };

                    searchableData.Add(new Data.SearchableData { 
                        Name = req.Requirement.Name, 
                        Examples=req.Examples, 
                        Description = req.Requirement.Description, 
                        ID = req.Requirement.RequirementID, 
                        Problem = req.Requirement.Problem, 
                        Solution = req.Requirement.Solution, 
                        Aliases = req.Requirement.Aliases,
                        References = req.Requirement.References,
                        Domain = req.Requirement.Domain,
                        isSelectedAsRequirement = isSelected, 
                        ObjectType = (int)Data.SearchableData.types.requirement });
                }
            }

            if (p != null)
            {
                foreach (Pattern pat in p)
                {
                    searchableData.Add(new Data.SearchableData 
                    { Name = pat.Name, 
                        Description = pat.Description, 
                        Problem = pat.Problem, 
                        Solution = pat.Solution, 
                        Aliases = pat.Aliases,
                        References = pat.References,
                        Domain = pat.Domain,
                        ID = pat.PatternID, 
                        ObjectType = (int)Data.SearchableData.types.pattern 
                    });
                }
            }

            if (pr != null)
            {
                foreach (PrivateRequirment_PatternsAndCategoriesAndExamples preq in pr)
                {
                        var tmpRel = (from rel in dataSet.RelationProjectPrivateSecurityRequirement 
                                     where rel.PrivateRequirementID == preq.Requirement.PrivateRequirementsID &&
                                     rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID
                                     select rel).FirstOrDefault<RelationProjectPrivateRequirement>();

                        searchableData.Add(new Data.SearchableData { 
                        Name = preq.Requirement.Name, 
                        isSelectedAsRequirement = (tmpRel != null),
                        Description = preq.Requirement.Description, 
                        Aliases = preq.Requirement.Aliases,
                        Problem = preq.Requirement.Problem, 
                        Solution = preq.Requirement.Solution, 
                        References = preq.Requirement.References,
                        Domain = preq.Requirement.Domain,
                        ID = preq.Requirement.PrivateRequirementsID, 
                        ObjectType = (int)Data.SearchableData.types.privateRequirement });
                }
            }

            if (c != null)
            {
                foreach (Categorie cat in dataSet.Categories)
                {
                    searchableData.Add(new Data.SearchableData { Name = cat.Name, Description = cat.Description, ID = cat.CategorieID, ObjectType = (int)Data.SearchableData.types.threatCategory });
                }
            }

        }

        private void populateSearchResult()
        {
            searchResult.Clear();
            populateSearchData(searchResult, dataSet.Requirements, dataSet.Patterns, dataSet.Categories, dataSet.PrivRequirements);

        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Grid ItemRef = sender as Grid;
            CheckBox cb = ItemRef.FindName("isSelectedAsRequirementCheckbox") as CheckBox;
            Border b = ItemRef.FindName("Border") as Border;


            try
            {
                if ((int)ItemRef.Tag == (int)Data.SearchableData.types.requirement)
                {
                    b.Background = greenBackground;

                    if (cb != null)
                    {
                        cb.Visibility = Visibility.Visible;
                        cb.IsEnabled = true;
                    }
                }
                else if ((int)ItemRef.Tag == (int)Data.SearchableData.types.privateRequirement)
                {
                    b.Background = darkGreenBackground;
                    if (cb != null)
                    {
                        cb.IsEnabled = true;
                        cb.Visibility = Visibility.Visible;
                    }
                }
                else if ((int)ItemRef.Tag == (int)Data.SearchableData.types.pattern)
                    b.Background = redBackground;
                else
                    b.Background = bluBackground;
            }
            catch (Exception)
            { 
            }
        }

        private void SRPCheckBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void SearchFilterChanged(object sender, RoutedEventArgs e)
        {
            updateSearchList();
        }

        private void ExpandList_Click(object sender, RoutedEventArgs e)
        {
            Button b1 = sender as Button;


            Grid g1 = b1.Parent as Grid;
            ListBox l1 = g1.FindName("ListBoxChild") as ListBox;

            if (b1.Content.Equals("+"))
            {
                Data.SearchableData sobject = b1.Tag as Data.SearchableData;

                List<Data.SearchableData> searchableDataChild = getChildElements(sobject);

                l1.ItemsSource = searchableDataChild.OrderBy(x => x.Name);
            }

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

        private List<Data.SearchableData> getChildElements(Data.SearchableData sobject)
        {
            List<Data.SearchableData> searchableDataChild = new List<myRequirements.Data.SearchableData>();

            List<ServiceReference1.Requirment_PatternsAndCategoriesAndExamples> rList = null;
            List<Categorie> cList = null;
            List<PrivateRequirement> prList = null;
            List<Pattern> pList = null;


            if (sobject.ObjectType == (int)Data.SearchableData.types.threatCategory)
            {
                rList = (from rel in dataSet.RelationCategoryRequirement
                         from req in dataSet.Requirements
                         where rel.CategorieID == sobject.ID && rel.RequirementID == req.Requirement.RequirementID
                         select req).ToList<ServiceReference1.Requirment_PatternsAndCategoriesAndExamples>();

                pList = (from rel in dataSet.RelationCategoryPattern
                         from pat in dataSet.Patterns
                         where rel.CategorieID == sobject.ID && rel.PatternID == pat.PatternID
                         select pat).ToList<Pattern>();


                prList = (from rel in dataSet.RelationCategoryPrivateSecurityRequirement
                          from privReq in dataSet.PrivRequirements
                          where rel.CategorieID == sobject.ID && rel.PrivateRequirementID == privReq.Requirement.PrivateRequirementsID
                          select privReq.Requirement).ToList<PrivateRequirement>();

            }
            else if (sobject.ObjectType == (int)Data.SearchableData.types.pattern)
            {
                rList = (from rel in dataSet.RelationPatternRequirement
                         from req in dataSet.Requirements
                         where rel.PatternID == sobject.ID && rel.RequirementID == req.Requirement.RequirementID
                         select req).ToList<ServiceReference1.Requirment_PatternsAndCategoriesAndExamples>();

                cList = (from rel in dataSet.RelationCategoryPattern
                         from cat in dataSet.Categories
                         where rel.PatternID == sobject.ID && rel.CategorieID == cat.CategorieID
                         select cat).ToList<Categorie>();

                prList = (from rel in dataSet.RelationPatternPrivateSecurityRequirement
                          from privReq in dataSet.PrivRequirements
                          where rel.PatternID == sobject.ID && rel.PrivateRequirementID == privReq.Requirement.PrivateRequirementsID
                          select privReq.Requirement).ToList<PrivateRequirement>();
            }
            else if (sobject.ObjectType == (int)Data.SearchableData.types.requirement)
            {
                pList = (from rel in dataSet.RelationPatternRequirement
                         from pat in dataSet.Patterns
                         where rel.RequirementID == sobject.ID && rel.PatternID == pat.PatternID
                         select pat).ToList<Pattern>();

                cList = (from rel in dataSet.RelationCategoryRequirement
                         from cat in dataSet.Categories
                         where rel.RequirementID == sobject.ID && rel.CategorieID == cat.CategorieID
                         select cat).ToList<Categorie>();

            }

            else if (sobject.ObjectType == (int)Data.SearchableData.types.privateRequirement)
            {
                pList = (from rel in dataSet.RelationPatternPrivateSecurityRequirement
                         from pat in dataSet.Patterns
                         where rel.PrivateRequirementID == sobject.ID && rel.PatternID == pat.PatternID
                         select pat).ToList<Pattern>();

                cList = (from rel in dataSet.RelationCategoryPrivateSecurityRequirement
                         from cat in dataSet.Categories
                         where rel.PrivateRequirementID == sobject.ID && rel.CategorieID == cat.CategorieID
                         select cat).ToList<Categorie>();
            }


            if (rList != null && SRPCheckBox.IsChecked == true)
            {
                foreach (ServiceReference1.Requirment_PatternsAndCategoriesAndExamples req in rList)
                {
                    bool isSelected = false;

                    try
                    {
                        var tmppreq = (from preq in dataSet.PrivRequirements
                                       from rel in dataSet.RelationProjectPrivateSecurityRequirement
                                       where rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID &&
                                       rel.PrivateRequirementID == preq.Requirement.PrivateRequirementsID &&
                                       preq.Requirement.RequirementID == req.Requirement.RequirementID
                                       select preq.Requirement).Single<PrivateRequirement>();

                        if (tmppreq != null)
                            isSelected = true;
                    }
                    catch (Exception tja) { };

                    foreach (SearchableData tmpData in tmpSelectedList)
                    {
                        if (tmpData.ID == req.Requirement.RequirementID)
                        {
                            isSelected = true;
                        }
                    }

                    if (!(sobject.Parent != null &&
                        sobject.Parent.ObjectType == (int)Data.SearchableData.types.requirement &&
                        sobject.ID == req.Requirement.RequirementID))
                    {
                        searchableDataChild.Add(new Data.SearchableData
                        {
                            Name = req.Requirement.Name,
                            Examples = req.Examples,
                            isSelectedAsRequirement = isSelected,
                            Description = req.Requirement.Description,
                            ID = req.Requirement.RequirementID,
                            Problem = req.Requirement.Problem,
                            Solution = req.Requirement.Solution,
                            Aliases = req.Requirement.Aliases,
                            References = req.Requirement.References,
                            Domain = req.Requirement.Domain,
                            Parent = sobject,
                            ObjectType = (int)Data.SearchableData.types.requirement
                        });
                    }
                }
            }

            if (pList != null && SDPCheckBox.IsChecked == true)
            {
                foreach (Pattern pat in pList)
                {

                    if (!(sobject.Parent != null &&
                        sobject.Parent.ObjectType == (int)Data.SearchableData.types.pattern &&
                        sobject.Parent.ID == pat.PatternID))
                    {
                        searchableDataChild.Add(new Data.SearchableData
                        {
                            Name = pat.Name,
                            Description = pat.Description,
                            Problem = pat.Problem,
                            Solution = pat.Solution,
                            References = pat.References,
                            Domain = pat.Domain,
                            Aliases = pat.Aliases,
                            ID = pat.PatternID,
                            Parent = sobject,
                            ObjectType = (int)Data.SearchableData.types.pattern
                        });
                    }
                }
            }

            if (prList != null && PSRPCheckBox.IsChecked == true)
            {
                foreach (PrivateRequirement preq in prList)
                {
                    var tmpRel = (from rel in dataSet.RelationProjectPrivateSecurityRequirement
                                  where rel.PrivateRequirementID == preq.PrivateRequirementsID &&
                                  rel.ProjectID == Data.SelectedProject.Instance.Project.ProjectID
                                  select rel).FirstOrDefault<RelationProjectPrivateRequirement>();

                    if (!(sobject.Parent != null &&
                       sobject.Parent.ObjectType == (int)Data.SearchableData.types.privateRequirement &&
                       sobject.Parent.ID == preq.PrivateRequirementsID))
                    {
                        searchableDataChild.Add(new Data.SearchableData
                        {
                            Name = preq.Name,
                            isSelectedAsRequirement = (tmpRel != null),
                            Description = preq.Description,
                            Aliases = preq.Aliases,
                            Problem = preq.Problem,
                            Solution = preq.Solution,
                            References = preq.References,
                            Domain = preq.Domain,
                            ID = preq.PrivateRequirementsID,
                            Parent = sobject,
                            ObjectType = (int)Data.SearchableData.types.privateRequirement
                        }
                            );
                    }
                }
            }

            if (cList != null && CategoriesCheckBox.IsChecked == true)
            {
                foreach (Categorie cat in cList)
                {
                    if (!(sobject.Parent != null &&
                        sobject.Parent.ObjectType == (int)Data.SearchableData.types.threatCategory &&
                        sobject.Parent.ID == cat.CategorieID))
                    {
                        searchableDataChild.Add(new Data.SearchableData
                        {
                            Name = cat.Name,
                            Description = cat.Description,
                            ID = cat.CategorieID,
                            Parent = sobject,
                            ObjectType = (int)Data.SearchableData.types.threatCategory
                        });
                    }
                }
            }
            return searchableDataChild;
        }


        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (selectedListBox != null && !selectedListBox.Equals(lb))
            {
                
                selectedListBox.SelectedIndex = -1;
            }
            selectedListBox = lb;
           

            Data.SearchableData selectedItem = selectedListBox.SelectedItem as Data.SearchableData;
            hideInfoPanels();

            if (selectedItem != null)
            {
                switch (selectedItem.ObjectType)
                {
                    case ((int)Data.SearchableData.types.requirement):
                        requirementID.DataContext = selectedItem;
                        RequirementName.DataContext = selectedItem;
                        RequirementProblem.DataContext = selectedItem;
                        RequirementAliases.DataContext = selectedItem;
                        RequirementDescription.DataContext = selectedItem;
                        RequirementSolution.DataContext = selectedItem;

                        foreach (Example ex in selectedItem.Examples)
                        {
                            ex.Name = ex.Name + " " + ex.ExampleID;
                        }

                        RequirementExamples.ItemsSource = selectedItem.Examples;
                        References_SRP.DataContext = selectedItem;
                        Domains_SRP.DataContext = selectedItem;

                        SecurityRequirement.Visibility = Visibility.Visible;

                        break;
                    case ((int)Data.SearchableData.types.pattern):
                        PatternID.DataContext = selectedItem;
                        PatternName.DataContext = selectedItem;
                        PatternDescription.DataContext = selectedItem;
                        PatternProblem.DataContext = selectedItem;
                        PatternSolution.DataContext = selectedItem;
                        PatternAliases.DataContext = selectedItem;
                        References_SDP.DataContext = selectedItem;
                        Domains_SDP.DataContext = selectedItem;

                        SecurityDesignPattern.Visibility = Visibility.Visible;
                        break;

                   case ((int)Data.SearchableData.types.threatCategory):
                        CategoryID.DataContext = selectedItem;
                        CategoryName.DataContext = selectedItem;
                        CategoryDescription.DataContext = selectedItem;

                        SecurityCategories.Visibility = Visibility.Visible;

                        break;
                    case((int)Data.SearchableData.types.privateRequirement):
                        requirementID1.DataContext = selectedItem;
                        RequirementName1.DataContext = selectedItem;
                        RequirementProblem1.DataContext = selectedItem;
                        RequirementAliases1.DataContext = selectedItem;
                        RequirementDescription1.DataContext = selectedItem;
                        RequirementSolution1.DataContext = selectedItem;
                        RequirementExamples1.ItemsSource = selectedItem.Examples;
                        References_PSRP.DataContext = selectedItem;
                        References_PSRP.DataContext = selectedItem;

                        PrivateSecurityRequirement.Visibility = Visibility.Visible;
                        break;

                    default:
                        break;
                }
            }
        }

        private void hideInfoPanels()
        {
            PrivateSecurityRequirement.Visibility = Visibility.Collapsed;
            SecurityRequirement.Visibility = Visibility.Collapsed;
            SecurityDesignPattern.Visibility = Visibility.Collapsed;
            SecurityCategories.Visibility = Visibility.Collapsed;
        }

        private void isSelectedAsRequirementCheckbox_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;

            try
            {

                CheckBox cb = sender as CheckBox;
                Data.SearchableData data = cb.Tag as Data.SearchableData;

                if (data.ObjectType == (int)Data.SearchableData.types.requirement)
                {   
                    proxy.toggleActiveRequirementAsync(Session.Instance.SessionID, Data.SelectedProject.Instance.Project.ProjectID, data.ID);
                }
                else if (data.ObjectType == (int)Data.SearchableData.types.privateRequirement)
                {
                    proxy.toggleActivePrivateRequirementAsync(Session.Instance.SessionID, Data.SelectedProject.Instance.Project.ProjectID, data.ID);
                }

                if (cb.IsChecked == true)
                {
                    tmpSelectedList.Add(data);
                }
                else
                {
                    foreach (SearchableData tmpData in tmpSelectedList)
                    {
                        if (tmpData.ID == data.ID)
                            tmpSelectedList.Remove(tmpData);
                    }

                    try
                    {
                        var preq = (from r in dataSet.PrivRequirements
                                    where r.Requirement.RequirementID == data.ID
                                    select r.Requirement).First<PrivateRequirement>();

                        if (preq != null)
                            preq.RequirementID = null;
                    }
                    catch (Exception tja) { }
                }
            }
            catch (Exception)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_toggleActivePrivateRequirementCompleted(object sender, toggleActivePrivateRequirementCompletedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        void proxy_toggleActiveRequirementCompleted(object sender, toggleActiveRequirementCompletedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void RequirementExamples_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            Border b = tb.Parent as Border;

            if (isOdd)
                b.Background = lightGrey;
 
            isOdd = !isOdd;
        }

        private void RequirementExamples_Loaded_1(object sender, RoutedEventArgs e)
        {
            isOdd = false;
        }
	}
}