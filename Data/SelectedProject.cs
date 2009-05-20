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
    public class SelectedProject : INotifyPropertyChanged
    {
        private static SelectedProject instance;
        private String name;
        
        
        public ServiceReference1.Project Project { set; get; }

        //To get a notification when the name changes...
        public String Name
        {
            set
            {
                name = value;
                Notify("Name");
            }
            get { return name; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void Notify(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public static SelectedProject Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectedProject();

                return instance;
            }
        }
    }
 
}
