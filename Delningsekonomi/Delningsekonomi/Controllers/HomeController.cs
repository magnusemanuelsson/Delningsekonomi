using GMapsAPITest;
using GMapsAPITest.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Delningsekonomi.Controllers
{
    public class HomeController : Controller
    {
        private PointsAPI service = new PointsAPI();
        

        public async Task<ActionResult> Index(string city2, string cityLat, string cityLng)
        {
            string radius = "1000";

            if (cityLat == null || cityLng == null)
            {
                if (Session["sessionlat"] == null || Session["sessionlng"] == null)
                {
                    cityLat = "63.8181";
                    cityLng = "20.3073";
                }
                else
                {
                    cityLat = (string)Session["sessionlat"];
                    cityLng = (string)Session["sessionlng"];
                }
            }
            else
            {
                Session["sessionlat"] = cityLat;
                Session["sessionlng"] = cityLng;
                Session["sessioncity"] = city2;
            }

            PointJSON pointsList = await service.GetPoints(cityLat, cityLng, radius, null);
            string jsonString = "{\"type\": \"FeatureCollection\",\"features\": [";

            foreach (var item in pointsList.resources)
            {
                jsonString = jsonString + "{\"geometry\": {\"type\": \"Point\",\"coordinates\": [" + item.Location.Longitude.ToString() + "," + item.Location.Latitude.ToString() + "]},";
                jsonString = jsonString + "\"type\": \"Feature\",\"properties\": { ";
                jsonString = jsonString + "\"category\": \"FoodPurchaseOffered\",\"hours\": \"N/A\",\"description\": \"" + ((item.Description != null) ? item.Description.ToString() : "") + "\",\"name\": \"" + ((item.Title != null) ? item.Title.ToString() : "") + "\",\"phone\": \"N/A\"";
                jsonString = jsonString + "}},";
            }
            jsonString = jsonString.TrimEnd(',');
            jsonString = jsonString + "]}";
            System.Diagnostics.Debug.WriteLine(jsonString);
            System.Diagnostics.Debug.WriteLine("Hej");
            ViewBag.Lat = cityLat;
            ViewBag.Lng = cityLng;
            ViewBag.Radius = radius;

            return View("Index", model: jsonString);
        }

        public async Task<ActionResult> ListView(string city2, string cityLat, string cityLng)
        {

            if (cityLat == null || cityLng == null)
            {
                if (Session["sessionlat"] == null || Session["sessionlng"] == null)
                {
                    cityLat = "63.8181";
                    cityLng = "20.3073";
                }
                else
                {
                    cityLat = (string) Session["sessionlat"];
                    cityLng = (string) Session["sessionlng"];
                }
            }
            else
            {
                Session["sessionlat"] = cityLat;
                Session["sessionlng"] = cityLng;
                Session["sessioncity"] = city2;
            }
            PointJSON pointsList = await service.GetPoints(cityLat, cityLng, "1000", null);
            System.Diagnostics.Debug.WriteLine(pointsList);

            foreach (Resource item in pointsList.resources)
            {
                string distancetopoint = Distance(cityLng, cityLat, item.Location.Longitude, item.Location.Latitude);
                item.Distance = distancetopoint;
            }

            PointJSON sortlist = new PointJSON();

            sortlist.resources = pointsList.resources.OrderBy(o => double.Parse(o.Distance)).ToList();
            

            ViewBag.Lat = cityLat;
            ViewBag.Lng = cityLng;

            return View("ListView", model: sortlist.resources);
        }

        public double Rad(double x)
        {
            

            return (x * (Math.PI / 180));

        }
        
        public string Distance(string longitude, string latitude, string pointlong, string pointlat)
        {
            System.Diagnostics.Debug.WriteLine(longitude);
            double currentlong = double.Parse(longitude, CultureInfo.InvariantCulture);
            double currentlat = double.Parse(latitude, CultureInfo.InvariantCulture);
            double longpoint = double.Parse(pointlong, CultureInfo.InvariantCulture);
            double latpoint = double.Parse(pointlat, CultureInfo.InvariantCulture);

            int R = 6378137;
            double dLat = Rad(latpoint - currentlat);
            double dlong = Rad(longpoint - currentlong);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(Rad(currentlat)) * Math.Cos(Rad(latpoint)) * Math.Sin(dlong / 2) * Math.Sin(dlong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return d.ToString();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}