﻿using GMapsAPITest;
using GMapsAPITest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Delningsekonomi.Controllers
{
    public class HomeController : Controller
    {
        private PointsAPI service = new PointsAPI();

        public async Task<ActionResult> Index()
        {

            PointJSON pointsList = await service.GetPoints("63.8181", "20.3073", "1000", null);
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
            ViewBag.APIstring = jsonString;

            return View("Index", model: jsonString);
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