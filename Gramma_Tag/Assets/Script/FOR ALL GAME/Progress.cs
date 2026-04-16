using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

[System.Serializable]
public class Progress 
{
    [PrimaryKey, AutoIncrement]
    public int ProgressID { get; set; }

    public int UserID { get; set; }
    public int ModuleID { get; set; }
    public int Score { get; set; }
    public int isPassed { get; set; }
    public int Stars { get; set; }
}
