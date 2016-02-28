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
using System.Xml.Linq;
using System.Device.Location;
using System.Threading.Tasks;
using Microsoft.Phone.Maps.Services;
using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace Bikes.Model
{
    public static class XExtensions
    {
        public static int ToInt(this XElement xe, int emptyValue)
        {
            return xe == null ? emptyValue : int.Parse(xe.Value);
        }

        public static bool ToBool(this XElement xe, bool emptyValue)
        {
            return xe == null ? emptyValue : bool.Parse(xe.Value);
        }

        // MJDTODO - Refactor  DRY
        public static BasicGeoposition ToBixiGeoCoordinate(this XElement xe, BasicGeoposition emptyValue)
        {
            double latitude = double.Parse(xe.Element("lat").Value);
            double longitude = double.Parse(xe.Element("long").Value);
            return xe == null ? emptyValue :
                new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
        }

        // MJDTODO - Refactor  DRY
        public static BasicGeoposition ToBCycleGeoCoordinate(this XElement xe, BasicGeoposition emptyValue)
        {
            double latitude = double.Parse(xe.Element("Latitude").Value);
            double longitude = double.Parse(xe.Element("Longitude").Value);
            return xe == null ? emptyValue : 
                new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
        }

        // MJDTODO - Refactor  DRY
        public static BasicGeoposition ToBixi2GeoCoordinate(this XElement xe, BasicGeoposition emptyValue)
        {
            double latitude = double.Parse(xe.Element("latitude").Value);
            double longitude = double.Parse(xe.Element("longitude").Value);
            return xe == null ? emptyValue : 
                new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
        }

        public static Task<string> DownloadStringTaskAsync(this WebClient webClient, Uri uri)
        {
            var tcs = new TaskCompletionSource<string>();

            webClient.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            webClient.DownloadStringAsync(uri);

            return tcs.Task;
        }

        public static Task<IList<MapLocation>> QueryAsyncTask(this ReverseGeocodeQuery reverseGeocode, GeoCoordinate coordinate)
        {
            var tcs = new TaskCompletionSource<IList<MapLocation>>();

            reverseGeocode.QueryCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        tcs.SetException(e.Error);
                    }
                    else
                    {
                        tcs.SetResult(e.Result);
                    }
                };

            reverseGeocode.GeoCoordinate = coordinate;
            reverseGeocode.QueryAsync();

            return tcs.Task;

        }

    }
}
