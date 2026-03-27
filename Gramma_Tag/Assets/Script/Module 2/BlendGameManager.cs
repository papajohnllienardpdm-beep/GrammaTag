using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BlendGameManager : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI progressText;
    public GameObject feedbackArea;
    public TextMeshProUGUI feedbackText;
    public BlendDraggable draggableWord;
    public Transform wordOriginalParent;

    private Vector2 wordOriginalPos;
    private List<Question> questions = new List<Question>();
    private int currentIndex = 0;
    private int score = 0;

    void Start()
    {
        wordOriginalPos = draggableWord.GetComponent<RectTransform>().anchoredPosition;

        LoadQuestions();
        ShowQuestion();
    }

    void LoadQuestions()
    {
        // TEMP DATA (pwede mo palitan ng SQLite later)
        questions.Add(new Question("crab", "Initial"));
        questions.Add(new Question("ring", "Final"));
        questions.Add(new Question("plant", "Initial"));
        questions.Add(new Question("sand", "Final"));
        questions.Add(new Question("flag", "Initial"));
        questions.Add(new Question("bench", "Final"));
        questions.Add(new Question("smile", "Initial"));
        questions.Add(new Question("pink", "Final"));
        questions.Add(new Question("clock", "Initial"));
        questions.Add(new Question("tent", "Final"));
    }

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        wordText.text = questions[currentIndex].word;
        progressText.text = $"Question {currentIndex + 1} of 10";
    }

    public void SubmitAnswer(string category)
    {
        if (currentIndex >= questions.Count) return;

        if (category == questions[currentIndex].category)
        {
            score++;
            feedbackText.text = "Correct!";
            feedbackText.color = Color.green;
        }
        else
        {
            feedbackText.text = "Wrong!";
            feedbackText.color = Color.red;
        }

        feedbackArea.SetActive(true);

        // 🔥 LAST QUESTION CHECK
        if (currentIndex == questions.Count - 1)
        {
            Invoke("NextQuestion", 0.3f); // mabilis
        }
        else
        {
            Invoke("NextQuestion", 0.8f); // normal
        }
    }

    void NextQuestion()
    {
        feedbackArea.SetActive(false);

        currentIndex++;

        // 🔥 CHECK KUNG TAPOS NA
        if (currentIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        // reset position
        draggableWord.ResetPosition(wordOriginalParent, wordOriginalPos);

        ShowQuestion();
    }

    void EndGame()
    {
        int coins = (score >= 6) ? 100 : 50;

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("CoinsEarned", coins);
        PlayerPrefs.SetInt("Passed", score >= 6 ? 1 : 0);

        SceneManager.LoadScene("ResultScene");

        PlayerPrefs.SetString("LastScene", "Module2_GameScene");

        PlayerPrefs.SetInt("TotalQ", 10);
    }
}

[System.Serializable]
public class Question
{
    public string word;
    public string category;

    public Question(string w, string c)
    {
        word = w;
        category = c;
    }
}