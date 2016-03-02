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

        public static async Task<Geoposition> FindMyLocationAsync()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                var pos = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                return pos;
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

            return null; // FIXME

        }

        internal static async Task DetermineCurrentCityAsync()
        {
            if (AppSettings.Instance.DefaultToNearestCity)
            {
                var location = await FindMyLocationAsync();

                if (location != null)
                {
                    var position = new BasicGeoposition { Latitude = location.Coordinate.Latitude, Longitude = location.Coordinate.Longitude };
                    CurrentCity = FindMyCity(position);
                }
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

            Countries.CurrentCountry = CurrentCity == null ? null : CurrentCity.Country;
        }

        internal static async Task InitializeAsync()
        {
            LoadCities();
            await DetermineCurrentCityAsync();
        }

    }
}
