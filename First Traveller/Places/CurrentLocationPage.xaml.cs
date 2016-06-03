using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using First_Traveller.Resources;
using First_Traveller.OtherClass;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Net.Http;
using First_Traveller.ReverseGeocodingJsonClass;
using Newtonsoft.Json;

namespace First_Traveller.Places
{
    public partial class CurrentLocationPage : PhoneApplicationPage
    {

        /*  instance variables  */

        double latitude, longitude;  //latitude, longitude value of current location
        RootObject downloadedLocation;
        string address;

        //progress indicator
        ProgressIndicator progressIndicator = new ProgressIndicator()
        {
            IsVisible = true,
            IsIndeterminate = true,
            Text = "Getting current location... "
        };



        /*  page's predefined methods  */

        // Constructor
        public CurrentLocationPage()
        {
            InitializeComponent();

            startAccess();  //call to access the users current location

            setPlaceType(); //set the list picker with the place types 

            this.Loaded += pageLoaded;  //page loaded event handler

            TiltEffect.SetIsTiltEnabled(searchButton, true);
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
        }

        //on navigated to
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!checkAppLocation())//call to check if app location settings is turned on or not
            {
                displayError("Please turn on location access in app settings to use the app!", false);
            }
            if (!checkPhoneLocationSettings())//call to check if phone location settings is turned on or not
            {
                displayError("Location is disabled in phone settings. Please turn on location access to use the app.", false);
            }
        }



        /*  doing main work  */

        //access the users current location with error checking
        private void startAccess()
        {
            if (checkAppLocation() && checkNetworkConnection() && checkPhoneLocationSettings())  //call to check if app location settings is turned on or not
            {
                SystemTray.SetProgressIndicator(this, progressIndicator); //start progress indicator
                progressIndicator.IsVisible = true; //display progress indicator

                setLocation(); //call to set the location
            }
            else if (!checkAppLocation())
            {
                //error
                displayError("Please turn on location access in app settings to use the app!");
            }
            else if (!checkNetworkConnection())
            {
                //error
                displayError("No Network Connection Available");
            }
            else if (!checkPhoneLocationSettings())
            {
                //error
                displayError("Location is disabled in phone settings. Please turn on location access to use the app.");
            }

        }

        //set the location
        private async void setLocation()
        {
            Geolocator geoLocator = new Geolocator();
            try
            {
                Geoposition geoloc = await geoLocator.GetGeopositionAsync(   //getting the current location
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                //set the latitude and longitude value of current location
                latitude = geoloc.Coordinate.Latitude;
                longitude = geoloc.Coordinate.Longitude;

                //call to set the current location's address
                setAddress();
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    displayError("Please turn on location access in app settings to use the app!");

                    progressIndicator.IsVisible = false;
                }
                else
                {
                    displayError("We're having trouble getting data.");

                    progressIndicator.IsVisible = false;
                }
            }

        }

        //set the current location's address
        private async void setAddress()
        {
            try
            {
                var client = new HttpClient();
                var result = await client.GetStringAsync("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyBeljmyYQYtZr7L1Mq8bz4qpvMUdGl5XPs");
                downloadedLocation = JsonConvert.DeserializeObject<RootObject>(result);

                if (downloadedLocation.status == "ZERO_RESULTS" || downloadedLocation.status == "OVER_QUERY_LIMIT" || downloadedLocation.status == "UNKNOWN_ERROR")
                {
                    displayError("Sorry, didn't get your location!");
                }
                else
                {
                    List<Result> currentLocList = downloadedLocation.results;

                    address = ""; //reset address
                    address = currentLocList[0].formatted_address;

                    currentLoc.Text = address; //set the new address
                }
            }
            catch (Exception)
            {
                displayError("We're having trouble getting data.");
            }

            progressIndicator.IsVisible = false;

            //ReverseGeocodeQuery query = new ReverseGeocodeQuery();
            //query.GeoCoordinate = new GeoCoordinate(latitude, longitude);

            //query.QueryAsync();
            //query.QueryCompleted += (s, e) =>
            //{
            //    if (e.Error != null)
            //    {
            //        displayError("We're having trouble getting data.");

            //        progressIndicator.IsVisible = false;
            //        return;
            //    }

            //    try
            //    {
            //        street = e.Result[0].Information.Address.Street;
            //        city = e.Result[0].Information.Address.City;
            //        state = e.Result[0].Information.Address.State;
            //        country = e.Result[0].Information.Address.Country;
            //        postalCode = e.Result[0].Information.Address.PostalCode;

            //        address = "";

            //        formatAddress();

            //        currentLoc.Text = address;
            //    }
            //    catch(Exception ex)
            //    {
            //        displayError("We're having trouble getting data.");
            //    }

            //    progressIndicator.IsVisible = false;
            //};

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

        //check phone location settings
        private bool checkPhoneLocationSettings()
        {
            Geolocator geo = new Geolocator();

            if (geo.LocationStatus == PositionStatus.Disabled)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //check if app location settings is turned on or not
        private bool checkAppLocation()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent") && !(bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"])
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        /*  event handlers  */

        //search buton event handler
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (latitude != 0 && longitude != 0) //if latitude and longitude values are not 0 then navigate to results page
            {
                NavigationService.Navigate(new Uri("/Places/ResultPage.xaml?place=" + placeSelection.SelectedItem + "&lat=" + latitude + "&lon=" + longitude, UriKind.Relative));
            }
            else if (!checkPhoneLocationSettings()) //check phone location settings
            {
                MessageBox.Show("Location is disabled in phone settings. Please turn on location access to use the app.");
                return;
            }
            else if (!checkAppLocation()) //check app location settings
            {
                MessageBox.Show("Please turn on location access in app settings to use the app!");
                return;
            }
            else if (!checkNetworkConnection()) //check internet connection
            {
                MessageBox.Show("No Network Connection Available");
                return;
            }
            else
            {
                MessageBox.Show("Cannot search without your location!");
                return;
            }
        }


        //refresh button tap event handler
        private void Refresh_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            startAccess(); //get the current location
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

            currentLoc.Text = "Error";  //display error in current location textblock

            latitude = longitude = 0;  //set latitude and longitude value to 0
        }



    }
}