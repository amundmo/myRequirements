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
using System.Collections.ObjectModel;
using System.Linq;

namespace myRequirements
{
	public partial class AdminControlTP : UserControl
	{
        Categories_RequirementsAndPatterns selectedCategorie;
        WebServiceClient proxy;

		public AdminControlTP()
		{
			// Required to initialize variables
			InitializeComponent();

            proxy = new WebServiceClient();

            collapsNewSRPBox();
            collapsNewSDPBox();

            ProgressBarRight.Visibility = Visibility.Visible;
            ProgressBarLeft.Visibility = Visibility.Visible;

            proxy.GetAllCategoriesCompleted += new EventHandler<GetAllCategoriesCompletedEventArgs>(proxy_GetAllCategoriesCompleted);
            proxy.CreateNewCategorieCompleted += new EventHandler<CreateNewCategorieCompletedEventArgs>(proxy_CreateNewCategorieCompleted);
            proxy.UpdateCategorieCompleted += new EventHandler<UpdateCategorieCompletedEventArgs>(proxy_UpdateCategorieCompleted);
            proxy.addCategoryAndRequirementRelationCompleted += new EventHandler<addCategoryAndRequirementRelationCompletedEventArgs>(proxy_addCategoryAndRequirementRelationCompleted);
            proxy.DeleteCategorieCompleted += new EventHandler<DeleteCategorieCompletedEventArgs>(proxy_DeleteCategorieCompleted);
            proxy.removeCategoryAndRequirementRelationCompleted += new EventHandler<removeCategoryAndRequirementRelationCompletedEventArgs>(proxy_removeCategoryAndRequirementRelationCompleted);
            proxy.removeCategoryAndPatternRelationCompleted += new EventHandler<removeCategoryAndPatternRelationCompletedEventArgs>(proxy_removeCategoryAndPatternRelationCompleted);

            questionInfoMessgeBox.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButotn_Click);
            questionInfoMessgeBox.cancelInfoMessageButton.Click += new RoutedEventHandler(cancelInfoMessageButton_Click);
        
        
        
        
        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedItem != null)
            {
                selectedCategorie = (Categories_RequirementsAndPatterns)lb.SelectedItem;
                SelectedElement.Instance.SelectedTP = selectedCategorie.Categorie.CategorieID;

                if (selectedCategorie != null)
                {
                    CategoryEditID.DataContext = selectedCategorie.Categorie;
                    CategoryEditName.DataContext = selectedCategorie.Categorie;
                    CategoryEditDescription.DataContext = selectedCategorie.Categorie;
                    SDPList.ItemsSource = selectedCategorie.Patterns;
                    SRPList.ItemsSource = selectedCategorie.Requirements;
                }
            }
        }

        public void populateleftList()
        {
            ProgressBarLeft.Visibility = Visibility.Visible;
            proxy.GetAllCategoriesAsync(Session.Instance.SessionID);
        }

        void proxy_GetAllCategoriesCompleted(object sender, GetAllCategoriesCompletedEventArgs e)
        {
            try
            {
                leftList.ItemsSource = e.Result;

                for (int i = 0; i < leftList.Items.Count; i++)
                {
                    if (((Categories_RequirementsAndPatterns)leftList.Items[i]).Categorie.CategorieID == SelectedElement.Instance.SelectedTP)
                    {
                        leftList.SelectedIndex = i;
                    }
                }

                ProgressBarRight.Visibility = Visibility.Collapsed;
                ProgressBarLeft.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ProgressBarRight.Visibility = Visibility.Collapsed;
                ProgressBarLeft.Visibility = Visibility.Collapsed;
            }
        }

        private void CreateNewCategory(object sender, RoutedEventArgs e)
        {
            ProgressBarRight.Visibility = Visibility.Visible;

            proxy.CreateNewCategorieAsync(Session.Instance.SessionID, new Categorie());
        }

        void proxy_CreateNewCategorieCompleted(object sender, CreateNewCategorieCompletedEventArgs e)
        {
            populateleftList();
        }

        private void SaveRequirement(object sender, RoutedEventArgs e)
        {
            updateSelectedCategory();
        }

        private void updateSelectedCategory()
        {
            ProgressBarRight.Visibility = Visibility.Visible;

          
            proxy.UpdateCategorieAsync(Session.Instance.SessionID, selectedCategorie);
        }

        void proxy_UpdateCategorieCompleted(object sender, UpdateCategorieCompletedEventArgs e)
        {
            if (!e.Result.Equals("Security Requirement successfully"))
            {
                //Do something!
            }

            ProgressBarRight.Visibility = Visibility.Collapsed;
        }

        private void CancelChanges(object sender, RoutedEventArgs e)
        {
            ProgressBarRight.Visibility = Visibility.Visible;
            populateleftList();
        }

        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            questionInfoMessgeBox.Visibility = Visibility.Visible;
            questionInfoMessgeBox.header.Text = "Are you sure?";
            questionInfoMessgeBox.message.Text = "Are you sure you want to delete the selected Category?";
            
        }

        void cancelInfoMessageButton_Click(object sender, RoutedEventArgs e)
        {
            questionInfoMessgeBox.Visibility = Visibility.Collapsed;
        }

        void okInfoMessageButotn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCategorie != null)
            {
                questionInfoMessgeBox.Visibility = Visibility.Collapsed;
                ProgressBarRight.Visibility = Visibility.Visible;

                proxy.DeleteCategorieAsync(Session.Instance.SessionID, selectedCategorie.Categorie);
            }
            else
            {
                //Some infomessage to user
            }
        }

        void proxy_DeleteCategorieCompleted(object sender, DeleteCategorieCompletedEventArgs e)
        {
            populateleftList();
        }



        private void newSRP(object sender, RoutedEventArgs e)
        {
            NewSRPIDTextBox.Text = "";

            if (NewSRPIDTextBlock.Visibility == Visibility.Visible)
            {
                collapsNewSRPBox();
            }
            else
            {
                showNewSRPBox();
            }
        }

        private void showNewSRPBox()
        {
            NewSRPIDTextBlock.Visibility = Visibility.Visible;
            NewSRPIDTextBox.Visibility = Visibility.Visible;
            NewSRPOKButton.Visibility = Visibility.Visible;
        }

        private void collapsNewSRPBox()
        {
            NewSRPIDTextBlock.Visibility = Visibility.Collapsed;
            NewSRPIDTextBox.Visibility = Visibility.Collapsed;
            NewSRPOKButton.Visibility = Visibility.Collapsed;
        }



        private void NewSRPOKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;
                proxy.addCategoryAndRequirementRelationAsync(Session.Instance.SessionID, Convert.ToInt32(NewSRPIDTextBox.Text), selectedCategorie.Categorie.CategorieID);

            }
            catch (Exception)
            {
                collapsNewSRPBox();
            }
        }

        void proxy_addCategoryAndRequirementRelationCompleted(object sender, addCategoryAndRequirementRelationCompletedEventArgs e)
        {
            collapsNewSRPBox();
            populateleftList();
        }

        private void collapsNewSDPBox()
        {
            NewSDPIDText.Visibility = Visibility.Collapsed;
            NewSDPIDTextBox.Visibility = Visibility.Collapsed;
            NewSDPOKButton.Visibility = Visibility.Collapsed;
        }

        private void showNewSDPBox()
        {
            NewSDPIDText.Visibility = Visibility.Visible;
            NewSDPIDTextBox.Visibility = Visibility.Visible;
            NewSDPOKButton.Visibility = Visibility.Visible;
        }

        private void NewSDP(object sender, RoutedEventArgs e)
        {

            NewSDPIDTextBox.Text = "";
            if (NewSDPIDText.Visibility == Visibility.Visible)
            {
                collapsNewSDPBox();
            }
            else
            {
                showNewSDPBox();
            }
        }

        private void NewSDPOKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;

                proxy.addCategoryAndDesignRelationCompleted += new EventHandler<addCategoryAndDesignRelationCompletedEventArgs>(proxy_addCategoryAndDesignRelationCompleted);
                proxy.addCategoryAndDesignRelationAsync(Session.Instance.SessionID, Convert.ToInt32(NewSDPIDTextBox.Text), selectedCategorie.Categorie.CategorieID);

            }
            catch (Exception)
            {
                collapsNewSDPBox();
            }
        }

        void proxy_addCategoryAndDesignRelationCompleted(object sender, addCategoryAndDesignRelationCompletedEventArgs e)
        {
            collapsNewSDPBox();

            populateleftList();
        }

        private void deleteRequirementRelation(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;
                Button button = sender as Button;

                proxy.removeCategoryAndRequirementRelationAsync(Session.Instance.SessionID, selectedCategorie.Categorie.CategorieID, Convert.ToInt32(button.Tag));
            }
            catch (Exception)
            {
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_removeCategoryAndRequirementRelationCompleted(object sender, removeCategoryAndRequirementRelationCompletedEventArgs e)
        {
            populateleftList();
        }

        private void deletePatternRelation(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;
                Button button = sender as Button;

                proxy.removeCategoryAndPatternRelationAsync(Session.Instance.SessionID, selectedCategorie.Categorie.CategorieID, Convert.ToInt32(button.Tag));
            }
            catch (Exception)
            {
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_removeCategoryAndPatternRelationCompleted(object sender, removeCategoryAndPatternRelationCompletedEventArgs e)
        {
            populateleftList();
        }

        private void SRPList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
                SelectedElement.Instance.SelectedSRP = ((sender as ListBox).SelectedItem as Requirement).RequirementID;

        }

        private void SDPList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
               if ((sender as ListBox).SelectedItem != null)
                SelectedElement.Instance.SelectedSDP = ((sender as ListBox).SelectedItem as Pattern).PatternID;
        }


	}
}