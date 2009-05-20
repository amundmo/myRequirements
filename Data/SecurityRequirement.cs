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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace myRequirements
{
    public class SecurityRequirement : INotifyPropertyChanged, ISearchableObject
    {
        private String name;
        private String aliases;
        private String abstractInformation;
        private String problem;
        private String commonAttacks;
        private String solution;
        private String issues;
        private String examples;
        private String tradeoffs;
        private String relatedPatterns;
        private String references;

        public SecurityRequirement(){
            Examples = new ObservableCollection<string>();
        }
        public int id { set; get; }
        
        public String Name
        {
            set
            {
                name = value;
                Notify("Name");
            }
            get { return name; }
        }

        public String AbstractText
        {
            set
            {
                abstractInformation = value;
                Notify("AbstractText");
            }
            get { return abstractInformation; }
        }


        public ObservableCollection<String> Examples { set; get; }


        public event PropertyChangedEventHandler PropertyChanged;
        void Notify(string propName)
        {
        if (PropertyChanged != null )
            PropertyChanged(this, new PropertyChangedEventArgs(propName));

        }

    }
}
