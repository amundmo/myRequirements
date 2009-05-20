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
    public class Session : INotifyPropertyChanged
    {
        private static Session instance;
        private ServiceReference1.WebServiceClient proxy;
        private ServiceReference1.User userdata;

        public Session()
        {
            proxy = new ServiceReference1.WebServiceClient();
            SessionID = "";
        }

        public ServiceReference1.User Userdata 
        {
            set
            {
                userdata = value;
                Notify("Userdata");
            }
            get { return userdata; }
        
        }

        public static Session Instance
        {
            get
            {
                if (instance == null)
                    instance = new Session();

                return instance;
            }
        }

        public ServiceReference1.WebServiceClient getWebServiceConnection()
        {
            return proxy;
        }


        public String SessionID { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;
        void Notify(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}


public class Singleton
{
   private static Singleton instance;

   private Singleton() {}

   public static Singleton Instance
   {
      get 
      {
         if (instance == null)
         {
            instance = new Singleton();
         }
          
          return instance;
      }
   }

   public String SessionID { set; get; }
}
