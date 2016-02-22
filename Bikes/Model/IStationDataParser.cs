using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public interface IStationDataParser
    {
        StationData Parse(string input);
    }
}
