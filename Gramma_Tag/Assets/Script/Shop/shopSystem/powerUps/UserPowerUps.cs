using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;


public class UserPowerUps : MonoBehaviour
{
    [PrimaryKey, AutoIncrement]
    public int UserPowerUpID { get; set; }

    public int UserID { get; set; }
    public string PowerUpType { get; set; }
    public int isActive { get; set; }
}
