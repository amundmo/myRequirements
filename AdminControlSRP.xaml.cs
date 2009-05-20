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
	public partial class AdminControlSRP : UserControl
	{
        private ServiceReference1.WebServiceClient proxy;
        private ServiceReference1.Requirment_PatternsAndCategoriesAndExamples selectedSecurityRequirement;

		public AdminControlSRP()
		{
			// Required to initialize variables
			InitializeComponent();
            proxy = Session.Instance.getWebServiceConnection();

            NewSDPIDText.Visibility = Visibility.Collapsed;
            NewSDPIDTextBox.Visibility = Visibility.Collapsed;
            NewSDPOKButton.Visibility = Visibility.Collapsed;

            NewCategoryOKButton.Visibility = Visibility.Collapsed;
            NewCategoryIDTextBox.Visibility = Visibility.Collapsed;
            NewCategoryIDTextBlock.Visibility = Visibility.Collapsed;

            ProgressBarRight.Visibility = Visibility.Visible;

            proxy.GetAllSRPCompleted += new EventHandler<myRequirements.ServiceReference1.GetAllSRPCompletedEventArgs>(proxy_GetAllSRPCompleted);
            proxy.UpdateSRPCompleted += new EventHandler<myRequirements.ServiceReference1.UpdateSRPCompletedEventArgs>(proxy_UpdateSRPCompleted);
            questionInfoMessgeBox.okInfoMessageButotn.Click += new RoutedEventHandler(okInfoMessageButton_Click);
            questionInfoMessgeBox.cancelInfoMessageButton.Click += new RoutedEventHandler(cancelInfoMessageButton_Click);
            proxy.DeleteSRPCompleted += new EventHandler<myRequirements.ServiceReference1.DeleteSRPCompletedEventArgs>(proxy_DeleteSRPCompleted);
            proxy.addPatternAndRequirementRelationCompleted += new EventHandler<myRequirements.ServiceReference1.addPatternAndRequirementRelationCompletedEventArgs>(proxy_addPatternAndRequirementRelationCompleted);
            proxy.removePatternandRequirementRelationCompleted += new EventHandler<myRequirements.ServiceReference1.removePatternandRequirementRelationCompletedEventArgs>(proxy_removePatternandRequirementRelationCompleted);
            proxy.addCategoryAndRequirementRelationCompleted += new EventHandler<myRequirements.ServiceReference1.addCategoryAndRequirementRelationCompletedEventArgs>(proxy_addCategoryAndRequirementRelationCompleted);
            proxy.addNewRequirementExampleByRequirmentIDCompleted += new EventHandler<myRequirements.ServiceReference1.addNewRequirementExampleByRequirmentIDCompletedEventArgs>(proxy_addNewRequirementExampleByRequirmentIDCompleted);
            proxy.removeRequirementExampleByRequirementIDCompleted += new EventHandler<myRequirements.ServiceReference1.removeRequirementExampleByRequirementIDCompletedEventArgs>(proxy_removeRequirementExampleByRequirementIDCompleted);
		}



        public void populateRequirementList()
        {
            ProgressBarRight.Visibility = Visibility.Visible;
            
            proxy.GetAllSRPAsync(Session.Instance.SessionID);
        }

        void proxy_GetAllSRPCompleted(object sender, myRequirements.ServiceReference1.GetAllSRPCompletedEventArgs e)
        {
            try
            {
                requirementList.ItemsSource = e.Result;

                for (int i = 0; i < requirementList.Items.Count; i++)
                {
                    if (((Requirment_PatternsAndCategoriesAndExamples)requirementList.Items[i]).Requirement.RequirementID == SelectedElement.Instance.SelectedSRP)
                    {
                        requirementList.SelectedIndex = i;
                        requirementList.SelectedItem = requirementList.Items[i];
                    }
                }
            }
            catch (Exception)
            {
            }

            //if (e.Result.Count > 0) //Fixes the cancel button :)
            //    requirementList.SelectedIndex = 0;

            ProgressBarRight.Visibility = Visibility.Collapsed;
        }


        private void CreateNewSRP(object sender, RoutedEventArgs e)
        {

            ProgressBarRight.Visibility = Visibility.Visible;
            proxy.CreateNewSRPCompleted += new EventHandler<myRequirements.ServiceReference1.CreateNewSRPCompletedEventArgs>(proxy_CreateNewSRPCompleted);

            proxy.CreateNewSRPAsync(Session.Instance.SessionID, new ServiceReference1.Requirement());

        }

        void proxy_CreateNewSRPCompleted(object sender, myRequirements.ServiceReference1.CreateNewSRPCompletedEventArgs e)
        {
            populateRequirementList();
        }

        private void requirementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedItem != null)
            {
                selectedSecurityRequirement = (ServiceReference1.Requirment_PatternsAndCategoriesAndExamples)lb.SelectedItem;
                SelectedElement.Instance.SelectedSRP = selectedSecurityRequirement.Requirement.RequirementID;

                if (selectedSecurityRequirement != null)
                {

                    //Update category and pattern list
                    //updatePatternsAndCategories();


                    requirementEditID.DataContext = selectedSecurityRequirement.Requirement;
                    requirementEditName.DataContext = selectedSecurityRequirement.Requirement;
                    requirementEditAliases.DataContext = selectedSecurityRequirement.Requirement;
                    requirementEditDescription.DataContext = selectedSecurityRequirement.Requirement;
                    requirementEditProblem.DataContext = selectedSecurityRequirement.Requirement;
                    requirementEditSolution.DataContext = selectedSecurityRequirement.Requirement;
                    requirementEditCategoriesList.ItemsSource = selectedSecurityRequirement.Categories;
                    requirementEditSDPList.ItemsSource = selectedSecurityRequirement.Patterns;
                    requirementEditExamplesList.ItemsSource = selectedSecurityRequirement.Examples;
                    requirementEditReferences.DataContext = selectedSecurityRequirement.Requirement;
                    requirementEditDomain.DataContext = selectedSecurityRequirement.Requirement;
                }
            }
        }

        //private void updatePatternsAndCategories()
        //{

        //    //proxy.getPatternsAndCategoriesByRequirmentCompleted += new EventHandler<myRequirements.ServiceReference1.getPatternsAndCategoriesAndExamplesByRequirmentCompletedEventArgs>(proxy_getPatternsAndCategoriesByRequirmentCompleted);
        //    //proxy.getPatternsAndCategoriesByRequirmentAsync(selectedSecurityRequirement);
        //    ProgressBarRight.Visibility = Visibility.Visible;
        //}

        //void proxy_getPatternsAndCategoriesByRequirmentCompleted(object sender, myRequirements.ServiceReference1.getPatternsAndCategoriesAndExamplesByRequirmentCompletedEventArgs e)
        //{
        //    requirementEditCategoriesList.ItemsSource = e.Result.Categories;
        //    requirementEditSDPList.ItemsSource = e.Result.Patterns;

        //    ProgressBarRight.Visibility = Visibility.Collapsed;
        //}

        private void SaveRequirement(object sender, RoutedEventArgs e)
        {
            updateSelectedRequirement();
        }


        private void updateSelectedRequirement()
        {
            ProgressBarRight.Visibility = Visibility.Visible;
            proxy.UpdateSRPAsync(Session.Instance.SessionID, selectedSecurityRequirement);
        }

        void proxy_UpdateSRPCompleted(object sender, myRequirements.ServiceReference1.UpdateSRPCompletedEventArgs e)
        {
            if (!e.Result.Equals("Security Requirement successfully"))
            {
                //Do something!
            }
       
            ProgressBarRight.Visibility = Visibility.Collapsed;
        }

        private void CancelEditRequirement(object sender, RoutedEventArgs e)
        {
            ProgressBarRight.Visibility = Visibility.Visible;
            populateRequirementList();
        }

        private void DeleteRequirement(object sender, RoutedEventArgs e)
        {
            questionInfoMessgeBox.Visibility = Visibility.Visible;
            questionInfoMessgeBox.header.Text = "Are you sure?";
            questionInfoMessgeBox.message.Text = "Are you sure you want to delete the selected security requirement?";

        }

        void cancelInfoMessageButton_Click(object sender, RoutedEventArgs e)
        {
            questionInfoMessgeBox.Visibility = Visibility.Collapsed;
        }

        void okInfoMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectedSecurityRequirement != null){
                questionInfoMessgeBox.Visibility = Visibility.Collapsed;
                ProgressBarRight.Visibility = Visibility.Visible;
                
                proxy.DeleteSRPAsync(Session.Instance.SessionID, selectedSecurityRequirement.Requirement);
            }
            else{
                //Some message to user
            }

        }

        void proxy_DeleteSRPCompleted(object sender, myRequirements.ServiceReference1.DeleteSRPCompletedEventArgs e)
        {
            //Check result
            populateRequirementList();
        }

        private void NewSDPClick(object sender, RoutedEventArgs e)
        {
            NewSDPIDTextBox.Text = "";

            if (NewSDPIDText.Visibility == Visibility.Visible)
            {
                NewSDPIDText.Visibility = Visibility.Collapsed;
                NewSDPIDTextBox.Visibility = Visibility.Collapsed;
                NewSDPOKButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                NewSDPIDText.Visibility = Visibility.Visible;
                NewSDPIDTextBox.Visibility = Visibility.Visible;
                NewSDPOKButton.Visibility = Visibility.Visible;
            }

        }

        private void NewSDPOKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;

                proxy.addPatternAndRequirementRelationAsync(Session.Instance.SessionID, selectedSecurityRequirement.Requirement.RequirementID, Convert.ToInt32(NewSDPIDTextBox.Text));
            }
            catch (Exception)
            {
                NewSDPIDText.Visibility = Visibility.Collapsed;
                NewSDPIDTextBox.Visibility = Visibility.Collapsed;
                NewSDPOKButton.Visibility = Visibility.Collapsed;
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_addPatternAndRequirementRelationCompleted(object sender, myRequirements.ServiceReference1.addPatternAndRequirementRelationCompletedEventArgs e)
        {
            NewSDPIDText.Visibility = Visibility.Collapsed;
            NewSDPIDTextBox.Visibility = Visibility.Collapsed;
            NewSDPOKButton.Visibility = Visibility.Collapsed;

            populateRequirementList();
        }


        private void DeleteSDP(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;
                Button button = sender as Button;

                proxy.removePatternandRequirementRelationAsync(Session.Instance.SessionID, Convert.ToInt32(button.Tag), selectedSecurityRequirement.Requirement.RequirementID);
 
            }
            catch (Exception)
            {
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
            
        }

        void proxy_removePatternandRequirementRelationCompleted(object sender, myRequirements.ServiceReference1.removePatternandRequirementRelationCompletedEventArgs e)
        {
            populateRequirementList();
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
                proxy.addCategoryAndRequirementRelationAsync(Session.Instance.SessionID, selectedSecurityRequirement.Requirement.RequirementID, Convert.ToInt32(NewCategoryIDTextBox.Text));
            }
            catch (Exception)
            {
                NewCategoryIDTextBlock.Visibility = Visibility.Collapsed;
                NewCategoryIDTextBox.Visibility = Visibility.Collapsed;
                NewCategoryOKButton.Visibility = Visibility.Collapsed;
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_addCategoryAndRequirementRelationCompleted(object sender, myRequirements.ServiceReference1.addCategoryAndRequirementRelationCompletedEventArgs e)
        {

            NewCategoryIDTextBlock.Visibility = Visibility.Collapsed;
            NewCategoryIDTextBox.Visibility = Visibility.Collapsed;
            NewCategoryOKButton.Visibility = Visibility.Collapsed;

            populateRequirementList();
        }

        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarRight.Visibility = Visibility.Visible;
                Button button = sender as Button;

                foreach (ServiceReference1.RelationCategorieRequirement rp in selectedSecurityRequirement.Requirement.RelationCategorieRequirements)
                {
                    if (rp.CategorieID == Convert.ToInt32(button.Tag))
                    {
                        selectedSecurityRequirement.Requirement.RelationCategorieRequirements.Remove(rp);
                        break;
                    }
                }
                updateSelectedRequirement();
                populateRequirementList();

                //proxy.removeCategoryAndRequirementRelationCompleted += new EventHandler<myRequirements.ServiceReference1.removeCategoryAndRequirementRelationCompletedEventArgs>(proxy_removeCategoryAndRequirementRelationCompleted);
                //proxy.removeCategoryAndRequirementRelationAsync(Session.Instance.SessionID, Convert.ToInt32(button.Tag), selectedSecurityRequirement.Requirement.RequirementID);
            }
            catch (Exception) {
                ProgressBarRight.Visibility = Visibility.Collapsed;
            }
        }

        void proxy_removeCategoryAndRequirementRelationCompleted(object sender, myRequirements.ServiceReference1.removeCategoryAndRequirementRelationCompletedEventArgs e)
        {
            populateRequirementList();
        }


        private void newExampleClick(object sender, RoutedEventArgs e)
        {
            if (selectedSecurityRequirement != null)
            {
                ProgressBarRight.Visibility = Visibility.Visible;

                proxy.addNewRequirementExampleByRequirmentIDAsync(Session.Instance.SessionID, selectedSecurityRequirement.Requirement.RequirementID);
            }
            else
            {
                //Some message to user...
            }
        }

        void proxy_addNewRequirementExampleByRequirmentIDCompleted(object sender, myRequirements.ServiceReference1.addNewRequirementExampleByRequirmentIDCompletedEventArgs e)
        {
            populateRequirementList();
        }

        private void DeleteExampleClick(object sender, RoutedEventArgs e)
        {
            try 
	        {	        
                Button button = sender as Button;
                ProgressBarRight.Visibility = Visibility.Visible;

                proxy.removeRequirementExampleByRequirementIDAsync(Session.Instance.SessionID, selectedSecurityRequirement.Requirement.RequirementID, Convert.ToInt32(button.Tag));
	        }
	        catch (Exception)
	        {

                ProgressBarRight.Visibility = Visibility.Collapsed;
	        }

        }

        void proxy_removeRequirementExampleByRequirementIDCompleted(object sender, myRequirements.ServiceReference1.removeRequirementExampleByRequirementIDCompletedEventArgs e)
        {
            populateRequirementList();
        }

        private void requirementEditCategoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
                SelectedElement.Instance.SelectedTP = ((sender as ListBox).SelectedItem as Categorie).CategorieID;
        }

        private void requirementEditSDPList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
                SelectedElement.Instance.SelectedSDP = ((sender as ListBox).SelectedItem as Pattern).PatternID;
        }
  	}
}