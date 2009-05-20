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

namespace myRequirements.Data
{
    public class SearchableData : INotifyPropertyChanged
    {
        public enum types { requirement, pattern, threatCategory, privateRequirement };
        public String selectedRequirementText;

        public int ID { set; get; }
        public String Name { set; get; }
        public String Aliases { set; get; }
        public String AbstractText { set; get; }
        public String CommonAttacks { set; get; }
        public String Issues { set; get; }
        public int ObjectType { set; get; }
        public String Description { set; get; }
        public String Problem { set; get; }
        public String Solution { set; get; }
        public bool isSelectedAsRequirement { set; get; }
        public bool SuggestAsPublic { set; get; }
        public int selectedExample { set; get; }
        public int Priority { set; get; }
        public String PriorityText { set; get; }
        public String Domain { set; get; }
        public String References { set; get; }
        public SearchableData Parent { set; get; }

        public String SelectedRequirementText{
            set
            {
                selectedRequirementText = value;
                Notify("SelectedRequirementText");
            }
            get { return selectedRequirementText; }
        }

        public ObservableCollection<ServiceReference1.Example> Examples { set; get; }



        public event PropertyChangedEventHandler PropertyChanged;
        void Notify(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }



        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
