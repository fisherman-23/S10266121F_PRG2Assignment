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
    class Airline
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Dictionary<string, Flight> Flights { get; set; }

        public bool AddFlight(Flight flight)
        {
            Flights.Add(flight.FlightNumber, flight);
            return true;
        }
        public double CalculateFees()
        {
            double totalFees = 0;
            int count = Flights.Count;
            double totalDiscount = 0;


            foreach (KeyValuePair<string, Flight> entry in Flights)
            {

                totalFees += entry.Value.CalculateFees();
                // For flights arriving/departing before 11am or after 9pm, airlines receive a discount of 110 on the total fees.
                if (entry.Value.ExpectedTime.Hour < 11 || entry.Value.ExpectedTime.Hour > 21)
                {
                    totalDiscount += 110;
                }
                // For not indicating any Special Request Codes, airlines receive a discount of 50 on the total fees.
                Console.WriteLine(entry.Value.GetType());
                if (entry.Value.GetType() == typeof(Flight) || entry.Value.GetType() == typeof(NORMFlight))
                {
                    totalDiscount += 50;
                }
                // For each flight with the Origin of Dubai (DXB), Bangkok (BKK) or Tokyo (NRT), airlines receive a discount of 25 on the total fees.
                if (entry.Value.Origin == "Dubai (DXB)" || entry.Value.Origin == "Bangkok (BKK)" || entry.Value.Origin == "Tokyo (NRT)")
                {
                    totalDiscount += 25;
                }
            }

            // For more than 5 flights arriving/departing, airlines receive an additional discount of 3% on the total fees.
            if (count > 5)
            {
                totalFees *= 0.97;
            }
            // For every 3 flights arriving/departing, airlines receive a discount of 350 on the total fees.
            totalDiscount += (count / 3) * 350;


            totalFees -= totalDiscount;
            return totalFees;
        }

        public bool RemoveFlight(Flight flight)
        {
            if (Flights.ContainsKey(flight.FlightNumber))
            {
                Flights.Remove(flight.FlightNumber);
                return true;
            } else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"Name: {Name} Code: {Code} Flights: {Flights}";
        }

        public Airline(string name, string code)
        {
            Flights = new Dictionary<string, Flight>();
            Name = name;
            Code = code;
        }
    }
}
