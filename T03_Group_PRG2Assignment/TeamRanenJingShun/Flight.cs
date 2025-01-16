//==========================================================
// Student Number : S10266121F
// Student Name : Ooi Jing Shun
// Partner Name : Ranen Sim
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    class Flight
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }

        public string Destination { get; set; }

        public DateTime ExpectedTime { get; set; }

        public string Status { get; set; }

        public double CalculateFees()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"Flight Number: {FlightNumber} Origin: {Origin} Destination: {Destination} ExpectedTime: {ExpectedTime} Status: {Status}";
        }

        public Flight(string flightNo, string origin, string dest, DateTime et, string status)
        {
            FlightNumber = flightNo;
            Origin = origin;
            Destination = dest;
            ExpectedTime = et;
            Status = status;
        }
    }
}
