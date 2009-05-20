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

namespace myRequirements
{
    public partial class Page : UserControl{

        private PrivateSecurityRequirements privSecReq;
        private SecurityRequirement selectedSecurityRequirement;

		public Page()
		{
			// Required to initialize variables
			InitializeComponent();

            initiateDummyData();
		}

        private void initiateDummyData()
        {
            privSecReq = new PrivateSecurityRequirements();


            SecurityRequirement secReq1 = new SecurityRequirement();
            SecurityRequirement secReq2 = new SecurityRequirement();
            
            secReq1.Name = "The need-to-know principle can be enforced with user access controls and authorisation procedures.";
            secReq1.AbstractInformation = "User access and authorisation provides protection against unauthorised access through user identification, authorisation and restricting access to information and functions needed by staff members to undertake their duties.";
 
            secReq1.Examples.Add("Another example 1");
            secReq1.Examples.Add("Another example 2 - with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text");

            secReq2.Name = "Access to an agency’s system can be controlled by insisting on an authentication procedure to establish, with some minimum degree of confidence, the identity of the system user.";
            secReq2.AbstractInformation = "Implementing password selection policies and password management practices prevents system users and systems from having authentication information easily subverted by brute force attacks against weak authenticators.";

            secReq2.Examples.Add("Another example 1");
            secReq2.Examples.Add("Another example 2 - with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text, with loooooooooooooooooong text");

            privSecReq.privateSecurityRequirements = new ObservableCollection<SecurityRequirement>();
            privSecReq.privateSecurityRequirements.Add(secReq1);
            privSecReq.privateSecurityRequirements.Add(secReq2);

            mySecurityRequirementListBox.ItemsSource = privSecReq.privateSecurityRequirements;
            

        }

   

        private void Button_Click_CreateNewRequirement(object sender, RoutedEventArgs e)
        {
            privSecReq.privateSecurityRequirements.Add(new SecurityRequirement { Name = "New private security requirement", AbstractInformation = "" });
            
        }

        private void mySecurityRequirementListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            selectedSecurityRequirement = (SecurityRequirement)lb.SelectedItem;

            //requirementName.DataContext = selectedSecurityRequirement;
            requirementAbstract.RichText = selectedSecurityRequirement.AbstractInformation;
            requirementInfoExamples.ItemsSource = selectedSecurityRequirement.Examples;
        }

        private void requirementInfoExamples_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            String selectedExample = (String)lb.SelectedItem;
            Boolean namealreadyAdded = false;

            if(selectedExample != null){

                foreach (String example in selectedSecurityRequirement.Examples)
                {
                    if (example.Equals(selectedSecurityRequirement.Name))
                    {
                        namealreadyAdded = true;
                    }

                }
                if (!namealreadyAdded)
                    selectedSecurityRequirement.Examples.Add(selectedSecurityRequirement.Name);

                selectedSecurityRequirement.Name = selectedExample;
            }

        }
	}
}
