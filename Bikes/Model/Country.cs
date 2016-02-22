using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bikes.Model
{
    public class Country : IComparable
    {
        public Country(string country) 
        {
            this.Name = country;
        }
        public string Name
        {
            get;
            set;
        }

        public string Flag
        {
            get
            {
                var imagePath = string.Format("/Images/{0}.png", Name);
                return imagePath;
            }
        }

        public override string ToString()
        {
            return this.Name.ToString();
        }

        public override bool Equals(object obj)
        {
            var country = obj as Country;
            return this.Name.Equals(country.Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var country = obj as Country;
            return this.Name.CompareTo(country.Name);
        }
    }
}
