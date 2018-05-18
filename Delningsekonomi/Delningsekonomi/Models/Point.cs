using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GMapsAPITest.Models
{
    public class Point
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("location.latitude")]
        public string Latitude { get; set; }
        [JsonProperty("location.longitude")]
        public string Longitude { get; set; }
        [JsonProperty("tags")]
        public IList<string> Tags { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        public string Distance { get; set; }
    }

    public class PointJSON
    {
        [JsonProperty("resources")]
        public List<Resource> resources { get; set; }    
    }

    public class Resource
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("location")]
        public Location Location { get; set; }
        [JsonProperty("tags")]
        public IList<string> Tags { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }

        public string Distance { get; set; }


    }

    public class Location
    {
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }
}