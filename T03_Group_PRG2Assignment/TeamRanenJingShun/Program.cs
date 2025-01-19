// See https://aka.ms/new-console-template for more information
using TeamRanenJingShun;

// Temporary code to test the classes
//Flight fl = new NORMFlight("SQ123", "DXB", "SIN", new DateTime(2025,1,1), "On Time");
//Flight fl2 = new CFFTFlight("SQ122", "DXB", "SIN", new DateTime(2025, 1, 1), "On Time");

//Console.WriteLine(fl.CalculateFees());
//Console.WriteLine(fl2.CalculateFees());
//Airline al = new Airline("SQ","123");
//Airline al2 = new Airline("SQ", "122");

//al.AddFlight(fl);
//al2.AddFlight(fl2);
//Console.WriteLine(al.CalculateFees());
//Console.WriteLine(al2.CalculateFees());

//1 Load files (airlines and boarding gates)
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

//4 List all boarding gates
Terminal Terminal5 = new Terminal("Terminal5");
void AddDataToTerminal(Terminal Terminal5)
{
    foreach (KeyValuePair<string, Airline> kvp in AirlineDict)
    {
        Terminal5.AddAirline(kvp.Value);
    }
    foreach (KeyValuePair<string, BoardingGate> kvp in BoardingGateDict)
    {
        Terminal5.AddBoardingGate(kvp.Value);
    }
}
AddDataToTerminal(Terminal5);

void DisplayBoardingGates(Terminal Terminal5)
{
    foreach (KeyValuePair<string, BoardingGate> kvp in Terminal5.BoardingGates)
    {
        Console.WriteLine($"Gate Name: {kvp.Value.GateName}");

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


        if (!(CFFT || DDJB || LWTT))
        {
            Console.WriteLine("No special request codes supported.");
        }
        else
        {
            Console.Write("Special request code(s) supported: ");
            if (CFFT)
            {
                Console.Write("CFFT ");
            }
            if (DDJB)
            {
                Console.Write("DDJB ");
            }
            if (LWTT)
            {
                Console.WriteLine("LWTT ");
            }
            Console.WriteLine();
        }

        if (kvp.Value.Flight != null)
        {
            Console.WriteLine($"Assigned Flight Number: {kvp.Value.Flight.FlightNumber}");
        }
        else
        {
            Console.WriteLine("No flight assigned to this gate.");
        }

        Console.WriteLine();
    }

}
DisplayBoardingGates(Terminal5);
