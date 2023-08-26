/*using Plugin.Media.Abstractions;
using Examen3PSussan.Models;
using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Location = Xamarin.Essentials.Location;
using Path = System.IO.Path;

namespace Examen3PSussan.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Sitio Sitio = null;
        public MapPage(Sitio sitio)
        {
            InitializeComponent();

            Sitio = sitio;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status == PermissionStatus.Granted)
                {
                    var localizacion = await Geolocation.GetLocationAsync();

                    if (localizacion != null)
                    {
                        var pin = new Pin()
                        {
                            Type = PinType.SearchResult,
                            Position = new Position(Sitio.Latitude, Sitio.Longitude),
                            Label = "Descripcion",
                            Address = Sitio.Description
                        };

                        mapa.Pins.Add(pin);
                        //mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(localizacion.Latitude, localizacion.Longitude), Distance.FromMeters(100)));
                        mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Sitio.Latitude, Sitio.Longitude), Distance.FromMeters(100)));
                    }
                }
                else
                {
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Equals("Location services are not enabled on device."))
                {
                    Message("Error", "Servicio de localizacion no encendido");
                }
                else
                {
                    Message("Error", e.Message);
                }
            }
        }
        private async void Message(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        private async void btnNavegar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var location = new Location(Sitio.Latitude, Sitio.Longitude);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                await Xamarin.Essentials.Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                Message("Error", ex.Message);
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}*/