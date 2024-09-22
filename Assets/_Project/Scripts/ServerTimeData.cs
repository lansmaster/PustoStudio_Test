
[System.Serializable]
public class ServerTimeData
{
    public string utc_offset;
    public string timezone;
    public int day_of_week;
    public int day_of_year;
    public string datetime;
    public string utc_datetime;
    public long unixtime;
    public long raw_offset;
    public int week_number;
    public bool dst;
    public string abbreviation;
    public int dst_offset;
    public int dst_from;
    public int dst_until;
    public string client_ip;
}