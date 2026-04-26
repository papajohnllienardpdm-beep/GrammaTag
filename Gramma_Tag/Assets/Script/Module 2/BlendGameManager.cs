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

    public TextMeshProUGUI questionText; // 🔥 NEW

    [Header("TIMER")]
    public TextMeshProUGUI timerText;
    public float gameDuration = 300f; // 5 minutes

    private float timer;
    private bool isTimerRunning = false;
    [Header("COUNTDOWN")]
    public TextMeshProUGUI countdownText;
    public GameObject gameplayUI; // optional (para itago muna UI)

    IEnumerator Start()
    {
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        DatabaseManager.Instance.DeductHeart();

        wordOriginalPos = draggableWord.GetComponent<RectTransform>().anchoredPosition;

        LoadQuestions();

        // ❗ HIDE GAME UI muna
        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        // ❗ START COUNTDOWN
        yield return StartCoroutine(StartCountdown());

        // ❗ SHOW GAME
        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        ShowQuestion();

        // ✅ START TIMER AFTER COUNTDOWN
        timer = gameDuration;
        isTimerRunning = true;
        UpdateTimerUI();
    }

    void LoadQuestions()
    {
        questions.Clear();

        int moduleID = 1;
        Debug.Log("🎯 GAME RECEIVED MODULE ID: " + moduleID);
        var dbQuestions = DatabaseManager.Instance.GetQuestionsByModule(moduleID);

        dbQuestions = dbQuestions.OrderBy(x => Random.value).Take(totalQuestions).ToList();

        foreach (var q in dbQuestions)
        {
            questions.Add(new Question(
                q.QuestionText,   // 🔥 WORD
                q.ChoiceA,
                q.ChoiceB,
                q.CorrectAnswer
            ));
        }
    }

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        Question q = questions[currentIndex];

        // 🔥 SHOW QUESTION TEXT (ITO ANG FIX)
        if (questionText != null)
            questionText.text = q.word;
        else
            Debug.LogError("❌ questionText not assigned!");

        // random swap
        if (Random.value > 0.5f)
        {
            choiceAText.text = q.choiceA;
            choiceBText.text = q.choiceB;

            choiceAZone.answerText = q.choiceA;
            choiceBZone.answerText = q.choiceB;
        }
        else
        {
            choiceAText.text = q.choiceB;
            choiceBText.text = q.choiceA;

            choiceAZone.answerText = q.choiceB;
            choiceBZone.answerText = q.choiceA;
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

        int moduleID = 1;

        // 🔥 IMPORTANT: gamitin coroutine
        StartCoroutine(SaveAndExit(moduleID, total, stars, passed));

        isTimerRunning = false;
    }

    IEnumerator SaveAndExit(int moduleID, int total, int stars, int passed)
    {
        // 🔥 WAIT hanggang ready DB
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        // ✅ SAVE
        DatabaseManager.Instance.SaveProgressBetter(1, moduleID, score, passed, stars);

        // ✅ COINS
        int coinsEarned = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

        // ✅ SAVE RESULT DATA
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("Passed", passed);
        PlayerPrefs.SetInt("CoinsEarned", coinsEarned);

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        // 👉 LOAD RESULT
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

        return questions[currentIndex].correctAnswer;
    }

    public string GetCurrentCorrectAnswer()
    {
        if (currentIndex >= questions.Count)
            return "";

        return questions[currentIndex].correctAnswer;
    }

    void Update()
    {
        if (!isTimerRunning) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            isTimerRunning = false;

            EndGame(); // ⏰ AUTO END PAG NAUBOS ORAS
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

}

[System.Serializable]
public class Question
{
    public string word; // 🔥 ito yung QuestionText
    public string choiceA;
    public string choiceB;
    public string correctAnswer;

    public Question(string w, string a, string b, string correct)
    {
        word = w;
        choiceA = a;
        choiceB = b;
        correctAnswer = correct;
    }


}