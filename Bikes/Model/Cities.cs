using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Messaging;
using Windows.Storage;
using System.IO;

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

        public static City FindMyCity(BasicGeoposition myLocation)
        {
            City myCity = null;
            double closestCity = double.MaxValue;

            foreach (var city in AllCities.Values)
            {
                double distanceToCityCenter = GeoUtil.DistanceTo(myLocation, city.Center);

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
            }
        }

        public static async void LoadCities()
        {
            cities.Clear();

            var bikeShares = new Uri("ms-appx:///Assets/BikeShares.xml");
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(bikeShares);
            XDocument doc = null;
            using (var stream = await sFile.OpenStreamForReadAsync())
            {
                doc = XDocument.Load(stream);
            }

            foreach (var xmlBikeShare in doc.Descendants("BikeShare"))
            {
                var vendor = xmlBikeShare.Element("Vendor").Value;
                var cityName = xmlBikeShare.Element("City").Value;
                var country = new Country(xmlBikeShare.Element("Country").Value);
                var serviceUrl = xmlBikeShare.Element("Url").Value;
                var cacheHack = xmlBikeShare.Element("CacheHack").ToInt(0) == 0 ? false : true;
                var location = xmlBikeShare.ToBCycleGeoCoordinate();
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

       
        internal static async Task InitializeAsync()
        {
            LoadCities();
            // FIXME
            //await DetermineCurrentCityAsync();
        }

    }
}
