using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Traveller.ReverseGeocodingJsonClass
{
    class RootObject
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
}
