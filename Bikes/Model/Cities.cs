using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Device.Location;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Phone.Maps.Services;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Messaging;

namespace Bikes.Model
{
    public class Cities
    {
        private static IDictionary<string, City> cities = new Dictionary<string, City>();
        private static City currentCity = null;

        public static IDictionary<string, City> AllCities
        {
            get
            {
                return cities;
            }
        }

        public static City FindMyCity(GeoCoordinate myLocation)
        {
            City myCity = null;
            double closestCity = double.MaxValue;

            if (myLocation == null || myLocation.IsUnknown)
            {
                return myCity;
            }

            foreach (var city in AllCities.Values)
            {
                double distanceToCityCenter = myLocation.GetDistanceTo(city.Center);

                if (distanceToCityCenter < closestCity)
                {
                    closestCity = distanceToCityCenter;
                    myCity = city;
                }
            }

            return myCity;
        }

        public static City CurrentCity
        {
            get
            {
                return currentCity;
            }
            set
            {
                currentCity = value;
                SaveCurrentCity();
            }
        }

        private static void LoadCurrentCity()
        {
            CurrentCity = null;
            string currentCityName = AppSettings.Instance.CurrentCity;

            if (string.IsNullOrEmpty(currentCityName))
            {
                return;
            }

            if (Cities.AllCities.Keys.Contains(currentCityName))
            {
                currentCity = Cities.AllCities[currentCityName];
            }
        }

        private static void SaveCurrentCity()
        {
            if (currentCity != null && !string.IsNullOrEmpty(currentCity.Name))
            {
                AppSettings.Instance.CurrentCity = currentCity.Name;
                AppSettings.Instance.SaveSettings();
            }
        }

        public static void LoadCities()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resourceStream = assembly.GetManifestResourceStream("Bikes.Model.BikeShares.xml"))
            {
                LoadCities(resourceStream);
            }
        }

        public static void LoadCities(System.IO.Stream stream)
        {
            cities.Clear();

            var bikeSharesXml = XDocument.Load(stream);
            foreach (var xmlBikeShare in bikeSharesXml.Descendants("BikeShare"))
            {
                var vendor = xmlBikeShare.Element("Vendor").Value;
                var cityName = xmlBikeShare.Element("City").Value;
                var country = new Country(xmlBikeShare.Element("Country").Value);
                var serviceUrl = xmlBikeShare.Element("Url").Value;
                var location = xmlBikeShare.ToBCycleGeoCoordinate(GeoCoordinate.Unknown);
                var cacheHack = xmlBikeShare.Element("CacheHack").ToInt(0) == 0 ? false : true;
                var city = new City
                    {
                        Vendor = (City.VendorType)Enum.Parse(typeof(City.VendorType), vendor, true),
                        Name = cityName,
                        Country = country,
                        Url = serviceUrl,
                        UseCacheHack = cacheHack,
                        Center = location
                    };

                if (!cities.Keys.Contains(cityName))
                {
                    cities.Add(cityName, city);
                }

                if (!Countries.AllCountries.Keys.Contains(country.Name))
                {
                    Countries.AllCountries.Add(country.Name, country);
                }
            }

        }

        public static async Task<GeoCoordinate> FindMyLocationAsync()
        {
            var location = new GeoCoordinate();
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                location = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    Messenger.Default.Send<StatusMessage>(new StatusMessage("location service is disabled in phone settings."));
                }
                else
                {
                    Messenger.Default.Send<StatusMessage>(new StatusMessage("Error: unable to determine your location."));
                }
            }

            return location;

        }

        internal static async Task DetermineCurrentCityAsync()
        {
            if (AppSettings.Instance.DefaultToNearestCity)
            {
                var location = await FindMyLocationAsync();
                CurrentCity = FindMyCity(location);
            }
            else
            {
                LoadCurrentCity();
            }

            //  Default if we are unable to determine current city
            if (CurrentCity == null)
            {
                CurrentCity = currentCity = Cities.AllCities["London"];
            }

            Countries.CurrentCountry = CurrentCity.Country;
        }

        internal static async Task InitializeAsync()
        {
            LoadCities();
            await DetermineCurrentCityAsync();
        }

    }
}
