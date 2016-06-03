using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using First_Traveller.JsonClass;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace First_Traveller.Places
{
    public partial class ResultPage : PhoneApplicationPage
    {
        string placeValue, latitudeValue, longitudeValue, addressValue;
        RootObject placesList;
        bool pageLoadedFlag = false;

        //progress indicator
        ProgressIndicator progressIndicator = new ProgressIndicator()
        {
            IsVisible = true,
            IsIndeterminate = true,
            Text = "Getting places... "
        };

        public ResultPage()
        {
            InitializeComponent();
            SystemTray.SetProgressIndicator(this, progressIndicator); //start progress indicator
            this.Loaded += Page_Laoded;
        }

        private void Page_Laoded(object sender, RoutedEventArgs e)
        {
            this.pageLoadedFlag = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationContext.QueryString.TryGetValue("place", out placeValue);
            NavigationContext.QueryString.TryGetValue("lat", out latitudeValue);
            NavigationContext.QueryString.TryGetValue("lon", out longitudeValue);
            //NavigationContext.QueryString.TryGetValue("address", out addressValue);

            nameHeader.Text = placeValue.Replace("_", " ");
            //addressHeader.Text = addressValue;
            getSearchResult();
        }

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

        private async void getSearchResult()
        {
            if(pageLoadedFlag)
            {
                return;
            }


            if (checkNetworkConnection())
            {
                try
                {
                    var client = new HttpClient();
                    var result = await client.GetStringAsync("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + latitudeValue + "," + longitudeValue + "&radius=15000&types=" + placeValue.Replace(" ", "_") + "&rankby=prominence&key=AIzaSyBeljmyYQYtZr7L1Mq8bz4qpvMUdGl5XPs");
                    placesList = JsonConvert.DeserializeObject<RootObject>(result);

                    calculateDistance(placesList.results);
                    placesListView.ItemsSource = placesList.results;

                    if (placesList.status == "ZERO_RESULTS")
                    {
                        displayError("Sorry, can't find a place with your search type! Please search with another place type.");
                    }
                }
                catch (Exception)
                {
                    displayError("We're having trouble getting data.");
                }
                finally
                {
                    progressIndicator.IsVisible = false;
                }
            }
            else
            {
                displayError("No Network Connection Available");
                progressIndicator.IsVisible = false;
            }
            
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Result rslt = ((Grid)sender).DataContext as Result;
            string place_id = rslt.place_id;

            NavigationService.Navigate(new Uri("/Places/DetailsPage.xaml?place_id=" + place_id + "&type=" + placeValue, UriKind.Relative));
            

        }
        private void reviewClicked(object sender, EventArgs e)
        {
            MarketplaceReviewTask market = new MarketplaceReviewTask();
            market.Show();
        }

        private void calculateDistance(List<Result> rsltList)
        {
            GeoCoordinate coordinate1 = new GeoCoordinate(Convert.ToDouble(latitudeValue), Convert.ToDouble(longitudeValue));

            foreach(Result rsltObj in rsltList)
            {
                double rsltLat = rsltObj.geometry.location.lat;
                double rsltLon = rsltObj.geometry.location.lng;

                GeoCoordinate coordinate2 = new GeoCoordinate(rsltLat, rsltLon);

                double distance = coordinate1.GetDistanceTo(coordinate2);

                rsltObj.distance = String.Format("{0:0.##}", distance*0.001) + " km";
            }
        }

        private void displayError(string msg)
        {
            MessageBox.Show(msg);

            placesListView.Visibility = Visibility.Collapsed;
            errorMsg.Visibility = Visibility.Visible;

            //if (this.NavigationService.CanGoBack)
            //{
            //    this.NavigationService.GoBack();
            //}
        }

    }
}