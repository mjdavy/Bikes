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
            var bikeNetworks = await loader.LoadNetworksAsync();
            Assert.IsNotNull(bikeNetworks);
            Assert.IsTrue(bikeNetworks.Networks.Count > 0);
        }
    }
}
