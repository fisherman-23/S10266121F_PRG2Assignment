// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using TeamRanenJingShun;

// Temporary code to test the classes
//Flight fl = new NORMFlight("SQ123", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl2 = new CFFTFlight("SQ122", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl3 = new CFFTFlight("SQ121", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl4 = new CFFTFlight("SQ120", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl5 = new CFFTFlight("SQ119", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl6 = new CFFTFlight("SQ118", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Console.WriteLine(fl.CalculateFees());
//Console.WriteLine(fl2.CalculateFees());
//Airline al = new Airline("SQ", "123");
//Airline al2 = new Airline("SQ", "122");

//al.AddFlight(fl);
//al.AddFlight(fl2);
//al.AddFlight(fl3);
//al.AddFlight(fl4);
//al.AddFlight(fl5);
//al.AddFlight(fl6);


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
DisplayFlights(FlightDict, AirlineDict);


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

DisplayBoardingGates(Terminal5);


// 7 Display full flight details from an airline
void DisplayAirline(Terminal terminal)
{
    Console.WriteLine("=============================================\r\nList of Airlines for Changi Airport Terminal 5\r\n=============================================");
    Console.WriteLine($"{"Airline Code", -20} Airline Name");
    foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
    {
        Console.WriteLine($"{kvp.Key, -20} {kvp.Value.Name}");
    }
}

void DisplayFlightFromAirline(Terminal terminal)
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
            if (airline.Flights.Count == 0)
            {
                Console.WriteLine($"No flights available for {airline.Name}");

            }
            else
            {
                Console.WriteLine($"{"Airline Number", -10} {"Origin", -20} {"Destination"}");
                Console.WriteLine("Departure/Arrival Time");
                foreach (KeyValuePair<string, Flight> kvp in airline.Flights)
                {
                    Flight flight = kvp.Value;
                    Console.WriteLine($"{flight.FlightNumber, -10} {flight.Origin, -25} {flight.Destination, -20}");
                }
                DisplayFlightDetails(airline);
            }
            break;
        }
        else
        {
            Console.WriteLine("Airline not found");
            continue;
        }
    }

}

DisplayFlightFromAirline(Terminal5);

void DisplayFlightDetails(Airline airline)
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
                Console.WriteLine($"Full flight details for {flight.FlightNumber}");
                string date = flight.ExpectedTime.ToString("dd/MM/yyyy");
                string time = flight.ExpectedTime.ToString("hh:mm tt") + ":00";
                Console.WriteLine($"{"Flight Number", -10} {"Airline Name", -25} {"Origin", -15} {"Destination", -15} {"Expected", -10}");

                bool sr = false;
                bool bg = false;
                BoardingGate? boardingGate = null;
                if (flight.GetType() == typeof(CFFTFlight) || flight.GetType() == typeof(DDJBFlight) || flight.GetType() == typeof(LWTTFlight))
                {
                    Console.Write($"{"Special Request Code",-20}");
                    sr = true;
                }
                if (!(flight.GetType() == typeof(NORMFlight))) {
                    Console.Write($"{"Boarding Gate", -15}");
                    bg = true;
                }
                Console.WriteLine();
                Console.WriteLine($"{flight.FlightNumber, -10} {airline.Name, -25} {flight.Origin, -15} {flight.Destination, -15} {date}");
                Console.WriteLine(time);
                if (sr)
                {
                    Console.Write($"{GetRequestCode(flight),-20}");
                }
                if (bg)
                {
                    Console.Write($"{GetBoardingGate(flight)}");
                }
                return;
            }
        }
    }
}

string[]? GetBoardingGate(Flight flight)
{
    if (flight is CFFTFlight)
    {
        return new string[] { "B1", "B2", "B3", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10", "C11", "C12", "C13", "C14", "C15", "C16", "C17", "C18", "C19", "C20", "C21", "C22" };
    }
    else if (flight is DDJBFlight)
    {
        return new string[] { "A10", "A11", "A12", "A13", "A20", "A21", "A22", "B10", "B11", "B12" };
    }
    else if (flight is LWTTFlight)
    {
        return new string[] { "A1", "A2", "A20", "A21", "A22", "C14", "C15", "C16", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10", "B11", "B12", "B13", "B14", "B15", "B16", "B17", "B18", "B19", "B20", "B21", "B22" };
    }
    return null;
}

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