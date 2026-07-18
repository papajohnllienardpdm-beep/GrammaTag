using UnityEngine;

[System.Serializable]
public class Module1ImageData
{
    public int quizID;

    [Header("Scenario")]
    public Sprite questionImage;

    [Header("Choice Images")]
    public Sprite choiceAImage;
    public Sprite choiceBImage;

    [Header("Result Images")]
    public Sprite resultImageA;
    public Sprite resultImageB;
}