using UnityEngine;

[System.Serializable]
public class ImageSet
{
    public Sprite imageA;
    public Sprite imageB;
    public Sprite imageC;
    public Sprite imageD;
}

[System.Serializable]
public class Module6QuestionData
{
    public string question;
    public string[] choices;
    public int correctIndex;

    public ImageSet images;
}