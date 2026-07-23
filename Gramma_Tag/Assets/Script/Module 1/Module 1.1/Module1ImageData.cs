using UnityEngine;

[System.Serializable]
public class Module1ImageData
{
    public int moduleID;
    public int quizID;

    [Header("Scenario")]
    public Sprite questionImage;

    [Header("Choice")]
    public Sprite choiceAImage;
    public Sprite choiceBImage;

    [Header("Result")]
    public Sprite resultImageA;
    public Sprite resultImageB;
}