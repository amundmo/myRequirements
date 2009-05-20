using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using myRequirements.Data;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using myRequirements.ServiceReference1;

namespace myRequirements
{
	public partial class AdminControlSDP : UserControl
	{
        private ServiceReference1.WebServiceClient proxy;
        private ServiceReference1.Pattern_RequirementAndCategories selectedPattern;

		public AdminControlSDP()
		{
			// Required to initialize variables
			InitializeComponent();

            proxy = new ServiceReference1.WebServiceClient();

            NewSRPIDText.Visibility = Visibility.Collapsed;
            NewSRPIDTextBox.Visibility = Visibility.Collapsed;
            NewSRPOKButton.Visibility = Visibility.Collapsed;

            NewCategoryOKButton.Visibility = Visibility.Collapsed;
            NewCategoryIDTextBox.Visibility = Visibility.Collapsed;
            NewCategoryIDTextBlock.Visibility = Visibility.Collapsed;

            ProgressBarRight.Visibility = Visibility.Visible;
            ProgressBarLeft.Visibility = Visibility.Visible;

            proxy.GetAllSDPCompleted += new EventHandler<myRequirements.ServiceReference1.GetAllSDPCompletedEventArgs>(proxy_GetAllSDPCompleted);
            proxy.CreateNewSDPCompleted += new EventHandler<myRequirements.ServiceReference1.CreateNewSDPCompletedEventArgs>(proxy_CreateNewSDPCompleted);
            proxy.UpdateSDPCompleted += new EventHandler<myRequirements.ServiceReference1.UpdateSDPCompletedEventArgs>(proxy_UpdateSDPCompleted);
            questionInfoMessgeBox.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButton_Click);
            questionInfoMessgeBox.cancelInfoMessageButton.Click += new RoutedEventHandler(cancelInfoMessageButton_Click);
            proxy.DeleteSDPCompleted += new EventHandler<myRequirements.ServiceReference1.DeleteSDPCompletedEventArgs>(proxy_DeleteSDPCompleted);
            proxy.addPatternAndRequirementRelationCompleted += new EventHandler<myRequirements.ServiceReference1.addPatternAndRequirementRelationCompletedEventArgs>(proxy_addPatternAndRequirementRelationCompleted);
            proxy.removePatternandRequirementRelationCompleted += new EventHandler<myRequirements.ServiceReference1.removePatternandRequirementRelationCompletedEventArgs>(proxy_removePatternandRequirementRelationCompleted);
            proxy.addCategoryAndDesignRelationCompleted += new EventHandler<myRequirements.ServiceReference1.addCategoryAndDesignRelationCompletedEventArgs>(proxy_addCategoryAndDesignRelationCompleted);
            proxy.removeCategoryAndPatternRelationCompleted += new EventHandler<myRequirements.ServiceReference1.removeCategoryAndPatternRelationCompletedEventArgs>(proxy_removeCategoryAndPatternRelationCompleted);
		}


       public void populateleftList()
        {
            ProgressBarLeft.Visibility = Visibility.Visible;
            proxy.GetAllSDPAsync(Session.Instance.SessionID);
        }

       void proxy_GetAllSDPCompleted(object sender, myRequirements.ServiceReference1.GetAllSDPCompletedEventArgs e)
       {
           try
           {
               leftList.ItemsSource = e.Result;
               

               //if (e.Result.Count > 0) //Fixes the cancel button :)
               //    leftList.SelectedIndex = 0;

               for (int i = 0; i < leftList.Items.Count; i++)
               {
                   if (((Pattern_RequirementAndCategories)leftList.Items[i]).Pattern.PatternID == SelectedElement.Instance.SelectedSDP)
                   {
                       leftList.SelectedIndex = i;
                       break;
                   }
               }


               ProgressBarRight.Visibility = Visibility.Collapsed;
               ProgressBarLeft.Visibility = Visibility.Collapsed;
           }
           catch (Exception ja)
           {
               ProgressBarRight.Visibility = Visibility.Collapsed;
               ProgressBarLeft.Visibility = Visibility.Collapsed;
           }
       }

        private void CreateNewSDP(object sender, RoutedEventArgs e)
        {

            ProgressBarRight.Visibility = Visibility.Visible;

            proxy.CreateNewSDPAsync(Session.Instance.SessionID, new ServiceReference1.Pattern());

        }

        void proxy_CreateNewSDPCompleted(object sender, myRequirements.ServiceReference1.CreateNewSDPCompletedEventArgs e)
        {
            populateleftList();
        }


        private void leftList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedItem != null)
            {
                selectedPattern = (ServiceReference1.Pattern_RequirementAndCategories)lb.SelectedItem;
                SelectedElement.Instance.SelectedSDP = selectedPattern.Pattern.PatternID;

                if (selectedPattern != null)
                {

                    //Update category and pattern list
                    //updatePatternsAndCategories();


                    requirementEditID.DataContext = selectedPattern.Pattern;
                    requirementEditName.DataContext = selectedPattern.Pattern;
                    requirementEditAliases.DataContext = selectedPattern.Pattern;
                    requirementEditDescription.DataContext = selectedPattern.Pattern;
                    requirementEditProblem.DataContext = selectedPattern.Pattern;
                    requirementEditSolution.DataContext = selectedPattern.Pattern;
                    requirementEditCategoriesList.ItemsSource = selectedPattern.Categories;
                    requirementEditRequirementList.ItemsSource = selectedPattern.Requirements;
                    requirementEditReferences.DataContext = selectedPattern.Pattern;
                    requirementEditDomain.DataContext = selectedPattern.Pattern;
                }
            }
        }

        private void SavePattern(object sender, RoutedEventArgs e)
        {
            updateSelectedPattern();
        }

        private void updateSelectedPattern()
        {
            ProgressBarRight.Visibility = Visibility.Visible;

            proxy.UpdateSDPAsync(Session.Instance.SessionID, selectedPattern);
        }

        void proxy_UpdateSDPCompleted(object sender, myRequirements.ServiceReference1.UpdateSDPCompletedEventArgs e)
        {
            if (!e.Result.Equals("Security Requirement successfully"))
            {
                //Do something!
            }

            ProgressBarRight.Visibility = Visibility.Collapsed;
        }

        private void CancelEditPattern(object sender, RoutedEventArgs e)
        {
            ProgressBarRight.Visibility = Visibility.Visible;
            populateleftList();
        }

        private void DeletePattern(object sender, RoutedEventArgs e)
        {
            questionInfoMessgeBox.Visibility = Visibility.Visible;
            questionInfoMessgeBox.header.Text = "Are you sure?";
            questionInfoMessgeBox.message.Text = "Are you sure you want to delete the selected Security Design Pattern?";

        }

        void cancelInfoMessageButton_Click(object sender, RoutedEventArgs e)
        {
            questionInfoMessgeBox.Visibility = Visibility.Collapsed;
        }

        void okInfoMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPattern != null)
            {
                questionInfoMessgeBox.Visibility = Visibility.Collapsed;
                ProgressBarRight.Visibility = Visibility.Visible;

                proxy.DeleteSDPAsync(Session.Instance.SessionID, selectedPattern.Pattern);
            }
            else
            {
                //message to user
            }
        }

        void proxy_DeleteSDPCompleted(object sender, myRequirements.ServiceReference1.DeleteSDPCompletedEventArgs e)
        {
            populateleftList();
        }

        private void NewSDPClick(object sender, RoutedEventArgs e)
        {
            NewSRPIDTextBox.Text = "";

            if (NewSRPIDText.Visibility == Visibility.Visible)
            {
                NewSRPIDText.Visibility = Visibility.Collapsed;
                NewSRPIDTextBox.Visibility = Visibility.Collapsed;
                NewSRPOKButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                NewSRPIDText.Visibility = Visibility.Visible;
                NewSRPIDTextBox.Visibility = Visibility.Visible;
                NewSRPOKButton.Visibility = Visibility.Visible;
            }

        }

        private void NewSRPOKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;

                proxy.addPatternAndRequirementRelationAsync(Session.Instance.SessionID, Convert.ToInt32(NewSRPIDTextBox.Text.Trim()), selectedPattern.Pattern.PatternID);
            }
            catch (Exception)
            {
                NewSRPIDText.Visibility = Visibility.Collapsed;
                NewSRPIDTextBox.Visibility = Visibility.Collapsed;
                NewSRPOKButton.Visibility = Visibility.Collapsed;
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_addPatternAndRequirementRelationCompleted(object sender, myRequirements.ServiceReference1.addPatternAndRequirementRelationCompletedEventArgs e)
        {
            NewSRPIDText.Visibility = Visibility.Collapsed;
            NewSRPIDTextBox.Visibility = Visibility.Collapsed;
            NewSRPOKButton.Visibility = Visibility.Collapsed;

            populateleftList();
        }

 
        private void DeleteSRP(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;
                Button button = sender as Button;

                proxy.removePatternandRequirementRelationAsync(Session.Instance.SessionID, selectedPattern.Pattern.PatternID, Convert.ToInt32(button.Tag));

            }
            catch (Exception)
            {
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
            
        }

        void proxy_removePatternandRequirementRelationCompleted(object sender, myRequirements.ServiceReference1.removePatternandRequirementRelationCompletedEventArgs e)
        {
            populateleftList();
        }


        private void newCategoryClick(object sender, RoutedEventArgs e)
        {
            NewCategoryIDTextBox.Text = "";

            if (NewCategoryIDTextBlock.Visibility == Visibility.Visible)
            {
                NewCategoryIDTextBlock.Visibility = Visibility.Collapsed;
                NewCategoryIDTextBox.Visibility = Visibility.Collapsed;
                NewCategoryOKButton.Visibility = Visibility.Collapsed;

            }
            else
            {
                NewCategoryIDTextBlock.Visibility = Visibility.Visible;
                NewCategoryIDTextBox.Visibility = Visibility.Visible;
                NewCategoryOKButton.Visibility = Visibility.Visible;
            }

        }

        private void NewCategoryOKButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarRight.Visibility = Visibility.Visible;

            try
            {
                proxy.addCategoryAndDesignRelationAsync(Session.Instance.SessionID, selectedPattern.Pattern.PatternID, Convert.ToInt32(NewCategoryIDTextBox.Text.Trim()));
            }
            catch (Exception)
            {
                NewCategoryIDTextBlock.Visibility = Visibility.Collapsed;
                NewCategoryIDTextBox.Visibility = Visibility.Collapsed;
                NewCategoryOKButton.Visibility = Visibility.Collapsed;
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_addCategoryAndDesignRelationCompleted(object sender, myRequirements.ServiceReference1.addCategoryAndDesignRelationCompletedEventArgs e)
        {
            NewCategoryIDTextBlock.Visibility = Visibility.Collapsed;
            NewCategoryIDTextBox.Visibility = Visibility.Collapsed;
            NewCategoryOKButton.Visibility = Visibility.Collapsed;

            populateleftList();
        }


        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;
                Button button = sender as Button;

                proxy.removeCategoryAndPatternRelationAsync(Session.Instance.SessionID, Convert.ToInt32(button.Tag), selectedPattern.Pattern.PatternID);
            }
            catch (Exception) {
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_removeCategoryAndPatternRelationCompleted(object sender, myRequirements.ServiceReference1.removeCategoryAndPatternRelationCompletedEventArgs e)
        {
            populateleftList(); 
        }

        private void requirementEditRequirementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((sender as ListBox).SelectedItem != null)
                SelectedElement.Instance.SelectedSRP = ((sender as ListBox).SelectedItem as Requirement).RequirementID;
            
        }

        private void requirementEditCategoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as ListBox).SelectedItem != null)
                SelectedElement.Instance.SelectedTP = ((sender as ListBox).SelectedItem as Categorie).CategorieID;
        }

	}
}