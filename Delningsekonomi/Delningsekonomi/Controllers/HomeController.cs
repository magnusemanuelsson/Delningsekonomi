﻿using Delningsekonomi.Models;
using GMapsAPITest;
using GMapsAPITest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Delningsekonomi.Controllers
{
    public class HomeController : Controller
    {
        private PointsAPI service = new PointsAPI();
        static List<string> filterList;

        public HomeController()
        {
            if (filterList == null)
            {
                RedirectToAction("Start");
            }
        }

        public ActionResult Start()
        {
            filterList = new List<string>();
            SetFilter("Orange", true);
            SetFilter("Blue", true);
            SetFilter("Buy", true);
            SetFilter("Rent", true);
            SetFilter("Service", true);
            SetFilter("Food", true);
            SetFilter("Vehicle", true);
            SetFilter("Misc", true);
            return View();
        }

        public async Task<ActionResult> Index(string city2, string cityLat, string cityLng)
        {

            string radius;
            if(Session["rangevalue"] == null)
            {
                radius = "1000";
            }
            else { 
                radius = (string) Session["rangevalue"];
            }

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

            PointJSON unfilteredList = await service.GetPoints(cityLat, cityLng, radius, new List<string>());
            PointJSON pointsList = GetFilteredList(unfilteredList);

            MapPoints mapPoints = new MapPoints(pointsList.resources.Count);

            int counter = 0;
            foreach (var item in pointsList.resources)
            {
                mapPoints.Features[counter].Geometry.Type = "Point";
                mapPoints.Features[counter].Geometry.Coordinates[0] = float.Parse(item.Location.Longitude, CultureInfo.InvariantCulture);
                mapPoints.Features[counter].Geometry.Coordinates[1] = float.Parse(item.Location.Latitude, CultureInfo.InvariantCulture);
                mapPoints.Features[counter].Type = "Feature";
                if (item.Tags != null && item.Tags.Count >= 3 &&
                    item.Tags[0] != null && item.Tags[1] != null && item.Tags[2] != null &&
                    (item.Tags[0] == "Orange" || item.Tags[0] == "Blue"))
                {
                    mapPoints.Features[counter].Properties.Category = item.Tags[0] + item.Tags[1] + item.Tags[2];
                    createImage(item.Tags[0], item.Tags[1], item.Tags[2]);
                }
                else
                {
                    mapPoints.Features[counter].Properties.Category = "Gray";
                }
                mapPoints.Features[counter].Properties.Hours = "N/A";
                mapPoints.Features[counter].Properties.Description = ((item.Description != null) ? item.Description.ToString() : "N/A");
                mapPoints.Features[counter].Properties.Name = ((item.Title != null) ? item.Title.ToString() : "N/A");
                mapPoints.Features[counter].Properties.Phone = "N/A";
                counter++;
            }

            string jsonString = JsonConvert.SerializeObject(mapPoints);

            ViewBag.Lat = cityLat;
            ViewBag.Lng = cityLng;
            ViewBag.Radius = radius;

            return View("Index", model: jsonString);
        }

        public async Task<ActionResult> ListView(string city2, string cityLat, string cityLng)
        {
            string radius;
            if (Session["rangevalue"] == null)
            {
                radius = "1000";
            }
            else
            {
                radius = (string)Session["rangevalue"];
            }

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
            PointJSON unfilteredList = await service.GetPoints(cityLat, cityLng, radius , new List<string>());
            PointJSON pointsList = GetFilteredList(unfilteredList);
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

        public void createImage(string str1, string str2, string str3)
        {
            var dir = Server.MapPath("/Images/Pins");
            var path1 = Path.Combine(dir, str1 + ".png");
            var path2 = Path.Combine(dir, str2 + ".png");
            var path3 = Path.Combine(dir, str3 + ".png");
            var outpath = Path.Combine(dir, str1 + str2 + str3 + ".png");
            Bitmap firstImage = (Bitmap)Image.FromFile(path1, true);
            Bitmap secondImage = (Bitmap)Image.FromFile(path2, true);
            Bitmap thirdImage = (Bitmap)Image.FromFile(path3, true);
            Bitmap compImage1 = MergeThreeImages(firstImage, secondImage, thirdImage);
            compImage1.Save(outpath, ImageFormat.Png);
        }

        [HttpPost]
        public ActionResult slider(FormCollection fc)
        {
            Session["rangevalue"] = fc["rangevalue"];
            return RedirectToAction("Index");
        }

        public static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width > secondImage.Width ? firstImage.Width : secondImage.Width;

            int outputImageHeight = firstImage.Height + secondImage.Height + 1;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new System.Drawing.Point(), firstImage.Size),
                    new Rectangle(new System.Drawing.Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new System.Drawing.Point(0, firstImage.Height + 1), secondImage.Size),
                    new Rectangle(new System.Drawing.Point(), secondImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        public static Bitmap MergeThreeImages(Image firstImage, Image secondImage, Image thirdImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            if (thirdImage == null)
            {
                throw new ArgumentNullException("thirdImage");
            }

            int outputImageWidth = firstImage.Width;
            int outputImageHeight = firstImage.Height;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(
                    firstImage,
                    new Rectangle(new System.Drawing.Point(), firstImage.Size),
                    new Rectangle(new System.Drawing.Point(), firstImage.Size),
                    GraphicsUnit.Pixel);
                graphics.DrawImage(
                    secondImage,
                    new Rectangle(256 - secondImage.Size.Width, 64, secondImage.Size.Width * 2, secondImage.Size.Height * 2),
                    new Rectangle(0, 0, secondImage.Size.Width, secondImage.Size.Height),
                    GraphicsUnit.Pixel);
                graphics.DrawImage(
                    thirdImage,
                    new Rectangle(new System.Drawing.Point(256, 192), thirdImage.Size),
                    new Rectangle(new System.Drawing.Point(), thirdImage.Size),
                    GraphicsUnit.Pixel);
            }

            return outputImage;

        }

        [HttpPost]
        public ActionResult FilterPost(bool Orange, bool Blue, bool Buy, bool Rent, bool Service, bool Food, bool Vehicle, bool Misc)
        {
            SetFilter(nameof(Orange), Orange);
            SetFilter(nameof(Blue), Blue);
            SetFilter(nameof(Buy), Buy);
            SetFilter(nameof(Rent), Rent);
            SetFilter(nameof(Service), Service);
            SetFilter(nameof(Food), Food);
            SetFilter(nameof(Vehicle), Vehicle);
            SetFilter(nameof(Misc), Misc);

            return RedirectToAction("Index");
        }

        public void SetFilter(string name, bool value)
        {
            if (value)
            {
                if (!filterList.Contains(name))
                {
                    filterList.Add(name);
                }
            }
            else
            {
                if (filterList.Contains(name))
                {
                    filterList.Remove(name);
                }
            }
        }

        [HttpPost]
        public ActionResult FilterPostlist(bool Orange, bool Blue, bool Buy, bool Rent, bool Service, bool Food, bool Vehicle, bool Misc)
        {
            SetFilter(nameof(Orange), Orange);
            SetFilter(nameof(Blue), Blue);
            SetFilter(nameof(Buy), Buy);
            SetFilter(nameof(Rent), Rent);
            SetFilter(nameof(Service), Service);
            SetFilter(nameof(Food), Food);
            SetFilter(nameof(Vehicle), Vehicle);
            SetFilter(nameof(Misc), Misc);

            return RedirectToAction("ListView");
        }

        public PointJSON GetFilteredList(PointJSON inputlist)
        {
            PointJSON list1 = new PointJSON();

            foreach(Resource resource in inputlist.resources)
            {
                if (filterList.Contains("Orange") && resource.Tags.Contains("Orange"))
                {
                    list1.resources.Add(resource);
                }
                if (filterList.Contains("Blue") && resource.Tags.Contains("Blue"))
                {
                    list1.resources.Add(resource);
                }
            }

            PointJSON list2 = new PointJSON();

            foreach (Resource resource in list1.resources)
            {
                if (filterList.Contains("Buy") && resource.Tags.Contains("Buy"))
                {
                    list2.resources.Add(resource);
                }
                if (filterList.Contains("Rent") && resource.Tags.Contains("Rent"))
                {
                    list2.resources.Add(resource);
                }
                if (filterList.Contains("Service") && resource.Tags.Contains("Service"))
                {
                    list2.resources.Add(resource);
                }
            }

            PointJSON list3 = new PointJSON();

            foreach (Resource resource in list2.resources)
            {
                if (filterList.Contains("Food") && resource.Tags.Contains("Food"))
                {
                    list3.resources.Add(resource);
                }
                if (filterList.Contains("Vehicle") && resource.Tags.Contains("Vehicle"))
                {
                    list3.resources.Add(resource);
                }
                if (filterList.Contains("Misc") && resource.Tags.Contains("Misc"))
                {
                    list3.resources.Add(resource);
                }
            }


            return list3;
        }
    }
}