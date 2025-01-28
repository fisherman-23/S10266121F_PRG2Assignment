
public class Rootobject
{
    public float latitude { get; set; }
    public float longitude { get; set; }
    public float generationtime_ms { get; set; }
    public int utc_offset_seconds { get; set; }
    public string timezone { get; set; }
    public string timezone_abbreviation { get; set; }
    public float elevation { get; set; }
    public Hourly_Units hourly_units { get; set; }
    public Hourly hourly { get; set; }
}

public class Hourly_Units
{
    public string time { get; set; }
    public string temperature_2m { get; set; }
    public string cloudcover { get; set; }
    public string precipitation_probability { get; set; }
    public string weathercode { get; set; }
}

public class Hourly
{
    public string[] time { get; set; }
    public float[] temperature_2m { get; set; }
    public int[] cloudcover { get; set; }
    public int[] precipitation_probability { get; set; }
    public int[] weathercode { get; set; }
}
