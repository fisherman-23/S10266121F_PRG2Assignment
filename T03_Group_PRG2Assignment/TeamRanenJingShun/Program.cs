﻿// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using TeamRanenJingShun;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Speech.Synthesis;

//// Temporary code to test the classes
//Flight fl = new NORMFlight("SQ123", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl2 = new CFFTFlight("SQ122", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl3 = new CFFTFlight("SQ121", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl4 = new CFFTFlight("SQ120", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl5 = new CFFTFlight("SQ119", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
//Flight fl6 = new CFFTFlight("SQ118", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");
////speakWriteLine(fl.CalculateFees());
////speakWriteLine(fl2.CalculateFees());
//Airline al = new Airline("SQ", "123");
//Airline al2 = new Airline("SQ", "122");

//al.AddFlight(fl);
//al.AddFlight(fl2);
//al.AddFlight(fl3);
//al.AddFlight(fl4);
//al.AddFlight(fl5);
//al.AddFlight(fl6);



//speakWriteLine(al.CalculateFees());
//speakWriteLine(al2.CalculateFees());

// Menu 
// Initialise data
string[] AirlinesCSVLines = File.ReadAllLines("assets/airlines.csv");
Dictionary<string, Airline> AirlineDict = new Dictionary<string, Airline>();
Dictionary<string, BoardingGate> BoardingGateDict = new Dictionary<string, BoardingGate>();
LoadFiles(AirlineDict, BoardingGateDict);

Dictionary<string, Flight> FlightDict = new Dictionary<string, Flight>();
LoadFlights(FlightDict);

Terminal Terminal5 = new Terminal("Terminal5");
AddFlightsToAirline(AirlineDict, FlightDict);
AddDataToTerminal(Terminal5);

SpeechSynthesizer s = new SpeechSynthesizer();
bool SpeechToggle = false;


// Start Menu Loop
bool loopContinue = true;
while (loopContinue)
{
    speakWriteLine("=============================================\nWelcome to Changi Airport Terminal 5\n=============================================");
    speakWriteLine($"1. List All Flights\n2. List Boarding Gates\n3. Assign a boarding gate to a flight\n4. Create flight\n5. Display airline flights\n6. Modify flight details\n7. Display Flight Schedule\n8. Exit\n9. Auto assign flights to gates\n10. Display total fees per airline for the day\n11. Get arrival destination weather\n12. Toggle text to speech: {SpeechToggle}\n13. Modify text to speech settings");

    speakWriteLine("Please select your option: ");
    string? input = Console.ReadLine().Trim();

    switch (input)
    {
        case "1":
            DisplayFlights(FlightDict, AirlineDict);
            break;
        case "2":
            DisplayBoardingGates(Terminal5);
            break;
        case "3":
            AssignBoardingGateToFlight(Terminal5, FlightDict, BoardingGateDict);
            break;
        case "4":
            CreateNewFlight(FlightDict, AirlineDict);
            break;
        case "5":
            DisplayAirline(Terminal5);
            DisplayFlightDetails(DisplayFlightFromAirline(Terminal5), Terminal5, FlightDict);
            break;
        case "6":
            ModifyFlightUserInput(Terminal5, FlightDict);
            break;
        case "7":
            DisplayScheduledFlights(FlightDict);
            break;
        case "8":
            speakWriteLine("Exiting...");
            loopContinue = false;
            break;
        case "9":
            processUnassignedFlights(FlightDict, Terminal5);
            break;
        case "10":
            DisplayTotalFees(Terminal5, FlightDict);
            break;
        case "11":
            getWeatherGivenFlight(FlightDict);
            break;
        case "12":
            SpeechToggle = ToggleSpeak(SpeechToggle);
            break;
        case "13":
            s = ModifySpeech(s);
            break;
        default:
            speakWriteLine("Invalid option. Please try again.");
            break;
    }
}


//1 Load files (airlines and boarding gates)
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

//2 Load files (flights)
void LoadFlights(Dictionary<string, Flight> FlightDict)
{
    string[] FlightsCSVLines = File.ReadAllLines("assets/flights.csv")[1..];
    foreach (string line in FlightsCSVLines)
    {
        string[] value = line.Split(",");
        string requestCode = value[4];
        if (requestCode == "CFFT")
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
        else
        {
            Flight flight = new NORMFlight(value[0], value[1], value[2], DateTime.Parse(value[3]), "On Time");
            FlightDict.Add(value[0], flight);
        }

    }
}


//3 List all flights
void DisplayFlights(Dictionary<string, Flight> FlightDict, Dictionary<string, Airline> AirlineDict)
{
    speakWriteLine("=============================================\r\nList of Flights for Changi Airport Terminal 5\r\n=============================================");
    speakWriteLine($"{"Flight Number",-17}{"Airline name",-20}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-30}");
    foreach (KeyValuePair<string, Flight> kvp in FlightDict)
    {
        speakWriteLine($"{kvp.Value.FlightNumber,-17}{AirlineDict[kvp.Value.FlightNumber[0..2]].Name,-20}{kvp.Value.Origin,-20}{kvp.Value.Destination,-20}{kvp.Value.ExpectedTime,-30}");
        speakWriteLine();
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

//4 List all boarding gates
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

void DisplayBoardingGates(Terminal terminal)
{
    speakWriteLine("=============================================\r\nList of Boarding Gates for Changi Airport Terminal 5\r\n=============================================");
    speakWriteLine($"{"Gate Name",-15} {"DDJB",-20} {"CFFT",-20} {"LWTT",-20} {"Flight assigned"}");
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

        string? flightdisplay = null;
        if (kvp.Value.Flight == null)
        {
            flightdisplay = "NIL";
        }
        else
        {
            flightdisplay = kvp.Value.Flight.FlightNumber;
        }


        speakWriteLine($"{kvp.Value.GateName,-15} {DDJB,-20} {CFFT,-20} {LWTT,-20} {flightdisplay}");


        //if (kvp.Value.Flight != null)
        //{
        //    speakWriteLine($"Assigned Flight Number: {kvp.Value.Flight.FlightNumber}");
        //}
        //else
        //{
        //    speakWriteLine("No flight assigned to this gate.");
        //}

        speakWriteLine();
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
    speakWrite("Enter Flight Number: ");
    string flightNumber = Console.ReadLine()?.Trim();
    if (FlightDict.ContainsKey(flightNumber))
    {
        Flight flight = FlightDict[flightNumber];
        speakWriteLine($"Flight Number: {flight.FlightNumber}");
        speakWriteLine($"Origin: {flight.Origin}");
        speakWriteLine($"Destination: {flight.Destination}");
        speakWriteLine($"Expected Time: {flight.ExpectedTime}");
        speakWriteLine($"Special Request Code: {requestCodeDict[flight.GetType()]}\n");

        speakWrite("Enter Boarding Gate Name: ");
        string gateName = Console.ReadLine()?.Trim();
        if (BoardingGateDict.ContainsKey(gateName))
        {


            speakWriteLine($"Boarding Gate Name: {gateName}");
            speakWriteLine($"Supports DDJB: {BoardingGateDict[gateName].SupportsDDJB}");
            speakWriteLine($"Supports CFFT: {BoardingGateDict[gateName].SupportsCFFT}");
            speakWriteLine($"Supports LWTT: {BoardingGateDict[gateName].SupportsLWTT}");


            if (Terminal5.BoardingGates[gateName].Flight != null)
            {
                speakWriteLine("This gate is already assigned to a flight.");
                return;

            }
            else if (!IsGateCompatible(Terminal5.BoardingGates[gateName], flight))
            {
                speakWriteLine("This gate is not compatible with the flight.");
                return;
            }





            while (true)
            {
                speakWriteLine("Would you like to update the status of the flight? (Y/N)");
                string response = Console.ReadLine()?.Trim();

                if (response == "Y")
                {
                    while (true)
                    {
                        speakWriteLine("1. Delayed\n2. Boarding\n3. On time");

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
                            speakWriteLine("Invalid status.");
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
                    speakWriteLine("Invalid response.");
                }
            }


            Terminal5.BoardingGates[gateName].Flight = flight;
            speakWriteLine($"Flight {flight.FlightNumber} has been assigned to gate {gateName}.");
        }
        else
        {
            speakWriteLine("Invalid gate name.");
        }
    }
    else
    {
        speakWriteLine("Invalid flight number.");
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
            speakWrite("Enter Flight Number (e.g., AB 123): ");
            flightNumber = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(flightNumber) &&
    flightNumber.Length > 2 &&
    char.IsLetter(flightNumber[0]) &&
    char.IsLetter(flightNumber[1]) &&
    flightNumber.Substring(2).Trim().All(char.IsDigit))
            {
                if (AirlineDict.ContainsKey(flightNumber[0..2]))
                {
                    flightNumber = flightNumber[0..2] + " " + flightNumber.Substring(2).Trim();
                    break;
                }

            }
            speakWriteLine("Invalid flight number. Ensure it's not empty and starts with a valid airline code.");
        }

        // Validate Origin
        string origin;
        while (true)
        {
            speakWrite("Enter Origin: ");
            origin = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(origin))
                break;
            speakWriteLine("Origin cannot be empty.");
        }

        // Validate Destination
        string destination;
        while (true)
        {
            speakWrite("Enter Destination: ");
            destination = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(destination))
                break;
            speakWriteLine("Destination cannot be empty.");
        }

        // Validate Date and Time
        DateTime expectedTime;
        while (true)
        {
            speakWrite("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
            string dateTimeInput = Console.ReadLine()?.Trim();
            if (DateTime.TryParseExact(dateTimeInput, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out expectedTime))
                break;
            speakWriteLine("Invalid date/time format. Please use 'dd/mm/yyyy hh:mm'.");
        }

        // Validate Special Request Code
        string requestCode;
        while (true)
        {
            speakWrite("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
            requestCode = Console.ReadLine()?.Trim().ToUpper();
            if (requestCode == "CFFT" || requestCode == "DDJB" || requestCode == "LWTT" || requestCode == "NONE")
                break;
            speakWriteLine("Invalid request code. Please enter 'CFFT', 'DDJB', 'LWTT', or 'None'.");
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

        speakWriteLine($"Flight {flightNumber} has been added!");

        // Confirm continuation
        speakWriteLine("Would you like to add another flight? (Y/N)");
        string response = Console.ReadLine()?.Trim().ToUpper();
        if (response == "Y")
        {
            speakWriteLine("Continuing...");
        }
        else if (response == "N")
        {
            speakWriteLine("Exiting...");
            AddDataToTerminal(Terminal5);
            break;
        }
        else
        {
            speakWriteLine("Invalid option. Exiting...");
            AddDataToTerminal(Terminal5);
            break;
        }
    }
}


// 7 Display full flight details from an airline
void DisplayAirline(Terminal terminal)
{
    speakWriteLine();
    speakWriteLine("=============================================\r\nList of Airlines for Changi Airport Terminal 5\r\n=============================================");
    speakWriteLine($"{"Airline Code",-20} Airline Name");
    foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
    {
        speakWriteLine($"{kvp.Key,-20} {kvp.Value.Name}");
    }
}

Airline DisplayFlightFromAirline(Terminal terminal)
{
    string? code = null;
    while (true)
    {
        speakWriteLine();
        speakWriteLine("Enter Airline Code: ");
        try
        {
            code = Console.ReadLine().ToUpper();
        }
        catch
        {
            speakWriteLine("Invalid input, please try again.");
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
            speakWriteLine();
            if (airline.Flights.Count == 0)
            {
                speakWriteLine($"No flights available for {airline.Name}");

            }
            else
            {
                speakWriteLine($"{"Flight Number",-15} {"Origin",-20} {"Destination"}");
                //speakWriteLine("Departure/Arrival Time");
                foreach (KeyValuePair<string, Flight> kvp in airline.Flights)
                {
                    Flight flight = kvp.Value;
                    speakWriteLine($"{flight.FlightNumber,-15} {flight.Origin,-20} {flight.Destination,-20}");
                }
                return airline;
            }
        }
        else
        {
            speakWriteLine("Airline not found");
            continue;
        }
    }

}

//Terminal5.AddAirline(al);
//DisplayAirline(Terminal5);
//DisplayFlightDetails(DisplayFlightFromAirline(Terminal5), Terminal5, FlightDict);


void DisplayFlightDetails(Airline airline, Terminal terminal, Dictionary<string, Flight> FlightDict)
{
    string? code = null;
    while (true)
    {
        speakWriteLine();
        speakWriteLine("Enter flight number: ");
        try
        {
            code = Console.ReadLine().ToUpper();
        }
        catch
        {
            speakWriteLine("Invalid input, please try again.");
            continue;
        }

        foreach (KeyValuePair<string, Flight> kvp in airline.Flights)
        {
            Flight flight = kvp.Value;
            if (kvp.Key == code)
            {
                speakWriteLine();
                speakWriteLine($"Full flight details for {flight.FlightNumber}");
                string date = flight.ExpectedTime.ToString("dd/MM/yyyy");
                string time = flight.ExpectedTime.ToString("hh:mm tt") + ":00";
                speakWriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-15} {"Destination",-20} {"Expected Date",-16} {"Expected Time"}");

                bool sr = false;
                bool bg = false;
                //BoardingGate? boardingGate = null;
                if (flight.GetType() == typeof(CFFTFlight) || flight.GetType() == typeof(DDJBFlight) || flight.GetType() == typeof(LWTTFlight))
                {
                    speakWrite($"{"Special Request Code",-30}");
                    sr = true;
                }
                //if (!(flight.GetType() == typeof(NORMFlight))) {
                //    speakWrite($"{"Boarding Gate", -15}");
                //    bg = true;
                //}
                BoardingGate? boardingGate = FindBoardingGateByFlightNumber(FlightDict, terminal, flight);
                if (boardingGate != null)
                {
                    speakWrite($"{"Boarding Gate",-15}");
                    bg = true;
                }
                speakWriteLine();
                speakWriteLine($"{flight.FlightNumber,-15} {airline.Name,-25} {flight.Origin,-15} {flight.Destination,-20} {date,-16} {time}");

                if (sr)
                {
                    speakWriteLine($"{GetRequestCode(flight),-30}");
                }
                if (bg)
                {
                    speakWriteLine($"{FindBoardingGateByFlightNumber(FlightDict, terminal, flight)}");
                    //foreach (string gate in GetBoardingGate(flight))
                    //{
                    //    speakWrite(gate + " ");
                    //}
                }
                speakWriteLine();
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

Airline? DisplayFullFlightFromAirline(Terminal terminal)
{
    string? code = null;
    while (true)
    {
        speakWriteLine();
        speakWriteLine("Enter Airline Code: ");
        try
        {
            code = Console.ReadLine().ToUpper();
        }
        catch
        {
            speakWriteLine("Invalid input, please try again.");
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
            speakWriteLine();
            if (airline.Flights.Count == 0)
            {
                speakWriteLine($"No flights available for {airline.Name}");
                return null;
            }
            else
            {
                speakWriteLine();
                speakWriteLine($"Full flight details for {airline.Name}");
                speakWriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-15} {"Destination",-15} {"Expected Date",-16} {"Expected Time",-17} {"Special Request Code",-25} {"Boarding Gate"}");
                foreach (KeyValuePair<string, Flight> kvp in airline.Flights)
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
                    speakWriteLine();
                    speakWrite($"{flight.FlightNumber,-15} {airline.Name,-25} {flight.Origin,-15} {flight.Destination,-15} {date,-16} {time,-18}");

                    if (sr)
                    {
                        speakWrite($"{GetRequestCode(flight),-26}");
                    }
                    else
                    {
                        speakWrite($"{"NIL",-26}");
                    }
                    if (bg)
                    {
                        speakWrite($"{FindBoardingGateByFlightNumber(FlightDict, terminal, flight)}");
                    }
                    else
                    {
                        speakWrite("NIL");
                    }
                    speakWriteLine();

                }
                return airline;
            }
        }
        else
        {
            speakWriteLine("Airline not found");
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
        speakWriteLine("Enter flight code to modify flight");
        string? input = Console.ReadLine();
        if (input != null)
        {
            input = input.ToUpper();
        }
        else
        {
            speakWriteLine("Input cannot be null.");
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
            speakWriteLine("Invalid flight code");
            continue;
        }

        speakWriteLine();
        speakWriteLine($"[1] Modify flight {flight.FlightNumber}");
        speakWriteLine($"[2] Delete flight {flight.FlightNumber}");
        speakWriteLine("Choose an option: ");
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
            speakWriteLine("Invalid choice");
            continue;
        }
    }


}

void DeleteFlight(Terminal terminal, Flight flight, Dictionary<string, Flight> FlightDict)
{
    speakWriteLine($"Are you sure you want to delete Flight Number: {flight.FlightNumber}? (Y/N)");
    string confirmation = Console.ReadLine()?.Trim().ToUpper();

    if (confirmation == "Y")
    {
        if (terminal.Flights.ContainsKey(flight.FlightNumber))
        {
            terminal.Flights.Remove(flight.FlightNumber);
            speakWriteLine("Flight removed from terminal.");
        }

        foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
        {
            Airline airline = kvp.Value;
            if (airline.Flights.ContainsKey(flight.FlightNumber))
            {
                airline.Flights.Remove(flight.FlightNumber);
                speakWriteLine($"Flight removed from {airline.Name}.");
                break;
            }
        }

        foreach (KeyValuePair<string, BoardingGate> kvp in terminal.BoardingGates)
        {
            BoardingGate gate = kvp.Value;
            if (gate.Flight?.FlightNumber == flight.FlightNumber)
            {
                gate.Flight = null;
                speakWriteLine("Flight unassigned from boarding gate");
                break;
            }
        }

        FlightDict.Remove(flight.FlightNumber);
        speakWriteLine("Flight deleted successfully");
    }
    else
    {
        speakWriteLine("Flight deletion canceled");
    }
}



void ModifyFlightInfo(Terminal terminal, Flight flight, Dictionary<string, Flight> FlightDict, Airline airline)
{
    while (true)
    {
        speakWriteLine("1. Modify Basic Information\r\n2. Modify Status\r\n3. Modify Special Request Code\r\n4. Modify Boarding Gate");
        speakWriteLine("Choose an option:");
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
            speakWriteLine("Invalid Input");
            continue;
        }
    }

    speakWriteLine("Flight updated!");
    speakWriteLine($"Flight number: {flight.FlightNumber}");
    speakWriteLine($"Airline name: {airline.Name}");
    speakWriteLine($"Origin: {flight.Origin}");
    speakWriteLine($"Destination: {flight.Destination}");
    speakWriteLine($"Expected Departure/Arrival Time: {flight.ExpectedTime}");
    speakWriteLine($"Status: {flight.Status}");
    speakWriteLine($"Special request code: {GetRequestCode(flight)}");
    BoardingGate? boardingGate = GetBoardingGate(terminal, flight);
    if (boardingGate != null)
    {
        speakWriteLine($"Boarding gate: {boardingGate.GateName}");
    }
    else
    {
        speakWriteLine($"Boarding gate: unassigned");
    }
}

void ModifyBasicInformation(Flight flight)
{
    while (true)
    {
        speakWriteLine();
        speakWriteLine("Enter new Origin: ");
        string? origin = Console.ReadLine();
        if (origin == null)
        {
            speakWriteLine("Invalid origin");
            continue;
        }

        speakWriteLine("Enter new Destination: ");
        string? destination = Console.ReadLine();
        if (destination == null)
        {
            speakWriteLine("Invalid destination");
            continue;
        }

        speakWriteLine("Enter new Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
        DateTime? expectedTime = null;
        try
        {
            expectedTime = Convert.ToDateTime(Console.ReadLine());

        }
        catch (FormatException)
        {
            speakWriteLine("Invalid Input, use the correct format (dd/MM/yyyy HH:mm).");
            continue;
        }
        if (expectedTime == null)
        {
            speakWriteLine("Invalid Expected Departure/Arrival Time");
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
        speakWriteLine();
        speakWriteLine("Select a status for the flight:");
        speakWriteLine("1. On Time");
        speakWriteLine("2. Delayed");
        speakWriteLine("3. Boarding");
        speakWriteLine("4. Enter Custom Status");

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
            speakWriteLine("Enter custom status: ");
            string? status = Console.ReadLine();

            if (status != null)
            {
                flight.Status = status;
                return;
            }
            else
            {
                speakWriteLine("Invalid custom status");
                continue;
            }
        }
        else
        {
            speakWriteLine("Invalid option. Pls enter a number between 1 and 4.");
        }
    }
}


void ModifySpecialRequestCode(Flight flight, Terminal terminal, Dictionary<string, Flight> flightDict)
{
    string flightNumber = flight.FlightNumber;
    Flight newFlight = null;

    while (true)
    {
        speakWriteLine("Please select the new special request code for the flight:");
        speakWriteLine("[1] CFFT Flight");
        speakWriteLine("[2] LWTT Flight");
        speakWriteLine("[3] DDJB Flight");
        speakWriteLine("[4] NORM Flight");

        string input = Console.ReadLine()?.Trim();

        if (input == null)
        {
            speakWriteLine("Input cannot be null");
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
            speakWriteLine("Invalid input");
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
            speakWriteLine($"Flight with number {flightNumber} has been updated.");
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
        speakWriteLine("Enter the new boarding gate number:");

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
    Dictionary<Type, string> requestCodeDict = new Dictionary<Type, string>
    {
        {typeof(CFFTFlight), "CFFT"},
        {typeof(DDJBFlight), "DDJB"},
        {typeof(LWTTFlight), "LWTT"},
        {typeof(NORMFlight), "None"},
        {typeof(Flight), "None"}
    };
    List<Flight> flights = new List<Flight>();
    foreach (KeyValuePair<string, Flight> kvp in FlightDict)
    {
        flights.Add(kvp.Value);
    }
    flights.Sort();
    speakWriteLine("=============================================\r\nFlight Schedule for Changi Airport Terminal 5\r\n=============================================");
    speakWriteLine($"{"Flight Number",-17}{"Airline name",-20}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-35}{"Status",-15}{"Special Code",-15}{"Boarding Gate",-15}");
    foreach (Flight flight in flights)
    {
        BoardingGate? boardingGate = FindBoardingGateByFlightNumber(FlightDict, Terminal5, flight);
        speakWriteLine($"{flight.FlightNumber,-17}{AirlineDict[flight.FlightNumber[0..2]].Name,-20}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedTime,-35}{flight.Status,-15}{requestCodeDict[flight.GetType()],-15}{boardingGate?.GateName ?? "Unassigned",-15}\n");
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
                //speakWriteLine($"Flight {flightNumber} is assigned to gate {kvp.Key}");
                return kvp.Value;
            }
        }
        //speakWriteLine($"Flight {flightNumber} is not assigned to any gate.");
        return null;
    }
    else
    {
        return null;
    }
}

// Advanced Feature (A): Ooi Jing Shun 
void processUnassignedFlights(Dictionary<string, Flight> FlightDict, Terminal Terminal5)
{
    Queue<Flight> unassignedFlights = new Queue<Flight>();
    List<BoardingGate> unassignedGates = new List<BoardingGate>();

    foreach (KeyValuePair<string, Flight> kvp in FlightDict)
    {
        BoardingGate? result = FindBoardingGateByFlightNumber(FlightDict, Terminal5, kvp.Value);
        if (result == null)
        {
            unassignedFlights.Enqueue(kvp.Value);
        }
    }
    int initialFlightCount = unassignedFlights.Count;
    speakWriteLine($"Total number of flights that are unassigned to boarding gates: {initialFlightCount}");
    foreach (KeyValuePair<string, BoardingGate> kvp in Terminal5.BoardingGates)
    {
        if (kvp.Value.Flight == null)
        {
            unassignedGates.Add(kvp.Value);
        }
    }
    int initialGateCount = unassignedGates.Count;
    speakWriteLine($"Total number of unassigned boarding gates: {initialGateCount}");

    int i = 0;
    while (unassignedFlights.Count > 0 && unassignedGates.Count > 0)
    {
        Flight flight = unassignedFlights.Dequeue();
        i = 0;
        while (true)
        {
            if (IsGateCompatible(unassignedGates[i], flight))
            {
                Terminal5.BoardingGates[unassignedGates[i].GateName].Flight = flight;

                speakWriteLine($"Flight: {flight.FlightNumber}, Airline: {Terminal5.Airlines[flight.FlightNumber[0..2]].Name}, Origin: {flight.Origin}, Destination: {flight.Destination}, Departure/Arrival Time: {flight.ExpectedTime.ToString("dd/MM/yyyy hh:mm tt:00")}, has been assigned to gate {unassignedGates[i].GateName}\n");
                unassignedGates.RemoveAt(i);
                break;
            }

            else
            {
                i++;
            }
        }


    }
    speakWriteLine($"Total number of flights that are processed and assigned to boarding gates: {initialFlightCount - unassignedFlights.Count}");
    speakWriteLine($"Total number of boarding gates that are processed and assigned to flights: {initialGateCount - unassignedGates.Count}");
    if (FlightDict.Count - initialFlightCount == 0)
    {
        speakWriteLine("100% of flights were processed automatically (no previously assigned flights).");
    }
    else
    {
        speakWriteLine($"Percentage of flights processed automatically over those already assigned: {(double)(initialFlightCount - unassignedFlights.Count) / (FlightDict.Count - initialFlightCount):P2}");
    }

    if (Terminal5.BoardingGates.Count - initialGateCount == 0)
    {
        speakWriteLine("100% of gates were processed automatically (no previously assigned gates).");
    }
    else
    {
        speakWriteLine($"Percentage of gates processed automatically over those already assigned: {(double)(initialGateCount - unassignedGates.Count) / (Terminal5.BoardingGates.Count - initialGateCount):P2}");
    }




}

bool IsGateCompatible(BoardingGate gate, Flight flight) =>
    (flight is CFFTFlight && gate.SupportsCFFT) ||
    (flight is DDJBFlight && gate.SupportsDDJB) ||
    (flight is LWTTFlight && gate.SupportsLWTT) || (flight is NORMFlight && !gate.SupportsDDJB && !gate.SupportsCFFT && !gate.SupportsLWTT);

// Advanced Feature (B): Ranen Sim
void DisplayTotalFees(Terminal terminal, Dictionary<string, Flight> FlightDict)
{
    // Check if all flights have boarding gates assigned
    foreach (KeyValuePair<string, Airline> airlineEntry in terminal.Airlines)
    {
        foreach (KeyValuePair<string, Flight> flightEntry in airlineEntry.Value.Flights)
        {
            if (FindBoardingGateByFlightNumber(FlightDict, terminal, flightEntry.Value) == null)
            {
                speakWriteLine("Ensure all flights have boarding gates assigned first before displaying total fees");
                return;
            }
        }
    }
    double subtotal = 0;
    double discount = 0;
    double total = 0;
    speakWriteLine("\n=== Airline Fee Breakdown ===");
    foreach (KeyValuePair<string, Airline> kvp in terminal.Airlines)
    {
        Airline airline = kvp.Value;
        double airlineSubtotal = 0;
        foreach (KeyValuePair<string, Flight> akvp in airline.Flights)
        {
            airlineSubtotal += akvp.Value.CalculateFees();
        }
        double airlineTotal = airline.CalculateFees();
        double airlineDiscount = airlineSubtotal - airlineTotal;
        speakWriteLine($"\nAirline: {airline.Name}");
        speakWriteLine($"Subtotal Fees: ${airlineSubtotal:0.00}");
        speakWriteLine($"Total Discounts: ${airlineDiscount:0.00}");
        speakWriteLine($"Final Total: ${airlineTotal:0.00}");
        subtotal += airlineSubtotal;
        discount += airlineDiscount;
        total += airlineTotal;
    }
    speakWriteLine($"\n=== {terminal.TerminalName} Total ===");
    speakWriteLine($"Total Subtotal: ${subtotal:0.00}");
    speakWriteLine($"Total Discounts: ${discount:0.00}");
    speakWriteLine($"Final Collection: ${total:0.00}");
    double discountPercent = (discount / total) * 100;
    speakWriteLine($"Discounts Percentage: {discountPercent:0.00}%");
    speakWriteLine();
}

// Additional Feature (C): Ooi Jing Shun (Weather of Destination upon Arrival)
async Task<string> getWeatherFromCoordinates(double latitude, double longitude, DateTime dt)
{
    List<string> weather = new List<string>();
    using HttpClient client = new();
    string forecast = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,cloudcover,precipitation_probability,weathercode";

    try
    {
        var result = await ProcessDataAsync<Rootobject>(client, forecast);
        var hourly = result.hourly;
        var weatherCodes = new Dictionary<int, string>
        {
            { 0, "Clear sky" },
            { 1, "Partly cloudy" },
            { 2, "Partly cloudy" },
            { 3, "Partly cloudy" },
            { 45, "Fog" },
            { 48, "Fog" },
            { 51, "Drizzle" },
            { 53, "Drizzle" },
            { 55, "Drizzle" },
            { 56, "Freezing drizzle" },
            { 57, "Freezing drizzle" },
            { 61, "Rain" },
            { 63, "Rain" },
            { 65, "Rain" },
            { 66, "Freezing rain" },
            { 67, "Freezing rain" },
            { 71, "Snowfall" },
            { 73, "Snowfall" },
            { 75, "Snowfall" },
            { 77, "Snow grains" },
            { 80, "Rain showers" },
            { 81, "Rain showers" },
            { 82, "Rain showers" },
            { 85, "Snow showers" },
            { 86, "Snow showers" },
            { 95, "Thunderstorm" },
            { 96, "Thunderstorm with hail" },
            { 99, "Thunderstorm with hail" }
        };

        string[] time = hourly.time;
        float[] temperature = hourly.temperature_2m;
        int[] cloudCover = hourly.cloudcover;
        int[] precipitation = hourly.precipitation_probability;
        int[] weatherCode = hourly.weathercode;
        DateTime dateFromString;
        for (int i = 0; i < time.Length; i++)
        {
            if (DateTime.TryParse(time[i], out dateFromString))
            {
                if ((Math.Abs((dateFromString - dt).TotalMinutes) <= 30))
                {
                    string weatherDescription;
                    if (weatherCodes.TryGetValue(weatherCode[i], out weatherDescription))
                    {
                        return $"{temperature[i]}°C, {weatherDescription}, {cloudCover[i]}% Cloud Cover, {precipitation[i]}% Chance of Precipitation";
                    }
                    else
                    {
                        return $"{temperature[i]}°C, Unknown Weather Code, {cloudCover[i]}% Cloud Cover, {precipitation[i]}% Chance of Precipitation";
                    }
                }
            }
        }
        return "NIL";
    }
    catch (Exception e)
    {
        speakWriteLine($"Error: {e.Message}");
        return "NIL";
    }
}
void getWeatherGivenFlight(Dictionary<string, Flight> FlightDict)
{
    speakWriteLine("Enter flight number to get weather forecast: ");
    string flightNumber = Console.ReadLine();
    if (FlightDict.ContainsKey(flightNumber))
    {
        try
        {
            Flight flight = FlightDict[flightNumber];
            string destination = FlightDict[flightNumber].Destination;
            speakWriteLine($"Getting weather forecast for {destination}...");
            List<string> coordinates = getCoordinates(destination).Result;
            if (coordinates[0] != "NIL")
            {
                double latitude = Convert.ToDouble(coordinates[0]);
                double longitude = Convert.ToDouble(coordinates[1]);

                // Convert from Singapore Time (SST) to GMT (UTC)
                TimeZoneInfo singaporeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                DateTime gmtTime = TimeZoneInfo.ConvertTimeToUtc(flight.ExpectedTime, singaporeTimeZone);
                speakWriteLine($"SGT Time: {flight.ExpectedTime}");
                speakWriteLine($"GMT Time: {gmtTime}");

                string weather = getWeatherFromCoordinates(latitude, longitude, gmtTime).Result;
                speakWriteLine($"Weather Conditions for {destination} Upon arrival time {flight.ExpectedTime}:\n{weather}");
            }

            else
            {
                speakWriteLine("Weather not found");
            }
        }
        catch (Exception e)
        {
            speakWriteLine($"Error: {e.Message}");
            speakWriteLine("Weather not found");
        }

    }
    else
    {
        speakWriteLine("Flight not found");
    }
}

async Task<List<string>> getCoordinates(string destination)
{
    using HttpClient client = new();
    client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0 (contact@yourdomain.com)");
    string url = $"https://nominatim.openstreetmap.org/search?q={destination}&format=json";

    try
    {

        var result = await ProcessDataAsync<List<Location>>(client, url);
        var property = result;

        if (property.Count == 0)
        {
            return new List<string> { "NIL", "NIL" };
        }
        else
        {
            string lat = result[0].lat;
            string lon = result[0].lon;
            return new List<string> { lat, lon };
        }
    }
    catch (Exception e)
    {
        speakWriteLine($"Error: {e.Message}");
        return new List<string> { "NIL", "NIL" };
    }
}
static async Task<T> ProcessDataAsync<T>(HttpClient client, string url)
{
    try
    {
        // Get the JSON response as a string
        using HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode(); // Ensure the request was successful

        string jsonResponse = await response.Content.ReadAsStringAsync();

        // Print the JSON to verify its content
        //speakWriteLine("JSON Response:");
        //speakWriteLine(jsonResponse);

        // Deserialize the JSON into the specified type
        T obj = JsonSerializer.Deserialize<T>(jsonResponse);
        return obj;
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"HTTP Request Error: {e.Message}");
        throw;
    }
    catch (JsonException e)
    {
        Console.WriteLine($"JSON Deserialization Error: {e.Message}");
        throw;
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected Error: {e.Message}");
        throw;
    }
}



// Additional feature (C): Ranen Sim (Text to speech for the visually impaired)


void speakWriteLine(string message = "")
{

    if (!SpeechToggle)
    {
        Console.WriteLine(message);
        return;
    }

    Console.WriteLine(message);

    message = message.Replace("=", "");
    message = message.Replace("*", "");
    message = message.Replace("_", "");
    message = message.Replace("-", "");
    message = message.Replace("  ", " ");
    message = message.Trim();
    s.Speak(message);
}
void speakWrite(string message = "")
{
    if (!SpeechToggle)
    {
        Console.Write(message);
        return;
    }
    Console.Write(message);

    message = message.Replace("=", "");
    message = message.Replace("*", "");
    message = message.Replace("_", "");
    message = message.Replace("-", "");
    message = message.Replace("  ", " ");
    message = message.Trim();
    s.Speak(message);
}

bool ToggleSpeak(bool onOrOff)
{
    if (!onOrOff)
    {
        onOrOff = true;
    }
    else
    {
        onOrOff = false;
    }
    return onOrOff;
}

SpeechSynthesizer ModifySpeech(SpeechSynthesizer s)
{
    while (true)
    {
        speakWriteLine("1. Change volume");
        speakWriteLine("2. Change speech rate");
        speakWrite("Enter your choice: ");

        string? input = Console.ReadLine();

        if (input == "1")
        {
            speakWriteLine("Set volume (0 - 100): ");
            string? volume = Console.ReadLine();

            try
            {
                s.Volume = Convert.ToInt32(volume);
            }
            catch
            {
                speakWriteLine("Invalid input");
                continue;
            }
            speakWriteLine("Speech volume updated successfully");
            return s;
        }
        else if (input == "2")
        {
            speakWriteLine("Set speech rate (-10 - 10): ");
            string? rate = Console.ReadLine();
            try
            {
                s.Rate = Convert.ToInt32(rate);
            }
            catch
            {
                speakWriteLine("Invalid input");
                continue;
            }
            speakWriteLine("Speech rate updated successfully");
            return s;
        }
        else
        {
            speakWriteLine("Invalid input");
            continue;
        }
    }
}