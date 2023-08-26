using Acr.UserDialogs;
using Plugin.AudioRecorder;
using Examen3PSussan.Views;
using Examen3PSussan.Controller;
using Examen3PSussan.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Examen3PSussan.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListSite : ContentPage
    {
        public Sitio Site;
        bool editando = false;

        private readonly AudioPlayer audioPlayer = new AudioPlayer();

        public ListSite()
        {
            InitializeComponent();

            LoadData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (editando)
            {
                LoadData();

                editando = false;

                Site = null;
            }
        }


        private void listSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                Site = e.Item as Sitio;
            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }

        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Site == null)
                {
                    Message("Aviso", "Seleccione un sitio");
                    return;
                }

                
                    DeleteSite(Site);
               
            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }
        }

        private async void btnUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Site == null)
                {
                    Message("Aviso", "Seleccione un sitio");
                    return;
                }

                editando = true;
                await Navigation.PushModalAsync(new UpdateSite(Site));
            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }
        }

        private async void DeleteSite(Sitio site)
        {
            var status = await DisplayAlert("Aviso", $"¿Desea eliminar la nota: {Site.Description}?", "SI", "NO");

            if (status)
            {
                var result = await SitioController.DeleteSite(Site.Id);

                if (result)
                {
                    Message("Aviso", "El sitio fue eliminado correctamente");
                    Site = null;
                    LoadData();

                    Site = null;
                }
                else
                {
                    Message("Aviso", "No se pudo eliminar el sitio");
                }
            }
        }

        private async void btnViewMapa_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Site contiene toda la data del item seleccionado
                if (Site == null)
                {
                    Message("Aviso", "Seleccione un sitio");
                    return;
                }

                var status = await DisplayAlert("Aviso", $"¿Desea ir a la ubicacion indicada?", "SI", "NO");

                if (status)
                {
                    /*await Navigation.PushModalAsync(new MapPage(Site));*/
                }

            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }
        }


        private async void LoadData()
        {
            try
            {
                await Task.Delay(1000);

                UserDialogs.Instance.ShowLoading("Cargando", MaskType.Gradient);

                var sitios = await SitioController.GetAllSite();
                listSites.ItemsSource = sitios;

                await Task.Delay(500);
                UserDialogs.Instance.HideLoading();

                var current = Connectivity.NetworkAccess;

                if (current != NetworkAccess.Internet)
                {
                    Message("Advertencia", "Actualmente no cuenta con acceso a internet");
                    return;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await Task.Delay(500);

                Message("Error: ", ex.Message);
            }
        }


        #region Metodos Utiles


        private async void Message(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }


        #endregion Metodos Utiles

        private void listeAudio(byte[] bytes)
        {

            var folderPath = "/storage/emulated/0/Android/data/com.companyname.Examen3parcialSussan/files/Audio";

            var nameAudio = "temp.wav";

            var fullPath = folderPath + "/" + nameAudio;

            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                Stream stream = new MemoryStream(bytes);

                using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }

                audioPlayer.Play(fullPath);
            }
            catch (Exception ex)
            {

                Message("Error: ", ex.Message);
            }

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        private void btnViewListen_Clicked(object sender, EventArgs e)
        {

            if (Site == null)
            {
                Message("Aviso", "Seleccione un sitio");
                return;
            }

            try
            {
                listeAudio(Site.AudioFile);
            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }
        }
    }
}