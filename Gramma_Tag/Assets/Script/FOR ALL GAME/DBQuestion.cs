using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

public class DBQuestion 
{
    [PrimaryKey, AutoIncrement]
    public int QuizID { get; set; }

    public int ModuleID { get; set; }
    public string QuestionText { get; set; }
    public string ChoiceA { get; set; }
    public string ChoiceB { get; set; }
    public string ChoiceC { get; set; }

    public string ChoiceD { get; set; }
    public string CorrectAnswer { get; set; }
}
