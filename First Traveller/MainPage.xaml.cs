using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using First_Traveller.OtherClass;
using System.IO.IsolatedStorage;

namespace First_Traveller
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            reviewReminder();  //method to remind user time to time to review the app

            TiltEffect.SetIsTiltEnabled(currentlocButton, true);
            TiltEffect.SetIsTiltEnabled(customlocButton, true);
        }

        private void CurrentLocation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Places/CurrentLocationPage.xaml", UriKind.Relative));
        }


        private void CustomLocation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Places/CustomLocationPage.xaml", UriKind.Relative));
        }

        /*  appbar button event handlers  */

        //app bar about button click event handler
        private void ApplicationBarAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/OtherPages/AboutPage.xaml", UriKind.Relative));
        }

        //app bar review button click event handler
        private void reviewClicked(object sender, EventArgs e)
        {
            MarketplaceReviewTask market = new MarketplaceReviewTask();
            market.Show();
        }


        //app bar settings button click event handler
        private void settingsClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/OtherPages/SettingsPage.xaml", UriKind.Relative));
        }

        //app bar share button click event handler
        private void shareClicked(object sender, EventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            ShareText shareTextObj = new ShareText();
            string status = shareTextObj.GetSharedText();

            shareStatusTask.Status = status;
            shareStatusTask.Show();
        }

        //app bar help button click event handler
        private void helpClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/OtherPages/HelpPage.xaml", UriKind.Relative));
        }

        //method to remind user time to time to review the app
        private void reviewReminder()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            //set the app name
            string appName = "First Traveller";

            if (!settings.Contains("review"))
            {
                settings.Add("review", 1);
                settings.Add("rcheck", 0);
            }
            else
            {
                int no = Convert.ToInt32(settings["review"]);
                int check = Convert.ToInt32(settings["rcheck"]);
                no++;
                if ((no == 4 || no == 7 || no % 10 == 0) && check == 0)
                {
                    settings["review"] = no;
                    MessageBoxResult mm = MessageBox.Show("Thank you for using " + appName + ".\nWould you like to give some time to rate and review this application to help us improve", appName, MessageBoxButton.OKCancel);
                    if (mm == MessageBoxResult.OK)
                    {
                        settings["rcheck"] = 1;
                        MarketplaceReviewTask rr = new MarketplaceReviewTask();
                        rr.Show();
                    }
                }
                else
                {
                    settings["review"] = no;
                }
            }
        }


    }
}