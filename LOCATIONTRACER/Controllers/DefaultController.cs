using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExifLib;
using GoogleMaps.LocationServices;

namespace LOCATIONTRACER.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            string path=null;
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                ViewBag.pic = pic;
                 path = System.IO.Path.Combine(Server.MapPath("~/Content/Image"), pic);

                file.SaveAs(path);
                using(System.IO.MemoryStream ms =new System.IO.MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }
            return RedirectToAction("detail", new { ipath=path});
        }
        public ActionResult detail(string ipath)
        {
            double[] GpsLongArray;
            double[] GpsLatArray;
            double GpsLongDouble=0;
            double GpsLatDouble=0;
            ExifReader er = new ExifReader(ipath);
            if(er.GetTagValue<double[]>(ExifTags.GPSLatitude,out GpsLatArray)&& er.GetTagValue<double[]>(ExifTags.GPSLongitude, out GpsLongArray))
            {
                GpsLongDouble = GpsLongArray[0] + GpsLongArray[1] / 60 + GpsLongArray[2] / 3600;
                GpsLatDouble = GpsLatArray[0] + GpsLatArray[1] / 60 + GpsLatArray[2] / 3600;
            }
            ViewBag.imgpath = ipath;
            ViewBag.latitude = GpsLatDouble;
            ViewBag.longitude = GpsLongDouble;
            GoogleMaps.LocationServices.GoogleLocationService svc = new GoogleMaps.LocationServices.GoogleLocationService();
            Region result = svc.GetRegionFromLatLong(GpsLatDouble, GpsLongDouble);
            var output = string.Format("That Lat/Long is in {0} ({1}) state", result.Name, result.ShortCode);
            ViewBag.Out = output;
            return View();
        }
        //public void TestLocationService()
        //{
        //    //GoogleMaps.Geolocation.GoogleLocationService svc = new GoogleMaps.Geolocation.GoogleLocationService();
        //    GoogleMaps.LocationServices.GoogleLocationService svc = new GoogleMaps.LocationServices.GoogleLocationService();
        //    Region result = svc.GetRegionFromLatLong(47.639747, -122.129731);
        //    var output = string.Format("That Lat/Long is in {0} ({1}) state", result.Name, result.ShortCode);
        //}
    }
}