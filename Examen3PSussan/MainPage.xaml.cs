using Acr.UserDialogs;
using Plugin.AudioRecorder;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Examen3PSussan.Controller;
using Examen3PSussan.Models;
using Examen3PSussan.Views;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Examen3PSussan
{
    public partial class MainPage : ContentPage
    {
        byte[] Image;
        private AudioRecorderService audioRecorderService = new AudioRecorderService()
        {
            StopRecordingOnSilence = false,
            StopRecordingAfterTimeout = false
        };

        private AudioPlayer audioPlayer = new AudioPlayer();
        private bool reproducir = false;
        MediaFile FileFoto = null;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetLatitudeAndLongitude();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            bool response = await DisplayAlert("Advertencia", "Seleccione el tipo de imagen que desea", "Camara", "Galeria");

            if (response)
                GetImageFromCamera();
            else
                GetImageFromGallery();
        }

        private async void GetImageFromGallery()
        {
            try
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    var FileFoto = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                    });
                    if (FileFoto == null)
                        return;

                    imgFoto.Source = ImageSource.FromStream(() => { return FileFoto.GetStream(); });
                    Image = File.ReadAllBytes(FileFoto.Path);
                }
                else
                {
                    ShowMessage("Advertencia", "Se produjo un error al seleccionar la imagen");
                }
            }
            catch (Exception)
            {
                ShowMessage("Advertencia", "Se produjo un error al seleccionar la imagen");
            }
        }

        private async void GetImageFromCamera()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (status == PermissionStatus.Granted)
            {
                try
                {
                    FileFoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        SaveToAlbum = true
                    });

                    if (FileFoto == null)
                        return;

                    imgFoto.Source = ImageSource.FromStream(() => { return FileFoto.GetStream(); });
                    Image = File.ReadAllBytes(FileFoto.Path);
                }
                catch (Exception)
                {
                    ShowMessage("Advertencia", "Se produjo un error al tomar la fotografia.");
                }
            }
            else
            {
                await Permissions.RequestAsync<Permissions.Camera>();
            }
        }
        public static string GenerateRandomId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var id = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
            return id;
        }
        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                ShowMessage("Advertencia", "Actualmente no cuenta con acceso a internet");
                return;
            }

            if (Image == null)
            {
                ShowMessage("Aviso", "Aun no se ha tomado una foto: Presione la imagen de ejemplo para capturar una imagen");
                return;
            }

            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                ShowMessage("Aviso", "Debe escribir una breve descripción");
                return;
            }

            if (txtDescription.Text.Length > 50)
            {
                ShowMessage("Aviso", "La descripción debe ser más corta");
                return;
            }

            if (!reproducir)
            {
                ShowMessage("Aviso", "No ha grabado ningún audio");
                return;
            }

            var audioBytes = ConvertAudioToByteArray();
            if (audioBytes.Length > 1500000)
            {
                ShowMessage("Aviso", "El audio debe ser más corto");
                return;
            }

            try
            {
                UserDialogs.Instance.ShowLoading("Guardando Nota", MaskType.Clear);
                DateTime selectedDate = datePicker.Date;
                string selectedDateStr = selectedDate.ToString("yyyy-MM-dd");
                string newId = GenerateRandomId();
                var sitio = new Sitio()
                {
                    Id = newId,
                    Fecha = selectedDateStr,
                    Description = txtDescription.Text,
                    Image = Image,
                    AudioFile = audioBytes
                };

                var result = await SitioController.CreateSite(sitio);

                UserDialogs.Instance.HideLoading();
                await Task.Delay(500);

                if (result)
                {
                    ShowMessage("Aviso", "Nota agregado correctamente");
                    ClearComponents();
                }
                else
                {
                    ShowMessage("Aviso", "No se pudo agregar la Nota");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await Task.Delay(500);
                ShowMessage("Error: ", ex.Message);
            }
        }

        private async void btnList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ListSite());
        }

        private async void GetLatitudeAndLongitude()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                {
                    var localizacion = await Geolocation.GetLocationAsync();
                    /*txtLatitude.Text = Math.Round(localizacion.Latitude, 5).ToString();
                    txtLongitude.Text = Math.Round(localizacion.Longitude, 5).ToString();*/
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
                    ShowMessage("Error", "Servicio de localización no encendido");
                }
                else
                {
                    ShowMessage("Error", e.Message);
                }
            }
        }

        private void ClearComponents()
        {
            imgFoto.Source = "imgMuestra.png";
            txtDescription.Text = "";
            Image = null;
            GetLatitudeAndLongitude();
        }

        private async void btnGrabar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.Microphone>();
                var status2 = await Permissions.RequestAsync<Permissions.StorageRead>();
                var status3 = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted || status2 != PermissionStatus.Granted || status3 != PermissionStatus.Granted)
                {
                    return; // si no tiene los permisos no avanza
                }

                if (audioRecorderService.IsRecording)
                {
                    await audioRecorderService.StopRecording();
                    audioPlayer.Play(audioRecorderService.GetAudioFilePath());
                   
                    btnGrabar.Text = "Grabar audio";
                    btnGrabar.BackgroundColor = Color.LimeGreen;
                    reproducir = true;
                }
                else
                {
                    await audioRecorderService.StartRecording();
                   
                    btnGrabar.Text = "Dejar de Grabar";
                    btnGrabar.BackgroundColor = Color.Red;
                    btnGrabar.TextColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", ex.Message, "OK");
            }
        }
        

        private async void ShowMessage(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        private Byte[] ConvertAudioToByteArray()
        {
            Stream audioFile = audioRecorderService.GetAudioFileStream();

            Byte[] bytes = ReadFully(audioFile);
            return bytes;
        }

        private Byte[] ConvertImageToByteArray()
        {
            if (FileFoto != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = FileFoto.GetStream();
                    stream.CopyTo(memory);

                    return memory.ToArray();
                }
            }

            return null;
        }

        private Byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
