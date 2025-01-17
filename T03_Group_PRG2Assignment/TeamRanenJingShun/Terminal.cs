using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    internal class Terminal
    {
        public string TerminalName { get; set; }
        public Dictionary<string, Airline> Airlines { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }

        public Dictionary<string, BoardingGate> BoardingGates { get; set; }

        public Dictionary<string, double> GateFees { get; set; }

        public Terminal(string name)
        {
            TerminalName = name;
            Airlines = new Dictionary<string, Airline>();
            Flights = new Dictionary<string, Flight>();
            BoardingGates = new Dictionary<string, BoardingGate>();
            GateFees = new Dictionary<string, double>();
        }

        public bool AddAirline(Airline airline) 
        {
            if (Airlines.ContainsKey(airline.Code))
            {
                return false;
            }
            else Parallel
        }
    }
}
