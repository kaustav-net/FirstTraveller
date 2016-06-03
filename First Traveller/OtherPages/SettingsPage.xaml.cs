using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace First_Traveller.OtherPages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent") || (bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"])
            {
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;  //as default is true
                IsolatedStorageSettings.ApplicationSettings.Save();

                locationSwitch.IsChecked = true;
                warningText.Visibility = Visibility.Collapsed;
            }
            else
            {
                locationSwitch.IsChecked = false;
                warningText.Visibility = Visibility.Visible;
            }
        } 

        private void Switch_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
            IsolatedStorageSettings.ApplicationSettings.Save();

            warningText.Visibility = Visibility.Collapsed;
        }

        private void Switch_Unchecked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
            IsolatedStorageSettings.ApplicationSettings.Save();

            warningText.Visibility = Visibility.Visible;
        }

    }
}