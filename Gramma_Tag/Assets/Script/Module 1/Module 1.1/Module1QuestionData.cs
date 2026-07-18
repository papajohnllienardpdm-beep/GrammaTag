using UnityEngine;

[System.Serializable]
public class Module1QuestionData
{
    [Header("Question")]

    [TextArea]
    public string question;

    public string choiceA;
    public string choiceB;

    [Tooltip("A or B")]
    public string correctAnswer;

    [Header("Images")]

    public Sprite questionImage;

    public Sprite choiceAImage;

    public Sprite choiceBImage;

    public Sprite resultImageA;

    public Sprite resultImageB;
}