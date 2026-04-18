using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
public class Modules 
{
    [PrimaryKey]
    public int ModuleID { get; set; }
    public string ModuleName { get; set; }
    public string Difficulty { get; set; }

    public string Overview { get; set; }
}
