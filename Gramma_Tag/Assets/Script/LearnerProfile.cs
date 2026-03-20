using SQLite;

public class LearnerProfile
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public string firstName { get; set; }
    public string lastName { get; set; }
    public int age { get; set; }
    public string gender { get; set; }
    public int coins { get; set; }
    public int hearts { get; set; }
    public string lastHeartTime { get; set; }
}