using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;

namespace Bikes.Model
{
    public class StationLoader
    {
        private Dictionary<City.VendorType, IStationDataParser> parserMap;

        public StationLoader()
        {
            parserMap = new Dictionary<City.VendorType, IStationDataParser>();
            this.RegisterParser(City.VendorType.CityBikes, new CityBikesParser());
            this.RegisterParser(City.VendorType.NextBike, new NextBikeParser());
        }

        public void RegisterParser(City.VendorType vendor, IStationDataParser parser)
        {
            parserMap[vendor] = parser;
        }

        public async Task<StationData> LoadDataAsync(City myCity)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            
            var stationData = new StationData();
            
            try
            {
                var rawStationData = await client.GetStringAsync(myCity.DataSource);
                stationData = await ParseDataAsync(myCity, rawStationData);  
            }
            catch (Exception ex)
            {
                stationData.Error("Error: Unable to load bike stations");
                Debug.WriteLine(ex.Message);
            }

            return stationData;
        }

        private async Task<StationData> ParseDataAsync(City myCity, string input)
        {
            return await Task.Run(() =>
                {
                    StationData data;

                    if (string.IsNullOrEmpty(input))
                    {
                        data = new StationData();
                        data.Error("No bike station data available.");

                    }
                    else
                    {
                        var parser = this.parserMap[myCity.Vendor];
                        data = parser.Parse(input);
                    }

                    return data;
                });
        }
    }
}
