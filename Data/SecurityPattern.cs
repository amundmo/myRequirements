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

namespace myRequirements.Data
{
    public class SecurityPattern : ISearchableObject
    {

        #region ISearchableObject Members

        public string Name
        {
            get;
            set;
        }

        public string AbstractText
        {
            get;
            set;
        }

        #endregion
    }
}
