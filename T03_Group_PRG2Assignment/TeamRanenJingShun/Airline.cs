using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    class Airline
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Dictionary<string, Flight> Flights { get; set; }

        public bool AddFlight(Flight flight)
        {

        }
        public double CalculateFees()
        {

        }

        public bool RemoveFlight(Flight flight)
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
