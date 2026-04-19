using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq; // 🔥 IMPORTANT for random
using UnityEngine.UI;

public class BlendGameManager : MonoBehaviour
{
   
    public TextMeshProUGUI progressText;
    public GameObject feedbackArea;
  
    public BlendDraggable draggableWord;
    public Transform wordOriginalParent;

    private Vector2 wordOriginalPos;
    private List<Question> questions = new List<Question>();
    private int currentIndex = 0;
    private int score = 0;

    private int totalQuestions = 10; // 🔥 LIMIT

    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;

    public BlendDropZone choiceAZone;
    public BlendDropZone choiceBZone;

  

    public Image progressBarFill;
    public Image feedbackImage;

    public Sprite correctSprite;
    public Sprite wrongSprite;

    public GameObject feedbackOverlay;
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

        int moduleID = 1;

        var dbQuestions = DatabaseManager.Instance.GetQuestionsByModule(moduleID);

        Debug.Log("DB Count: " + dbQuestions.Count);

        dbQuestions = dbQuestions.OrderBy(x => Random.value).Take(totalQuestions).ToList();

        foreach (var q in dbQuestions)
        {
            string word = q.QuestionText.ToLower();

            // GET BLEND
            string blend = "";
            if (q.CorrectAnswer == "Initial Blend")
                blend = word.Substring(0, 2);
            else if (q.CorrectAnswer == "Final Blend")
                blend = word.Substring(word.Length - 2);

            // 🔥 IMPORTANT: get WRONG from opposite category
            var wrongPool = dbQuestions
      .Where(x => x.CorrectAnswer != q.CorrectAnswer)
      .ToList();

            var other = wrongPool
                .OrderBy(x => Random.value)
                .FirstOrDefault();

            string wrongWord = other != null ? other.QuestionText : "cat";

            questions.Add(new Question(blend, word, wrongWord));
        }

        Debug.Log("Final Questions: " + questions.Count);

        Debug.Log("SelectedModuleID: " + moduleID);
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

        if (Random.value > 0.5f)
        {
            choiceAText.text = q.correctWord;
            choiceBText.text = q.wrongWord;

            choiceAZone.isCorrect = true;
            choiceBZone.isCorrect = false;
        }
        else
        {
            choiceAText.text = q.wrongWord;
            choiceBText.text = q.correctWord;

            choiceAZone.isCorrect = false;
            choiceBZone.isCorrect = true;
        }

        progressText.text = $"Question {currentIndex + 1} / {questions.Count}";
        progressBarFill.fillAmount = (float)currentIndex / questions.Count;
    }


    void NextQuestion()
    {
        feedbackOverlay.SetActive(false);

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

    public void CorrectAnswer()
    {
        score++;

        feedbackImage.sprite = correctSprite;

        feedbackOverlay.SetActive(true);

        Invoke("NextQuestion", 1.5f);
    }

    public void WrongAnswer()
    {
        feedbackImage.sprite = wrongSprite;

        feedbackOverlay.SetActive(true);

        Invoke("NextQuestion", 1.5f);
    }

    public Vector2 GetOriginalPos()
    {
        return wordOriginalPos;
    }

    public string GetCurrentCorrectWord()
    {
        if (currentIndex >= questions.Count)
            return ""; // or null

        return questions[currentIndex].correctWord;
    }
}

[System.Serializable]
public class Question
{
    public string blend;
    public string correctWord;
    public string wrongWord;

    public Question(string b, string correct, string wrong)
    {
        blend = b;
        correctWord = correct;
        wrongWord = wrong;
    }


}