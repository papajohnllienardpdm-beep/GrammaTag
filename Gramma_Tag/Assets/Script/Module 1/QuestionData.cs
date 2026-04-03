using System.Collections.Generic;
using UnityEngine;

public enum QuestionType { DragDrop, Matching, MultipleChoice }

[System.Serializable]
public class QuestionData
{
    public QuestionType questionType;

    // COMMON
    public string sentenceText;
    public string explanation;

    // DRAG DROP
    public string[] wordChoices;
    public string correctWord;

    // MATCHING
    public string matchInstruction;
    public List<MatchPair> matchPairs;

    // ✅ MODULE 4 (MCQ)
    public string[] choices;
    public string correctAnswer;
    public Sprite image;
}

[System.Serializable]
public class MatchPair
{
    public string word;
    public string correctLabel;
}