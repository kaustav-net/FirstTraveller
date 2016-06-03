using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.Http;
using Newtonsoft.Json;
using First_Traveller.DetailsJsonClass;
using Microsoft.Phone.Tasks;
using System.Windows.Media;

namespace First_Traveller.Places
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        string typeValue, place_idValue;
        RootObject detailsList;
        Result detailObj;
        bool pageLoadedFlag = false;

        //progress indicator
        ProgressIndicator progressIndicator = new ProgressIndicator()
        {
            IsVisible = true,
            IsIndeterminate = true,
            Text = "Getting details... "
        };

        public DetailsPage()
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

            NavigationContext.QueryString.TryGetValue("place_id", out place_idValue);
            NavigationContext.QueryString.TryGetValue("type", out typeValue);

            typeHeader.Text = typeValue;
            getDetailsResult();
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

        private async void getDetailsResult()
        {
            if (pageLoadedFlag)
            {
                return;
            }


            if (checkNetworkConnection())
            {
                try
                {
                    var client = new HttpClient();
                    var result = await client.GetStringAsync("https://maps.googleapis.com/maps/api/place/details/json?placeid=" + place_idValue + "&key=AIzaSyBeljmyYQYtZr7L1Mq8bz4qpvMUdGl5XPs");
                    detailsList = JsonConvert.DeserializeObject<RootObject>(result);
                    detailObj = detailsList.result;

                    if (detailsList.status == "OK")
                    {
                        ContentPanel.DataContext = detailObj;
                        ContentPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show(detailsList.status);
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
                progressIndicator.IsVisible = false;
                displayError("No network connection available");
            }

        }

        private void reviewClicked(object sender, EventArgs e)
        {
            MarketplaceReviewTask market = new MarketplaceReviewTask();
            market.Show();
        }

        private void displayError(string msg)
        {
            MessageBox.Show(msg);

            ContentPanel.Visibility = Visibility.Collapsed;
            errorMsg.Visibility = Visibility.Visible;

            //if (this.NavigationService.CanGoBack)
            //{
            //    this.NavigationService.GoBack();
            //}
        }
    }
}