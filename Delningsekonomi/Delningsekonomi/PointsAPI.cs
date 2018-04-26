using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using GMapsAPITest.Models;
using Newtonsoft.Json;

namespace GMapsAPITest
{
    public class PointsAPI
    {

        readonly string uri = "https://www.askvigg.com/api/v1.1/resources.json";

        public async Task<PointJSON> GetPoints(string latitude, string longitude, string distance, string tags)
        {
            if(distance == null)
            {
                distance = "";
            }
            if (tags == null)
            {
                tags = "";
            }

            using (HttpClient httpClient = new HttpClient())
            {
                string apistr = await httpClient.GetStringAsync(uri + "?latitude=" + latitude + "&longitude=" + longitude + "&distance=" + distance + "&tags=" + tags);
                var returnval = JsonConvert.DeserializeObject<PointJSON>(apistr);
                return returnval;
                
                /*
                dynamic dyn = JsonConvert.DeserializeObject(await httpClient.GetStringAsync(uri + "?latitude=" + latitude + "&longitude=" + longitude + "&distance=" + distance + "&tags=" + tags));
                var modelPoint = new List<Point>();

                foreach (var obj in dyn.data)
                {
                    modelPoint.Add(new Point()
                    {
                        Title = (obj.title != null) ? obj.title.ToString() : "",
                        Latitude = (obj.location.latitude != null) ? obj.location.latitude.ToString() : "",
                        Longitude = (obj.location.longitude != null) ? obj.location.longitude.ToString() : "",
                        Tags = (obj.tags != null) ? obj.tags.ToList() : "",
                        Description = (obj.description != null) ? obj.description.ToString() : ""
                    });
                }

                return modelPoint;
                */
            }
        }
    }
}