// QuestionManager.cs — attach to the GameManager object
using UnityEngine;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    private List<QuestionData> all = new List<QuestionData>();
    private List<QuestionData> shuffled;
    private int index = 0;

    void Awake() { Build(); Shuffle(); }

    void Build()
    {
        all.Add(new QuestionData
        {
            questionType = QuestionType.DragDrop,
            sentenceText = "Yesterday, Ana ___ to school.",
            wordChoices = new string[] { "walk", "walked", "walking" },
            correctWord = "walked",
            explanation = "walked = past tense!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.DragDrop,
            sentenceText = "Right now, she ___ her homework.",
            wordChoices = new string[] { "does", "did", "will do" },
            correctWord = "does",
            explanation = "does = present tense!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.DragDrop,
            sentenceText = "Tomorrow, they ___ to the park.",
            wordChoices = new string[] { "go", "went", "will go" },
            correctWord = "will go",
            explanation = "will go = future tense!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.DragDrop,
            sentenceText = "They ___ playing outside.",
            wordChoices = new string[] { "am", "is", "are" },
            correctWord = "are",
            explanation = "are = plural subject!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.DragDrop,
            sentenceText = "She ___ a good student.",
            wordChoices = new string[] { "am", "is", "are" },
            correctWord = "is",
            explanation = "is = singular she!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.DragDrop,
            sentenceText = "I ___ happy today.",
            wordChoices = new string[] { "am", "is", "are" },
            correctWord = "am",
            explanation = "am = I always!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.Matching,
            matchInstruction = "Match each word to its tense!",
            matchPairs = new List<MatchPair> {
                new MatchPair{word="yesterday", correctLabel="past"},
                new MatchPair{word="tomorrow",  correctLabel="future"},
                new MatchPair{word="right now", correctLabel="present"}
            },
            explanation = "Yesterday=past, Right now=present, Tomorrow=future!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.Matching,
            matchInstruction = "Match each word to its tense!",
            matchPairs = new List<MatchPair> {
                new MatchPair{word="last week", correctLabel="past"},
                new MatchPair{word="next time", correctLabel="future"},
                new MatchPair{word="today",     correctLabel="present"}
            },
            explanation = "Last week=past, Today=present, Next time=future!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.Matching,
            matchInstruction = "Match each word to its tense!",
            matchPairs = new List<MatchPair> {
                new MatchPair{word="ago",   correctLabel="past"},
                new MatchPair{word="later", correctLabel="future"},
                new MatchPair{word="now",   correctLabel="present"}
            },
            explanation = "Ago=past, Now=present, Later=future!"
        });
        all.Add(new QuestionData
        {
            questionType = QuestionType.Matching,
            matchInstruction = "Match each word to its tense!",
            matchPairs = new List<MatchPair> {
                new MatchPair{word="long ago",   correctLabel="past"},
                new MatchPair{word="soon",       correctLabel="future"},
                new MatchPair{word="at present", correctLabel="present"}
            },
            explanation = "Long ago=past, At present=present, Soon=future!"
        });
    }

    void Shuffle()
    {
        shuffled = new List<QuestionData>(all);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var t = shuffled[i]; shuffled[i] = shuffled[j]; shuffled[j] = t;
        }
        index = 0;
    }

    public QuestionData GetCurrent() =>
        index < shuffled.Count ? shuffled[index] : null;
    public void Advance() => index++;
    public bool IsFinished() => index >= shuffled.Count;
    public int CurrentNumber() => index + 1;
    public int Total() => shuffled.Count;
}