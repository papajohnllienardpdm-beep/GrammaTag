using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq; // 🔥 IMPORTANT for random

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

    private int totalQuestions = 10; // 🔥 LIMIT

    IEnumerator Start()
    {
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        // ❤️ BAWAS HEART
        DatabaseManager.Instance.DeductHeart();

        wordOriginalPos = draggableWord.GetComponent<RectTransform>().anchoredPosition;

        LoadQuestions();
        ShowQuestion();
    }

    void LoadQuestions()
    {
        questions.Clear();

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        Debug.Log("ModuleID: " + moduleID);

        var dbQuestions = DatabaseManager.Instance.GetQuestionsByModule(moduleID);

        Debug.Log("DB Count: " + dbQuestions.Count);

        // 🔥 RANDOMIZE
        dbQuestions = dbQuestions.OrderBy(x => Random.value).ToList();

        // 🔥 LIMIT TO 10
        dbQuestions = dbQuestions.Take(totalQuestions).ToList();

        foreach (var q in dbQuestions)
        {
            questions.Add(new Question(q.QuestionText, q.CorrectAnswer));
        }

        Debug.Log("Final Questions: " + questions.Count);
    }

    void ShowQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("NO QUESTIONS!");
            return;
        }

        if (currentIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        Question q = questions[currentIndex];

        if (wordText != null)
        {
            wordText.text = q.word;
        }
        else
        {
            Debug.LogError("WORDTEXT NOT ASSIGNED!");
        } // 🔥 IMPORTANT

        progressText.text = $"Question {currentIndex + 1} / {questions.Count}";
    }

    public void SubmitAnswer(string category)
    {
        if (currentIndex >= questions.Count) return;

        string correct = questions[currentIndex].category;

        Debug.Log("Chosen: " + category);
        Debug.Log("Correct: " + correct);

        if (category == correct)
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

        Invoke("NextQuestion", 0.8f);
    }

    void NextQuestion()
    {
        feedbackArea.SetActive(false);

        currentIndex++;

        if (currentIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        draggableWord.ResetPosition(wordOriginalParent, wordOriginalPos);

        ShowQuestion();
    }

    void EndGame()
    {
        int total = questions.Count;

        int stars = 0;
        int passed = 0;

        if (score >= 9)
        {
            stars = 3;
            passed = 1;
        }
        else if (score >= 7)
        {
            stars = 2;
            passed = 1;
        }
        else if (score >= 6)
        {
            stars = 1;
            passed = 1;
        }
        else
        {
            stars = 0;
            passed = 0;
        }

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID");

        // 🔥 SAVE PROGRESS
        DatabaseManager.Instance.SaveProgressBetter(1, moduleID, score, passed, stars);

        // 🪙 GIVE COINS
        int coinsEarned = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

        // 👉 RESULT SCENE
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("Passed", passed);
        PlayerPrefs.SetInt("CoinsEarned", coinsEarned);

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("ResultScene");
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