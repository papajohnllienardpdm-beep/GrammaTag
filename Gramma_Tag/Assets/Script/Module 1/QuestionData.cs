// QuestionData.cs — NOT a MonoBehaviour. Just a data class.
using System.Collections.Generic;

public enum QuestionType { DragDrop, Matching }

[System.Serializable]
public class QuestionData
{
    public QuestionType questionType;
    public string sentenceText;
    public string[] wordChoices;
    public string correctWord;
    public string matchInstruction;
    public List<MatchPair> matchPairs;
    public string explanation;
}

[System.Serializable]
public class MatchPair
{
    public string word;
    public string correctLabel;
}