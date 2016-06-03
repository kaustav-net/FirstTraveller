using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using First_Traveller.OtherClass;
using Microsoft.Phone.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace First_Traveller.Places
{
    public partial class CustomLocationPage : PhoneApplicationPage
    {
        /*  instance variables  */

        double latitude, longitude;  //latitude, longitude value of current location
        string address;
        //RootObject customLocation;

        //progress indicator
        ProgressIndicator progressIndicator = new ProgressIndicator()
        {
            IsVisible = true,
            IsIndeterminate = true,
            Text = "Getting current location... "
        };

        //constructor
        public CustomLocationPage()
        {
            InitializeComponent();

            setPlaceType();

            TiltEffect.SetIsTiltEnabled(searchButton, true);
        }


        //private async void geocodeAddress(string address)
        //{
        //    string formattedAddress = address.Replace(' ', '+');

        //    try
        //    {
        //        var client = new HttpClient();
        //        var result = await client.GetStringAsync("https://maps.googleapis.com/maps/api/geocode/json?address=" + formattedAddress + "&key=AIzaSyBeljmyYQYtZr7L1Mq8bz4qpvMUdGl5XPs");
        //        customLocation = JsonConvert.DeserializeObject<RootObject>(result);

        //        if (customLocation.status == "OVER_QUERY_LIMIT" || customLocation.status == "UNKNOWN_ERROR" || customLocation.status == "REQUEST_DENIED")
        //        {
        //            displayError("We're having trouble getting data.");
        //        }

        //        if (customLocation.status == "ZERO_RESULTS")
        //        {
        //            displayError("Sorry, didn't get the address! Please try with another address.");
        //        }

        //        else
        //        {
        //            List<Result> currentLocList = customLocation.results;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        displayError("We're having trouble getting data.");
        //    }

        //    progressIndicator.IsVisible = false;
        //}


        //search button event handler
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            //geocodeAddress(customLoc.Text);

            if(progressIndicator.IsVisible == true)
            {
                MessageBox.Show("Please wait! We're checking the address!");
            }

            if (latitude != 0 && longitude != 0) //if latitude and longitude values are not 0 then navigate to results page
            {
                NavigationService.Navigate(new Uri("/Places/ResultPage.xaml?place=" + placeSelection.SelectedItem + "&lat=" + latitude + "&lon=" + longitude, UriKind.Relative));
            }
        }



        /*  checking methods  */

        //check network connection
        private bool checkNetworkConnection()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /*  formatting info  */

        //set the place type listpicker
        private void setPlaceType()
        {
            IEnumerable<string> typeList = (new Place()).place;
            placeSelection.ItemsSource = typeList;
        }



        /*  appbar button event handlers  */


        //app bar review button click event handler
        private void reviewClicked(object sender, EventArgs e)
        {
            MarketplaceReviewTask market = new MarketplaceReviewTask();
            market.Show();
        }

        //app bar help button click event handler
        private void helpClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/OtherPages/HelpPage.xaml", UriKind.Relative));
        }



        /*  Utility methods  */


        //dispay error
        private void displayError(string msg, bool shomMsg = true)
        {
            if (shomMsg)  // check if wanted to display messagebox
            {
                MessageBox.Show(msg);
            }

            latitude = longitude = 0;  //set latitude and longitude value to 0
        }
    }
}