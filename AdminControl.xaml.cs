using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace myRequirements
{
	public partial class AdminControl : UserControl
	{
        private AdminControlSDP sdp;
        private AdminControlSRP srp;
        private AdminControlTP tp;
        private AdminControlUserManager umanager;


        public AdminControl()
		{
            sdp = new AdminControlSDP();
            srp = new AdminControlSRP();
            tp = new AdminControlTP();
            umanager = new AdminControlUserManager();

			InitializeComponent();


            AddSecurityRequirementPattern.Content = srp;
            AddSecurityDesignPattern.Content = sdp;
            AddThreatPattern.Content = tp;
            UserManager.Content = umanager;

		}

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl cntr = sender as TabControl;
            TabItem tabItem = cntr.SelectedItem as TabItem;

            if (tabItem.Header.Equals("Security Design Patterns"))
                sdp.populateleftList();
            else if(tabItem.Header.Equals("Security Requirement Patterns"))
                srp.populateRequirementList();
            else if(tabItem.Header.Equals("Threat patterns"))
                tp.populateleftList();
            else if(tabItem.Header.Equals("User Manager"))
                umanager.updateUsersFromProxy();
          

           
        }
	}
}