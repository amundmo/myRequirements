using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace myRequirements.Data
{
    public class SelectedElement : INotifyPropertyChanged
    {
        private static SelectedElement instance;
        
        
        public ServiceReference1.Project Project { set; get; }
        private int selectedSDP;
        private int selectedSRP;
        private int selectedTP;

        public int SelectedSDP
        {
            set
            {
                selectedSDP = value;
                Notify("selectedSDP");
            }
            get { return selectedSDP; }
        }

        public int SelectedSRP
        {
            set
            {
                selectedSRP = value;
                Notify("selectedSRP");
            }
            get { return selectedSRP; }
        }

        public int SelectedTP
        {
            set
            {
                selectedTP = value;
                Notify("selectedTP");
            }
            get { return selectedTP; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void Notify(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public static SelectedElement Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectedElement();

                return instance;
            }
        }
    }
}
