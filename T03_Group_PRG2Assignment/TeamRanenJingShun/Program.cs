﻿// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using TeamRanenJingShun;

// Temporary code to test the classes
Flight fl = new NORMFlight("SQ123", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
Flight fl2 = new CFFTFlight("SQ122", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
Flight fl3 = new CFFTFlight("SQ121", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
Flight fl4 = new CFFTFlight("SQ120", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
Flight fl5 = new CFFTFlight("SQ119", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
Flight fl6 = new CFFTFlight("SQ118", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Console.WriteLine(fl.CalculateFees());
//Console.WriteLine(fl2.CalculateFees());
Airline al = new Airline("SQ", "123");
Airline al2 = new Airline("SQ", "122");

al.AddFlight(fl);
al.AddFlight(fl2);
al.AddFlight(fl3);
al.AddFlight(fl4);
al.AddFlight(fl5);
al.AddFlight(fl6);


//Console.WriteLine(al.CalculateFees());
//Console.WriteLine(al2.CalculateFees());

//1 Load files (airlines and boarding gates)

string[] AirlinesCSVLines = File.ReadAllLines("assets/airlines.csv");
Dictionary<string, Airline> AirlineDict = new Dictionary<string, Airline>();
Dictionary<string, BoardingGate> BoardingGateDict = new Dictionary<string, BoardingGate>();

void LoadFiles(Dictionary<string, Airline> AirlineDict, Dictionary<string, BoardingGate> BoardingGateDict)
{
    string[] AirlinesCSVLines = File.ReadAllLines("assets/airlines.csv");
    for (int i = 1; i < AirlinesCSVLines.Length; i++)
    {
        string[] line = AirlinesCSVLines[i].Split(',');
        Airline airline = new Airline(line[0].Trim(), line[1].Trim());
        AirlineDict[line[1]] = airline;
    }

    string[] BoardingGatesCSVLines = File.ReadAllLines("assets/boardinggates.csv");
    for (int i = 1; i < BoardingGatesCSVLines.Length; i++)
    {
        string[] line = BoardingGatesCSVLines[i].Split(',');
        BoardingGate boardingGate = new BoardingGate(line[0].Trim(), Convert.ToBoolean(line[2].Trim()), Convert.ToBoolean(line[1].Trim()), Convert.ToBoolean(line[3].Trim()));
        BoardingGateDict[line[0]] = boardingGate;
    }
}
LoadFiles(AirlineDict, BoardingGateDict);

//2 Load files (flights)
void LoadFlights(Dictionary<string, Flight> FlightDict)
{
    string[] FlightsCSVLines = File.ReadAllLines("assets/flights.csv")[1..];
    foreach (string line in FlightsCSVLines)
    {
        string[] value = line.Split(",");
        string requestCode = value[4];
        if (requestCode == "")
        {
            Flight flight = new NORMFlight(value[0], value[1], value[2], DateTime.Parse(value[3]), "On Time");
            FlightDict.Add(value[0], flight);
        }
        else if (requestCode == "CFFT")
        {
            Flight flight = new CFFTFlight(value[0], value[1], value[2], DateTime.Parse(value[3]), "On Time");
            FlightDict.Add(value[0], flight);
        }
        else if (requestCode == "DDJB")
        {
            Flight flight = new DDJBFlight(value[0], value[1], value[2], DateTime.Parse(value[3]), "On Time");
            FlightDict.Add(value[0], flight);
        }
        else if (requestCode == "LWTT")
        {
            Flight flight = new LWTTFlight(value[0], value[1], value[2], DateTime.Parse(value[3]), "On Time");
            FlightDict.Add(value[0], flight);
        }

    }
}
Dictionary<string, Flight> FlightDict = new Dictionary<string, Flight>();
LoadFlights(FlightDict);

//3 List all flights
void DisplayFlights(Dictionary<string, Flight> FlightDict, Dictionary<string, Airline> AirlineDict)
{
    Console.WriteLine("=============================================\r\nList of Flights for Changi Airport Terminal 5\r\n=============================================");
    Console.WriteLine($"{"Flight Number",-17}{"Airline name",-20}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-30}");
    foreach (KeyValuePair<string, Flight> kvp in FlightDict)
    {
        Console.WriteLine($"{kvp.Value.FlightNumber,-17}{AirlineDict[kvp.Value.FlightNumber[0..2]].Name,-20}{kvp.Value.Origin,-20}{kvp.Value.Destination,-20}{kvp.Value.ExpectedTime,-30}");
        Console.WriteLine();
    }
}   
//DisplayFlights(FlightDict, AirlineDict);


//4 List all boarding gates
Terminal Terminal5 = new Terminal("Terminal5");
void AddDataToTerminal(Terminal terminal)
{
    foreach (KeyValuePair<string, Airline> kvp in AirlineDict)
    {
        terminal.AddAirline(kvp.Value);
    }
    foreach (KeyValuePair<string, BoardingGate> kvp in BoardingGateDict)
    {
        terminal.AddBoardingGate(kvp.Value);
    }
}
AddDataToTerminal(Terminal5);

void DisplayBoardingGates(Terminal terminal)
{
    Console.WriteLine("=============================================\r\nList of Boarding Gates for Changi Airport Terminal 5\r\n=============================================");
    Console.WriteLine($"{"Gate Name", -15} {"DDJB", -20} {"CFFT", -20} {"LWTT"}");
    foreach (KeyValuePair<string, BoardingGate> kvp in terminal.BoardingGates)
    {

        bool CFFT = false;
        bool DDJB = false;
        bool LWTT = false;
        if (kvp.Value.SupportsCFFT)
        {
            CFFT = true;
        }
        if (kvp.Value.SupportsDDJB)
        {
            DDJB = true;
        }
        if (kvp.Value.SupportsLWTT)
        {
            LWTT = true;
        }



        Console.WriteLine($"{kvp.Value.GateName, -15} {DDJB, -20} {CFFT, -20} {LWTT}");
        

        //if (kvp.Value.Flight != null)
        //{
        //    Console.WriteLine($"Assigned Flight Number: {kvp.Value.Flight.FlightNumber}");
        //}
        //else
        //{
        //    Console.WriteLine("No flight assigned to this gate.");
        //}

        Console.WriteLine();
    }

}

//DisplayBoardingGates(Terminal5);

//5 Assign a boarding gate to a flight
void AssignBoardingGateToFlight(Terminal Terminal5, Dictionary<string, Flight> FlightDict, Dictionary<string, BoardingGate> BoardingGateDict)
{
    Dictionary<Type,string> requestCodeDict = new Dictionary<Type, string>
    {
        {typeof(CFFTFlight), "CFFT"},
        {typeof(DDJBFlight), "DDJB"},
        {typeof(LWTTFlight), "LWTT"},
        {typeof(NORMFlight), "None"},
        {typeof(Flight), "None"}
    };  
    Console.WriteLine("Enter Flight Number: ");
    string flightNumber = Console.ReadLine(); //SQ 115
    if (FlightDict.ContainsKey(flightNumber))
    {   
        Console.WriteLine("Enter Boarding Gate Name: ");
        string gateName = Console.ReadLine(); //A1
        if (BoardingGateDict.ContainsKey(gateName))
        {   
            if (Terminal5.BoardingGates[gateName].Flight != null)
            {
                Console.WriteLine("This gate is already assigned to a flight.");
                return;
            }
            Flight flight = FlightDict[flightNumber];
            Console.WriteLine($"Flight Number: {flight.FlightNumber}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Time: {flight.ExpectedTime}");
            Console.WriteLine($"Special Request Code: {requestCodeDict[flight.GetType()]}");

            Console.WriteLine($"Boarding Gate Name: {gateName}");
            Console.WriteLine($"Supports DDJB: {BoardingGateDict[gateName].SupportsDDJB}");
            Console.WriteLine($"Supports CFFT: {BoardingGateDict[gateName].SupportsCFFT}");
            Console.WriteLine($"Supports LWTT: {BoardingGateDict[gateName].SupportsLWTT}");

            while (true) {
                Console.WriteLine("Would you like to update the status of the flight? (Y/N)");
                string response = Console.ReadLine();

                if (response == "Y")
                {
                    while (true)
                    {
                        Console.WriteLine("1. Delayed\n2. Boarding\n3. On time");

                        string status = Console.ReadLine();
                        if (status == "1")
                        {
                            flight.Status = "Delayed";
                            break;
                        }
                        else if (status == "2")
                        {
                            flight.Status = "Boarding";
                            break;
                        }
                        else if (status == "3")
                        {
                            flight.Status = "On Time";
                            break;

                        }
                        else
                        {
                            Console.WriteLine("Invalid status.");
                        }

                    }
                    break;
                }
                else if (response == "N")
                {
                    flight.Status = "On Time";
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid response.");
                }
            }


            Terminal5.BoardingGates[gateName].Flight = flight;
            Console.WriteLine($"Flight {flight.FlightNumber} has been assigned to gate {gateName}.");
        }
        else
        {
            Console.WriteLine("Invalid gate name.");
        }
    }
    else
    {
        Console.WriteLine("Invalid flight number.");
    }
}
//AssignBoardingGateToFlight(Terminal5, FlightDict, BoardingGateDict);

//6 Create a new flight
void CreateNewFlight(Dictionary<string, Flight> FlightDict)
{
    while (true)
    {
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();
        Console.Write("Enter Origin: ");
        string origin = Console.ReadLine();
        Console.Write("Enter Destination: ");
        string destination = Console.ReadLine();
        Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
        DateTime expectedTime = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
        string requestCode = Console.ReadLine();
        
        if (requestCode == "")
        {
            Flight flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "On Time");
            FlightDict.Add(flightNumber, flight);
            // Add to flights.csv
            File.AppendAllText("assets/flights.csv", $"{flightNumber},{origin},{destination},{expectedTime},\n");
        }
        else if (requestCode == "CFFT")
        {
            Flight flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "On Time");
            FlightDict.Add(flightNumber, flight);
            // Add to flights.csv
            File.AppendAllText("assets/flights.csv", $"{flightNumber},{origin},{destination},{expectedTime},CFFT\n");
        }
        else if (requestCode == "DDJB")

        {
            Flight flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "On Time");
            FlightDict.Add(flightNumber, flight);
            // Add to flights.csv
            File.AppendAllText("assets/flights.csv", $"{flightNumber},{origin},{destination},{expectedTime},DDJB\n");
        }
        else if (requestCode == "LWTT")
        {
            Flight flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "On Time");
            FlightDict.Add(flightNumber, flight);
            // Add to flights.csv
            File.AppendAllText("assets/flights.csv", $"{flightNumber},{origin},{destination},{expectedTime},LWTT\n");
        }

        Console.WriteLine($"Flight {flightNumber} has been added!");
        Console.WriteLine("Would you like to add another flight? (Y/N)");
        string response = Console.ReadLine();
        if (response == "Y")
        {
            Console.WriteLine("Continuing");
        } else if (response == "N")
        {
            Console.WriteLine("Exiting...");
            break;
        }
        else
        {
            Console.WriteLine("Not a valid option, exiting...");
            break;
        }
    }

}


// 7 Display full flight details from an airline
void DisplayAirline(Terminal terminal)
{
    Console.WriteLine();
    Console.WriteLine("=============================================\r\nList of Airlines for Changi Airport Terminal 5\r\n=============================================");
    Console.WriteLine($"{"Airline Code", -20} Airline Name");
    foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
    {
        Console.WriteLine($"{kvp.Key, -20} {kvp.Value.Name}");
    }
}

Airline DisplayFlightFromAirline(Terminal terminal)
{
    string? code = null;
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("Enter Airline Code: ");
        try
        {
            code = Console.ReadLine().ToUpper();
        }
        catch
        {
            Console.WriteLine("Invalid input, please try again.");
            continue;
        }
        

        Airline? airline = null;
        foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
        {
            if (kvp.Key == code)
            {
                airline = kvp.Value;
            }
        }
        if (airline != null)
        {
            Console.WriteLine();
            if (airline.Flights.Count == 0)
            {
                Console.WriteLine($"No flights available for {airline.Name}");

            }
            else
            {
                Console.WriteLine($"{"Flight Number", -15} {"Origin", -20} {"Destination"}");
                //Console.WriteLine("Departure/Arrival Time");
                foreach (KeyValuePair<string, Flight> kvp in airline.Flights)
                {
                    Flight flight = kvp.Value;
                    Console.WriteLine($"{flight.FlightNumber, -15} {flight.Origin, -20} {flight.Destination, -20}");
                }
                return airline;
            }
        }
        else
        {
            Console.WriteLine("Airline not found");
            continue;
        }
    }

}

Terminal5.AddAirline(al);
//DisplayAirline(Terminal5);
//DisplayFlightDetails(DisplayFlightFromAirline(Terminal5), Terminal5, FlightDict);


void DisplayFlightDetails(Airline airline, Terminal terminal, Dictionary<string, Flight> FlightDict)
{
    string? code = null;
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("Enter flight number: ");
        try
        {
            code = Console.ReadLine().ToUpper();
        }
        catch
        {
            Console.WriteLine("Invalid input, please try again.");
            continue;
        }

        foreach(KeyValuePair<String, Flight> kvp in airline.Flights)
        {
            Flight flight = kvp.Value;
            if (kvp.Key == code)
            {
                Console.WriteLine();
                Console.WriteLine($"Full flight details for {flight.FlightNumber}");
                string date = flight.ExpectedTime.ToString("dd/MM/yyyy");
                string time = flight.ExpectedTime.ToString("hh:mm tt") + ":00";
                Console.WriteLine($"{"Flight Number", -15} {"Airline Name", -25} {"Origin", -15} {"Destination", -15} {"Expected Date", -16} {"Expected Time"}");

                bool sr = false;
                bool bg = false;
                //BoardingGate? boardingGate = null;
                if (flight.GetType() == typeof(CFFTFlight) || flight.GetType() == typeof(DDJBFlight) || flight.GetType() == typeof(LWTTFlight))
                {
                    Console.Write($"{"Special Request Code", -30}");
                    sr = true;
                }
                //if (!(flight.GetType() == typeof(NORMFlight))) {
                //    Console.Write($"{"Boarding Gate", -15}");
                //    bg = true;
                //}
                BoardingGate? boardingGate = FindBoardingGateByFlightNumber(FlightDict, terminal, flight);
                if (boardingGate != null)
                {
                    Console.Write($"{"Boarding Gate", -15}");
                    bg = true;
                }
                Console.WriteLine();
                Console.WriteLine($"{flight.FlightNumber, -15} {airline.Name, -25} {flight.Origin, -15} {flight.Destination, -15} {date, -16} {time}");

                if (sr)
                {
                    Console.Write($"{GetRequestCode(flight),-30}");
                }
                if (bg)
                {
                    Console.WriteLine(FindBoardingGateByFlightNumber(FlightDict, terminal, flight));
                    //foreach (string gate in GetBoardingGate(flight))
                    //{
                    //    Console.Write(gate + " ");
                    //}
                }
                return;
            }
        }
    }
}

//string[]? GetBoardingGate(Flight flight)
//{
//    if (flight is CFFTFlight)
//    {
//        return new string[] { "B1", "B2", "B3", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10", "C11", "C12", "C13", "C14", "C15", "C16", "C17", "C18", "C19", "C20", "C21", "C22" };
//    }
//    else if (flight is DDJBFlight)
//    {
//        return new string[] { "A10", "A11", "A12", "A13", "A20", "A21", "A22", "B10", "B11", "B12" };
//    }
//    else if (flight is LWTTFlight)
//    {
//        return new string[] { "A1", "A2", "A20", "A21", "A22", "C14", "C15", "C16", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10", "B11", "B12", "B13", "B14", "B15", "B16", "B17", "B18", "B19", "B20", "B21", "B22" };
//    }
//    return null;
//}

string GetRequestCode(Flight flight)
{
    if (flight is DDJBFlight)
    {
        return "DDJB";
    }
    else if (flight is CFFTFlight)
    {
        return "CFFT";
    }
    else if (flight is LWTTFlight)
    {
        return "LWTT";
    }
    else
    {
        return "NORM";
    }
}


// 8 Modify flight details
ModifyFlightUserInput(Terminal5, FlightDict);

Airline DisplayFullFlightFromAirline(Terminal terminal)
{
    string? code = null;
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("Enter Airline Code: ");
        try
        {
            code = Console.ReadLine().ToUpper();
        }
        catch
        {
            Console.WriteLine("Invalid input, please try again.");
            continue;
        }


        Airline? airline = null;
        foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
        {
            if (kvp.Key == code)
            {
                airline = kvp.Value;
            }
        }
        if (airline != null)
        {
            Console.WriteLine();
            if (airline.Flights.Count == 0)
            {
                Console.WriteLine($"No flights available for {airline.Name}");
                return null;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"Full flight details for {airline.Name}");
                Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-15} {"Destination",-15} {"Expected Date",-16} {"Expected Time", -17} {"Special Request Code", -25} {"Boarding Gate"}");
                foreach (KeyValuePair<String, Flight> kvp in airline.Flights)
                {
                    Flight flight = kvp.Value;

                    string date = flight.ExpectedTime.ToString("dd/MM/yyyy").Trim();
                    string time = flight.ExpectedTime.ToString("hh:mm tt").Trim();

                    bool sr = false;
                    bool bg = false;
                    if (flight.GetType() == typeof(CFFTFlight) || flight.GetType() == typeof(DDJBFlight) || flight.GetType() == typeof(LWTTFlight))
                    {
                        sr = true;
                    }
                    BoardingGate? boardingGate = FindBoardingGateByFlightNumber(FlightDict, terminal, flight);
                    if (boardingGate != null)
                    {
                        bg = true;
                    }
                    Console.WriteLine();
                    Console.Write($"{flight.FlightNumber,-15} {airline.Name,-25} {flight.Origin,-15} {flight.Destination,-15} {date,-16} {time, -18}");

                    if (sr)
                    {
                        Console.Write($"{GetRequestCode(flight), -26}");
                    } 
                    else
                    {
                        Console.Write($"{"NIL", -26}");
                    }
                    if (bg)
                    {
                        Console.Write(FindBoardingGateByFlightNumber(FlightDict, terminal, flight));
                    }
                    else
                    {
                        Console.Write("NIL");
                    }
                    Console.WriteLine();
                    
                }
                return airline;
            }
        }
        else
        {
            Console.WriteLine("Airline not found");
            continue;
        }
    }

}

void ModifyFlightUserInput(Terminal terminal, Dictionary<string, Flight> FlightDict)
{
    while (true)
    {
        DisplayAirline(terminal);
        Airline airline = DisplayFullFlightFromAirline(Terminal5);
        Console.WriteLine("Enter flight code to modify flight");
        string? input = Console.ReadLine();
        if (input != null)
        {
            input = input.ToUpper();
        }
        else
        {
            Console.WriteLine("Input cannot be null.");
            continue;
        }

        Flight? flight = null;
        foreach (KeyValuePair<string, Flight> kvp in airline.Flights)
        {
            if (kvp.Key == input)
            {
                flight = kvp.Value;
            }
        }
        if (flight == null)
        {
            Console.WriteLine("Invalid flight code");
            continue;
        }

        Console.WriteLine();
        Console.WriteLine($"[1] Modify flight {flight.FlightNumber}");
        Console.WriteLine($"[2] Delete flight {flight.FlightNumber}");
        Console.WriteLine("Enter your choice: ");
        string? modifyInput = Console.ReadLine();

        if (modifyInput == "1")
        {
            ModifyFlightInfo(terminal, flight);
        }
        else if (modifyInput == "2")
        {

        }
        else
        {
            Console.WriteLine("Invalid choice");
            continue;
        }
        

    }
}

void ModifyFlightInfo(Terminal terminal, Flight flight)
{
    
}


// 9 Display scheduled flights from earliest to latest

void DisplayScheduledFlights(Dictionary<string, Flight> FlightDict)
{
    List<Flight> flights = new List<Flight>();
    foreach (KeyValuePair<string, Flight> kvp in FlightDict)
    {
        flights.Add(kvp.Value);
    }
    flights.Sort();
    Console.WriteLine("=============================================\r\nFlight Schedule for Changi Airport Terminal 5\r\n=============================================");
    Console.WriteLine($"{"Flight Number",-17}{"Airline name",-20}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-35}{"Status",-15}{"Boarding Gate",-15}");
    foreach (Flight flight in flights)
    {
        BoardingGate? boardingGate = FindBoardingGateByFlightNumber(FlightDict, Terminal5, flight);
        Console.WriteLine($"{flight.FlightNumber,-17}{AirlineDict[flight.FlightNumber[0..2]].Name,-20}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedTime,-35}{flight.Status,-15}{boardingGate?.GateName ?? "Unassigned",-15}\n");
    }
}
//DisplayScheduledFlights(FlightDict);

BoardingGate? FindBoardingGateByFlightNumber(Dictionary<string, Flight> FlightDict, Terminal Terminal5, Flight targetFlight)
{
    string flightNumber = targetFlight.FlightNumber;
    if (FlightDict.ContainsKey(flightNumber))
    {
        Flight flight = FlightDict[flightNumber];
        Dictionary<string, BoardingGate> BoardingGateDict = Terminal5.BoardingGates;
        foreach (KeyValuePair<string, BoardingGate> kvp in BoardingGateDict)
        {
            if (kvp.Value.Flight == flight)
            {
                //Console.WriteLine($"Flight {flightNumber} is assigned to gate {kvp.Key}");
                return kvp.Value;
            }
        }
        //Console.WriteLine($"Flight {flightNumber} is not assigned to any gate.");
        return null;
    }
    else
    {
        return null;
    }
}