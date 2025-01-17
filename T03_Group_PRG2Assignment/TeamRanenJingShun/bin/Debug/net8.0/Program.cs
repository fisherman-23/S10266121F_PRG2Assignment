// See https://aka.ms/new-console-template for more information
using TeamRanenJingShun;

Console.WriteLine("Hello, World!");

Flight fl = new Flight("SQ123", "DXB", "SIN", new DateTime(2025,1,1), "On Time");
Console.WriteLine(fl.CalculateFees());
Airline al = new Airline("SQ","123");
al.AddFlight(fl);
Console.WriteLine(al.CalculateFees());