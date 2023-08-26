using Newtonsoft.Json;
using Examen3PSussan.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;


namespace Examen3PSussan.Controller
{
    public class SitioController
    {
        private static readonly string FirebaseUrl = "https://alumnos-16b81-default-rtdb.firebaseio.com/";
        private static FirebaseClient firebaseClient = new FirebaseClient(FirebaseUrl);

        public static async Task<List<Sitio>> GetAllSite()
        {
            try
            {
                var sitios = await firebaseClient.Child("sitios").OnceAsync<Sitio>();
                var listSitios = new List<Sitio>();

                foreach (var sitio in sitios)
                {
                    listSitios.Add(sitio.Object);
                }
                // Ordenar la lista de sitios por fecha de creación (de más nuevo a más viejo)
                listSitios.Sort((s1, s2) => s2.Fecha.CompareTo(s1.Fecha));
                return listSitios;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<Sitio>();
        }


        public async static Task<bool> DeleteSite(string id)
        {
            try
            {
                await firebaseClient.Child("sitios").Child(id).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async static Task<bool> CreateSite(Sitio sitio)
        {
            try
            {
                
                string sitioId = sitio.Id;

                
                await firebaseClient.Child("sitios").Child(sitioId).PutAsync(sitio);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }



        public async static Task<bool> UpdateSitio(Sitio sitio)
        {
            try
            {
               
                await firebaseClient.Child("sitios").Child(sitio.Id).PutAsync(sitio);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }

}


