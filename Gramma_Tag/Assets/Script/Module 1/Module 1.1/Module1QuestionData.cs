using UnityEngine;

[System.Serializable]
public class Module1QuestionData
{
    public int quizID;
    public int moduleID;

    [TextArea]
    public string question;

    public string choiceA;
    public string choiceB;

    // EXACTLY SAME AS DATABASE
    public string correctAnswer;
}