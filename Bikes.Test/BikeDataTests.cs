using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Bikes.Model;

namespace Bikes.Test
{
    [TestClass]
    public class BikeDataTests
    {
        [TestMethod]
        public async Task TestNetworks()
        {
            StationLoader loader = new StationLoader();
            var bikeNetworks = await loader.LoadBikeSharesAsync();
            Assert.IsNotNull(bikeNetworks);
            Assert.IsTrue(bikeNetworks.Networks.Count > 0);
        }

        [TestMethod]
        public async Task TestNetwork()
        {
            StationLoader loader = new StationLoader();
            var bikeNetwork = await loader.LoadBikeShareAsync("/v2/networks/hubway");
            Assert.IsNotNull(bikeNetwork);
            Assert.IsTrue(bikeNetwork.Stations.Stations.Count > 0);
        }
    }
}
