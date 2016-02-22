using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class Countries
    {
        private static Country currentCountry;

        static Countries()
        {
            AllCountries = new Dictionary<string, Country>();
        }

        public static Country CurrentCountry
        {
            get
            {
                return currentCountry;
            }
            set
            {
                currentCountry = value;
            }
        }

        public static IDictionary<string, Country> AllCountries 
        {
            get;
            set;
        }
    }
}
