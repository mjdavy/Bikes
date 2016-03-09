using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Bikes.Model
{
    public class StationLoader
    {
        const string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        Uri baseUri = new Uri("http://api.citybik.es/");

        public StationLoader()
        {
        }

        public async Task<BikeShareNetworks> LoadBikeSharesAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", userAgent);
            var networks = new BikeShareNetworks();

            try
            {
                var Uri = new Uri(baseUri, "/v2/networks");
                var networksJson = await client.GetStringAsync(baseUri);
                networks =  await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<BikeShareNetworks>(networksJson));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); // MJDTODO - error handling
            }

            return networks;
        }

        public async Task<BikeShareNetwork> LoadBikeShareAsync(string networkEndPoint)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", userAgent);
            var network = new BikeShareNetwork();
            
            try
            {
                var endPoint = new Uri(baseUri, networkEndPoint);
                var networkJson = await client.GetStringAsync(endPoint);
                network = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<BikeShareNetwork>(networkJson));
            }
            catch (Exception ex)
            { 
                Debug.WriteLine(ex.Message);
            }

            return network;
        }

    }
}
