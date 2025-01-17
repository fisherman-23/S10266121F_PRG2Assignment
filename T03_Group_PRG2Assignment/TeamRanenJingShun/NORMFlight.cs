using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    class NORMFlight : Flight 
    {

        public NORMFlight(string flightNo, string origin, string dest, DateTime et, string status) : base(flightNo, origin, dest, et, status)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
