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

    private int totalQuestions = 10;

    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;

    public BlendDropZone choiceAZone;
    public BlendDropZone choiceBZone;

    // ✅ CHANGED: Slider instead of Image
    public Slider progressBar;

    public Image feedbackImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    public GameObject feedbackOverlay;

    [Header("FEEDBACK TEXT")]
    public TextMeshProUGUI feedbackText; // 🔥 drag mo yung "Feedback Text"
    [TextArea] public string correctMessage;
    [TextArea] public string wrongMessage;

    public TextMeshProUGUI questionText;

    [Header("TIMER")]
    public TextMeshProUGUI timerText;
    public float gameDuration = 300f;

    private float timer;
    private bool isTimerRunning = false;

    [Header("COUNTDOWN")]
    public TextMeshProUGUI countdownText;
    public GameObject gameplayUI;


    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    IEnumerator Start()
    {
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        DatabaseManager.Instance.DeductHeart();

        wordOriginalPos = draggableWord.GetComponent<RectTransform>().anchoredPosition;

        LoadQuestions();

        // ✅ setup progress bar
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        yield return StartCoroutine(StartCountdown());

        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        ShowQuestion();

        timer = gameDuration;
        isTimerRunning = true;
        UpdateTimerUI();
    }

    void LoadQuestions()
    {
        questions.Clear();

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);
        Debug.Log("🎯 GAME RECEIVED MODULE ID: " + moduleID);

        var dbQuestions = DatabaseManager.Instance.GetQuestionsByModule(moduleID);

        Debug.Log("📦 QUESTIONS COUNT: " + dbQuestions.Count);

        dbQuestions = dbQuestions
            .OrderBy(x => Random.value)
            .Take(totalQuestions)
            .ToList();

        foreach (var q in dbQuestions)
        {
            questions.Add(new Question(
                q.QuestionText,
                q.ChoiceA,
                q.ChoiceB,
                q.CorrectAnswer
            ));
        }
    }

    void ShowQuestion()
    {
        // ✅ kapag tapos na lahat
        if (currentIndex >= questions.Count)
        {
            // 🔥 force full progress
            progressBar.value = questions.Count;
            progressText.text = $"Question {questions.Count} / {questions.Count}";

            EndGame();
            return;
        }

        Question q = questions[currentIndex];

        if (questionText != null)
            questionText.text = q.word;

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

        // ✅ PROGRESS (starts at 0)
        progressBar.value = currentIndex;
        progressText.text = $"Question {currentIndex} / {questions.Count}";
    }

    void NextQuestion()
    {
        feedbackOverlay.SetActive(false);

        currentIndex++;

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

        StartCoroutine(SaveAndExit(moduleID, total, stars, passed));

        isTimerRunning = false;
    }

    IEnumerator SaveAndExit(int moduleID, int total, int stars, int passed)
    {
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        DatabaseManager.Instance.SaveProgressBetter(1, moduleID, score, passed, stars);

        int coinsEarned = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

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

        if (feedbackText != null)
            feedbackText.text = correctMessage;

        // 🔊 PLAY CORRECT SOUND
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(correctSFX);

        feedbackOverlay.SetActive(true);

        Invoke("NextQuestion", 1.5f);
    }

    public void WrongAnswer()
    {
        feedbackImage.sprite = wrongSprite;

        if (feedbackText != null)
            feedbackText.text = wrongMessage;

        // 🔊 PLAY WRONG SOUND
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(wrongSFX);

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
            return "";

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

            EndGame();
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
    public string word;
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