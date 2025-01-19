// See https://aka.ms/new-console-template for more information
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

//1 Load files(airlines and boarding gates)
string[] AirlinesCSVLines = File.ReadAllLines("assets/airlines.csv");
Dictionary<string, Airline> AirlineDict = new Dictionary<string, Airline>();
for (int i = 1; i < AirlinesCSVLines.Length; i++)
{
    string[] line = AirlinesCSVLines[i].Split(',');
    Airline airline = new Airline(line[0].Trim(), line[1].Trim());
    AirlineDict[line[1]] = airline;
}

string[] BoardingGatesCSVLines = File.ReadAllLines("assets/boardinggates.csv");
Dictionary<string, BoardingGate> BoardingGateDict = new Dictionary<string, BoardingGate>();
for (int i = 1; i < BoardingGatesCSVLines.Length; i++)
{
    string[] line = BoardingGatesCSVLines[i].Split(',');
    BoardingGate boardingGate = new BoardingGate(line[0].Trim(), Convert.ToBoolean(line[2].Trim()), Convert.ToBoolean(line[1].Trim()), Convert.ToBoolean(line[3].Trim()));
    BoardingGateDict[line[0]] = boardingGate;
}


//2 Load files (flights)
Dictionary<string, Flight> FlightDict = new Dictionary<string, Flight>();
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
    } else if (requestCode == "LWTT")
    {
        Flight flight = new LWTTFlight(value[0], value[1], value[2], DateTime.Parse(value[3]), "On Time");
        FlightDict.Add(value[0], flight);
    }

}
//Console.WriteLine("Flights loaded");
//foreach (KeyValuePair<string, Flight> kvp in FlightDict )
//{
//    Console.WriteLine(kvp.Value);
//}