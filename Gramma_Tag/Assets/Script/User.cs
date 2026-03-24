using SQLite;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int UserID { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public int Hearts { get; set; }
    public int Coins { get; set; }
    public string lastHeartTime { get; set; }
}