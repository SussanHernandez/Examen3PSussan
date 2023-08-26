using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Examen3PSussan.Models
{
    public class Sitio
    {
        [JsonProperty("id")]
        public String Id { get; set; }
        public string Key { get; set; }
        [JsonProperty("fecha")]
        public String Fecha { get; set; }

        [JsonProperty("descripcion")]
        public String Description { get; set; }

        [JsonProperty("fotografia")]
        public byte[] Image { get; set; }

        [JsonProperty("audiofile")]
        public byte[] AudioFile { get; set; }
        
    }
}

