using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Delningsekonomi.Models
{
    public class MapPoints
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("features")]
        public Feature[] Features { get; set; }

        public MapPoints(int size)
        {
            Type = "FeatureCollection";
            Features = new Feature[size];
            for (int i = 0; i < size; i++)
            {
                Features[i] = new Feature();
            }
        }
    }

    public class Feature
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        public Feature()
        {
            Geometry = new Geometry();
            Type = "";
            Properties = new Properties();
        }
    }

    public class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public float[] Coordinates { get; set; }

        public Geometry()
        {
            Type = "";
            Coordinates = new float[2];
        }
    }

    public class Properties
    {
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("hours")]
        public string Hours { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }

        public Properties()
        {
            Category = "";
            Hours = "";
            Description = "";
            Name = "";
            Phone = "";
        }
    }

   
}