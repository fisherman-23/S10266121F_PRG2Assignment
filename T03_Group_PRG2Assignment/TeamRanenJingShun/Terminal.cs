﻿//==========================================================
// Student Number : S10267339B
// Student Name : Ranen Sim
// Partner Name : Ooi Jing Shun
//==========================================================
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
            else
            {
                Airlines.Add(airline.Code, airline);
                return true;
            }
        }

        public bool AddBoardingGate(BoardingGate boardingGate)
        {
            if (BoardingGates.ContainsKey(boardingGate.GateName))
            {
                return false;
            }
            else
            {
                BoardingGates.Add(boardingGate.GateName, boardingGate);
                return true;
            }
        }

        public Airline? GetAirlineFromFlight(Flight flight)
        {
            foreach (KeyValuePair<string, Airline> kvp in Airlines)
            {
                Airline airline = kvp.Value;
                if (airline.Flights.ContainsKey(flight.FlightNumber))
                {
                    return airline;
                }
            }
            return null;
        }

        public void PrintAirlineFees()
        {
            foreach (KeyValuePair<string, Airline> kvp in Airlines)
            {
                Airline airline = kvp.Value;
                double total = airline.CalculateFees();
                Console.WriteLine($"Airline: {airline.Name}\tTotal Fees: {total:f2}");
            }
        }


        public override string ToString()
        {
            return "Terminal Name: " + TerminalName;
        }
    }
}
