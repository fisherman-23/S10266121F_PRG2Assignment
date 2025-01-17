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
            int count = 0;

            foreach (KeyValuePair<string, Flight> entry in Flights)
            {
                count++;
                totalFees += entry.Value.CalculateFees();
                // For flights arriving/departing before 11am or after 9pm, airlines receive a discount of 110 on the total fees.
                if (entry.Value.ExpectedTime.Hour < 11 || entry.Value.ExpectedTime.Hour > 21)
                {
                    totalFees -= 110;
                }
                // For not indicating any Special Request Codes, airlines receive a discount of 50 on the total fees.
                Console.WriteLine(entry.Value.GetType());
                if (entry.Value.GetType() == typeof(Flight) || entry.Value.GetType() == typeof(NORMFlight))
                {
                    totalFees -= 50;
                }
            }
            // For every 3 flights arriving/departing, airlines receive a discount of 350 on the total fees.
            totalFees -= (count / 3) * 350;


            // For more than 5 flights arriving/departing, airlines receive an additional discount of 3% on the total fees.
            if (count > 5)
            {
                totalFees *= 0.97;
            }

            // For airlines with the Origin of Dubai (DXB), Bangkok (BKK) or Tokyo (NRT), airlines receive a discount of 25 on the total fees.
            if (Flights.Values.Any(f => f.Origin == "DXB" || f.Origin == "BKK" || f.Origin == "NRT"))
            {
                totalFees -= 25;
            }


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
