// See https://aka.ms/new-console-template for more information
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

// Add flights to airline
void AddFlightsToAirline(Dictionary<string, Airline> AirlineDict, Dictionary<string, Flight> FlightDict)
{
    foreach (KeyValuePair<string, Flight> kvp in FlightDict)
    {
        AirlineDict[kvp.Value.FlightNumber[0..2]].AddFlight(kvp.Value);
    }
}
AddFlightsToAirline(AirlineDict, FlightDict);

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
    Console.WriteLine($"{"Gate Name",-15} {"DDJB",-20} {"CFFT",-20} {"LWTT"}");
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



        Console.WriteLine($"{kvp.Value.GateName,-15} {DDJB,-20} {CFFT,-20} {LWTT}");


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
    Dictionary<Type, string> requestCodeDict = new Dictionary<Type, string>
    {
        {typeof(CFFTFlight), "CFFT"},
        {typeof(DDJBFlight), "DDJB"},
        {typeof(LWTTFlight), "LWTT"},
        {typeof(NORMFlight), "None"},
        {typeof(Flight), "None"}
    };
    Console.WriteLine("Enter Flight Number: ");
    string flightNumber = Console.ReadLine()?.Trim();
    if (FlightDict.ContainsKey(flightNumber))
    {
        Console.WriteLine("Enter Boarding Gate Name: ");
        string gateName = Console.ReadLine()?.Trim();
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

            while (true)
            {
                Console.WriteLine("Would you like to update the status of the flight? (Y/N)");
                string response = Console.ReadLine()?.Trim();

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
void CreateNewFlight(Dictionary<string, Flight> FlightDict, Dictionary<string, Airline> AirlineDict)
{
    while (true)
    {
        // Validate Flight Number
        string flightNumber;
        while (true)
        {
            Console.Write("Enter Flight Number (e.g., AB 123): ");
            flightNumber = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(flightNumber) &&
    flightNumber.Length > 2 &&
    char.IsLetter(flightNumber[0]) &&
    char.IsLetter(flightNumber[1]) &&
    flightNumber.Substring(2).Trim().All(char.IsDigit))
            {
                if (AirlineDict.ContainsKey(flightNumber[0..2])) {
                    flightNumber = flightNumber[0..2] + " " + flightNumber.Substring(2).Trim();
                    break;
                }
                
            }
            Console.WriteLine("Invalid flight number. Ensure it's not empty and starts with a valid airline code.");
        }

        // Validate Origin
        string origin;
        while (true)
        {
            Console.Write("Enter Origin: ");
            origin = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(origin))
                break;
            Console.WriteLine("Origin cannot be empty.");
        }

        // Validate Destination
        string destination;
        while (true)
        {
            Console.Write("Enter Destination: ");
            destination = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(destination))
                break;
            Console.WriteLine("Destination cannot be empty.");
        }

        // Validate Date and Time
        DateTime expectedTime;
        while (true)
        {
            Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
            string dateTimeInput = Console.ReadLine()?.Trim();
            if (DateTime.TryParseExact(dateTimeInput, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out expectedTime))
                break;
            Console.WriteLine("Invalid date/time format. Please use 'dd/mm/yyyy hh:mm'.");
        }

        // Validate Special Request Code
        string requestCode;
        while (true)
        {
            Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
            requestCode = Console.ReadLine()?.Trim().ToUpper();
            if (requestCode == "CFFT" || requestCode == "DDJB" || requestCode == "LWTT" || requestCode == "NONE")
                break;
            Console.WriteLine("Invalid request code. Please enter 'CFFT', 'DDJB', 'LWTT', or 'None'.");
        }

        // Create flight based on request code
        Flight flight;
        switch (requestCode)
        {
            case "CFFT":
                flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "On Time");
                break;
            case "DDJB":
                flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "On Time");
                break;
            case "LWTT":
                flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "On Time");
                break;
            default:
                flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "On Time");
                break;
        }

        // Add flight to dictionaries and CSV
        FlightDict.Add(flightNumber, flight);
        AirlineDict[flightNumber[0..2]].AddFlight(flight);
        File.AppendAllText("assets/flights.csv", $"{flightNumber},{origin},{destination},{expectedTime},{(requestCode == "NONE" ? "" : requestCode)}\n");

        Console.WriteLine($"Flight {flightNumber} has been added!");

        // Confirm continuation
        Console.WriteLine("Would you like to add another flight? (Y/N)");
        string response = Console.ReadLine()?.Trim().ToUpper();
        if (response == "Y")
        {
            Console.WriteLine("Continuing...");
        }
        else if (response == "N")
        {
            Console.WriteLine("Exiting...");
            AddDataToTerminal(Terminal5);
            break;
        }
        else
        {
            Console.WriteLine("Invalid option. Exiting...");
            AddDataToTerminal(Terminal5);
            break;
        }
    }
}
CreateNewFlight(FlightDict, AirlineDict);


// 7 Display full flight details from an airline
void DisplayAirline(Terminal terminal)
{
    Console.WriteLine();
    Console.WriteLine("=============================================\r\nList of Airlines for Changi Airport Terminal 5\r\n=============================================");
    Console.WriteLine($"{"Airline Code",-20} Airline Name");
    foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
    {
        Console.WriteLine($"{kvp.Key,-20} {kvp.Value.Name}");
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
                Console.WriteLine($"{"Flight Number",-15} {"Origin",-20} {"Destination"}");
                //Console.WriteLine("Departure/Arrival Time");
                foreach (KeyValuePair<string, Flight> kvp in airline.Flights)
                {
                    Flight flight = kvp.Value;
                    Console.WriteLine($"{flight.FlightNumber,-15} {flight.Origin,-20} {flight.Destination,-20}");
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

        foreach (KeyValuePair<String, Flight> kvp in airline.Flights)
        {
            Flight flight = kvp.Value;
            if (kvp.Key == code)
            {
                Console.WriteLine();
                Console.WriteLine($"Full flight details for {flight.FlightNumber}");
                string date = flight.ExpectedTime.ToString("dd/MM/yyyy");
                string time = flight.ExpectedTime.ToString("hh:mm tt") + ":00";
                Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-15} {"Destination",-15} {"Expected Date",-16} {"Expected Time"}");

                bool sr = false;
                bool bg = false;
                //BoardingGate? boardingGate = null;
                if (flight.GetType() == typeof(CFFTFlight) || flight.GetType() == typeof(DDJBFlight) || flight.GetType() == typeof(LWTTFlight))
                {
                    Console.Write($"{"Special Request Code",-30}");
                    sr = true;
                }
                //if (!(flight.GetType() == typeof(NORMFlight))) {
                //    Console.Write($"{"Boarding Gate", -15}");
                //    bg = true;
                //}
                BoardingGate? boardingGate = FindBoardingGateByFlightNumber(FlightDict, terminal, flight);
                if (boardingGate != null)
                {
                    Console.Write($"{"Boarding Gate",-15}");
                    bg = true;
                }
                Console.WriteLine();
                Console.WriteLine($"{flight.FlightNumber,-15} {airline.Name,-25} {flight.Origin,-15} {flight.Destination,-15} {date,-16} {time}");

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

Airline? DisplayFullFlightFromAirline(Terminal terminal)
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
                Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-15} {"Destination",-15} {"Expected Date",-16} {"Expected Time",-17} {"Special Request Code",-25} {"Boarding Gate"}");
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
                    Console.Write($"{flight.FlightNumber,-15} {airline.Name,-25} {flight.Origin,-15} {flight.Destination,-15} {date,-16} {time,-18}");

                    if (sr)
                    {
                        Console.Write($"{GetRequestCode(flight),-26}");
                    }
                    else
                    {
                        Console.Write($"{"NIL",-26}");
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


BoardingGate? GetBoardingGate(Terminal terminal, Flight flight)
{
    foreach (var kvp in terminal.BoardingGates)
    {
        if (kvp.Value.Flight?.FlightNumber == flight.FlightNumber)
        {
            return kvp.Value;
        }
    }

    return null;
}


void ModifyFlightUserInput(Terminal terminal, Dictionary<string, Flight> FlightDict)
{
    while (true)
    {
        DisplayAirline(terminal);
        Airline? airline = DisplayFullFlightFromAirline(Terminal5);
        if (airline == null)
        {
            continue;
        }
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
        Console.WriteLine("Choose an option: ");
        string? modifyInput = Console.ReadLine();

        if (modifyInput == "1")
        {
            ModifyFlightInfo(terminal, flight, FlightDict, airline);
            return;
        }
        else if (modifyInput == "2")
        {
            DeleteFlight(terminal, flight, FlightDict);
            return;
        }
        else
        {
            Console.WriteLine("Invalid choice");
            continue;
        }
    }


}

void DeleteFlight(Terminal terminal, Flight flight, Dictionary<string, Flight> FlightDict)
{
    Console.WriteLine($"Are you sure you want to delete Flight Number: {flight.FlightNumber}? (Y/N)");
    string confirmation = Console.ReadLine()?.Trim().ToUpper();

    if (confirmation == "Y")
    {
        if (terminal.Flights.ContainsKey(flight.FlightNumber))
        {
            terminal.Flights.Remove(flight.FlightNumber);
            Console.WriteLine("Flight removed from terminal.");
        }

        foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
        {
            Airline airline = kvp.Value;
            if (airline.Flights.ContainsKey(flight.FlightNumber))
            {
                airline.Flights.Remove(flight.FlightNumber);
                Console.WriteLine($"Flight removed from {airline.Name}.");
                break;
            }
        }

        foreach (KeyValuePair<string, BoardingGate> kvp in terminal.BoardingGates)
        {
            BoardingGate gate = kvp.Value;
            if (gate.Flight?.FlightNumber == flight.FlightNumber)
            {
                gate.Flight = null;
                Console.WriteLine("Flight unassigned from boarding gate");
                break;
            }
        }

        FlightDict.Remove(flight.FlightNumber);
        Console.WriteLine("Flight deleted successfully");
    }
    else
    {
        Console.WriteLine("Flight deletion canceled");
    }
}



void ModifyFlightInfo(Terminal terminal, Flight flight, Dictionary<string, Flight> FlightDict, Airline airline)
{
    while (true)
    {
        Console.WriteLine("1. Modify Basic Information\r\n2. Modify Status\r\n3. Modify Special Request Code\r\n4. Modify Boarding Gate");
        Console.WriteLine("Choose an option:");
        string? input = Console.ReadLine();
        if (input == "1")
        {
            ModifyBasicInformation(flight);
            break;
        }
        else if (input == "2")
        {
            ModifyStatus(flight);
            break;
        }
        else if (input == "3")
        {
            ModifySpecialRequestCode(flight, terminal, FlightDict);
            break;
        }
        else if (input == "4")
        {
            ModifyBoardingGate(flight, terminal);
            break;
        }
        else
        {
            Console.WriteLine("Invalid Input");
            continue;
        }
    }

    Console.WriteLine("Flight updated!");
    Console.WriteLine($"Flight number: {flight.FlightNumber}");
    Console.WriteLine($"Airline name: {airline.Name}");
    Console.WriteLine($"Origin: {flight.Origin}");
    Console.WriteLine($"Destination: {flight.Destination}");
    Console.WriteLine($"Expected Departure/Arrival Time: {flight.ExpectedTime}");
    Console.WriteLine($"Status: {flight.Status}");
    Console.WriteLine($"Special request code: {GetRequestCode(flight)}");
    BoardingGate? boardingGate = GetBoardingGate(terminal, flight);
    if (boardingGate != null)
    {
        Console.WriteLine($"Boarding gate: {boardingGate.GateName}");
    }
    else
    {
        Console.WriteLine($"Boarding gate: unassigned");
    }
}

void ModifyBasicInformation(Flight flight)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("Enter new Origin: ");
        string? origin = Console.ReadLine();
        if (origin == null)
        {
            Console.WriteLine("Invalid origin");
            continue;
        }

        Console.WriteLine("Enter new Destination: ");
        string? destination = Console.ReadLine();
        if (destination == null)
        {
            Console.WriteLine("Invalid destination");
            continue;
        }

        Console.WriteLine("Enter new Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
        DateTime? expectedTime = null;
        try
        {
            expectedTime = Convert.ToDateTime(Console.ReadLine());

        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid Input, use the correct format (dd/MM/yyyy HH:mm).");
            continue;
        }
        if (expectedTime == null)
        {
            Console.WriteLine("Invalid Expected Departure/Arrival Time");
            continue;
        }

        flight.Origin = origin;
        flight.Destination = destination;
        flight.ExpectedTime = expectedTime.Value;

        return;
    }
}

void ModifyStatus(Flight flight)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("Select a status for the flight:");
        Console.WriteLine("1. On Time");
        Console.WriteLine("2. Delayed");
        Console.WriteLine("3. Boarding");
        Console.WriteLine("4. Enter Custom Status");

        string? input = Console.ReadLine();

        if (input == "1")
        {
            flight.Status = "On Time";
            break;
        }
        else if (input == "2")
        {
            flight.Status = "Delayed";
            break;
        }
        else if (input == "3")
        {
            flight.Status = "Boarding";
            break;
        }
        else if (input == "4")
        {
            Console.WriteLine("Enter custom status: ");
            string? status = Console.ReadLine();

            if (status != null)
            {
                flight.Status = status;
                return;
            }
            else
            {
                Console.WriteLine("Invalid custom status");
                continue;
            }
        }
        else
        {
            Console.WriteLine("Invalid option. Pls enter a number between 1 and 4.");
        }
    }
}


void ModifySpecialRequestCode(Flight flight, Terminal terminal, Dictionary<string, Flight> flightDict)
{
    string flightNumber = flight.FlightNumber;
    Flight newFlight = null;

    while (true)
    {
        Console.WriteLine("Please select the new special request code for the flight:");
        Console.WriteLine("[1] CFFT Flight");
        Console.WriteLine("[2] LWTT Flight");
        Console.WriteLine("[3] DDJB Flight");
        Console.WriteLine("[4] NORM Flight");

        string input = Console.ReadLine()?.Trim();

        if (input == null)
        {
            Console.WriteLine("Input cannot be null");
            continue;
        }

        input = input.ToUpper();
        if (input == "1")
        {
            newFlight = new CFFTFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedTime, flight.Status);
            break;
        }
        else if (input == "2")
        {
            newFlight = new LWTTFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedTime, flight.Status);
            break;
        }
        else if (input == "3")
        {
            newFlight = new DDJBFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedTime, flight.Status);
            break;
        }
        else if (input == "4")
        {
            newFlight = new NORMFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedTime, flight.Status);
            break;
        }
        else
        {
            Console.WriteLine("Invalid input");
            continue;
        }
    }

    foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
    {
        Airline airline = kvp.Value;
        if (airline.Flights.ContainsKey(flightNumber))
        {
            airline.Flights.Remove(flightNumber);
            airline.Flights[flightNumber] = newFlight;
        }
    }

    foreach (KeyValuePair<string, BoardingGate> kvp in terminal.BoardingGates)
    {
        BoardingGate gate = kvp.Value;
        if (gate.Flight != null && gate.Flight.FlightNumber == flightNumber)
        {
            gate.Flight = newFlight;
        }
    }

    foreach (KeyValuePair<string, Flight> kvp in terminal.Flights)
    {
        Flight oldflight = kvp.Value;
        if (oldflight.FlightNumber == flightNumber)
        {
            terminal.Flights[kvp.Key] = newFlight;
            Console.WriteLine($"Flight with number {flightNumber} has been updated.");
            break;
        }
    }



    if (flightDict.ContainsKey(flightNumber))
    {
        flightDict.Remove(flightNumber);
        flightDict[flightNumber] = newFlight;
    }
}

void ModifyBoardingGate(Flight flight, Terminal terminal)
{
    while (true)
    {
        Console.WriteLine("Enter the new boarding gate number:");

        string input = Console.ReadLine()?.Trim();

        if (input == null || input == "")
        {
            continue;
        }

        if (!terminal.BoardingGates.ContainsKey(input))
        {
            continue;
        }

        // Get the current gate that the flight is assigned to
        BoardingGate currentGate = null;
        foreach (KeyValuePair<string, BoardingGate> kvp in terminal.BoardingGates)
        {
            BoardingGate gate = kvp.Value;
            if (gate.Flight != null && gate.Flight.FlightNumber == flight.FlightNumber)
            {
                currentGate = gate;
                break;
            }
        }

        // Get the new gate where the flight will be assigned
        BoardingGate newGate = terminal.BoardingGates[input];

        // Free the current gate if it was assigned
        if (currentGate != null)
        {
            currentGate.Flight = null;
        }

        newGate.Flight = flight;

        break;
    }
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