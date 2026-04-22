using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using System;

public class Achievement 
{
    [PrimaryKey, AutoIncrement]
    public int AchievementID { get; set; }

    public int UserID { get; set; }
    public int ModuleID { get; set; }

    // 🔥 store as string para safe sa SQLite
    public string dateEarned { get; set; }
}
