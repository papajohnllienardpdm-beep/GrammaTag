using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Module4GameManager : MonoBehaviour
{
    [Header("UI - Timer")]
    public TMP_Text timerText;

    [Header("Game Timer Settings")]
    public float gameDuration = 300f;

    private float timer;
    private bool isTimerRunning = false;

    [Header("UI - Question")]
    public TMP_Text sentenceText;

    [Header("UI - Answers")]
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    [Header("UI - Progress")]
    public TMP_Text progressText;
    public Slider progressBar;

    private List<Module4Question> questions = new List<Module4Question>();
    private int totalQuestions = 10;

    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool answered = false;

    private List<string> currentShuffledChoices;

    public float nextDelay = 1.5f;

    Color normalColor = Color.white;
    Color dimColor = new Color(0.7f, 0.7f, 0.7f);

    [Header("Start Countdown")]
    public GameObject getReadyText;
    public GameObject gameUI;

    void Start()
    {
        StartCoroutine(WaitForDB()); // ✅ FIXED
    }

    IEnumerator WaitForDB()
    {
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        DatabaseManager.Instance.DeductHeart();

        LoadQuestionsFromDB();

        timer = gameDuration;

        if (getReadyText != null)
        {
            gameUI.SetActive(false);
            StartCoroutine(StartGameWithDelay());
        }
        else
        {
            gameUI.SetActive(true);
            StartTimer();
            LoadQuestion();
        }
    }

    void LoadQuestionsFromDB()
    {
        questions.Clear();

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        var dbQuestions = DatabaseManager.Instance
            .GetQuestionsByModule(moduleID)
            .OrderBy(x => Random.value)
            .Take(totalQuestions)
            .ToList();

        foreach (var q in dbQuestions)
        {
            questions.Add(new Module4Question(
                q.QuestionText,
                q.ChoiceA,
                q.ChoiceB,
                q.ChoiceC,
                q.ChoiceD,
                q.CorrectAnswer
            ));
        }
    }

    void LoadQuestion()
    {
        answered = false;

        Module4Question q = questions[currentQuestionIndex];

        sentenceText.text = q.questionText;

        currentShuffledChoices = q.choices.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerTexts[i].text = currentShuffledChoices[i];

            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(currentShuffledChoices[index]));

            answerButtons[i].interactable = true;
            answerButtons[i].GetComponent<Image>().color = normalColor;
        }

        UpdateProgress();
    }

    void SelectAnswer(string selectedAnswer)
    {
        if (answered) return;

        answered = true;

        Module4Question q = questions[currentQuestionIndex];

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button btn = answerButtons[i];
            TMP_Text txt = answerTexts[i];

            btn.interactable = false;

            string choice = currentShuffledChoices[i];

            btn.GetComponent<Image>().color = dimColor;
            txt.color = Color.gray;

            if (choice == q.correctAnswer)
            {
                btn.GetComponent<Image>().color = Color.green;
                txt.color = Color.white;
            }

            if (choice == selectedAnswer && choice != q.correctAnswer)
            {
                btn.GetComponent<Image>().color = Color.red;
                txt.color = Color.white;
            }
        }

        if (selectedAnswer == q.correctAnswer)
            score++;

        Invoke(nameof(NextQuestion), nextDelay);
    }

    public void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Count)
        {
            FinishGame();
            return;
        }

        LoadQuestion();
    }

    void UpdateProgress()
    {
        progressText.text = (currentQuestionIndex + 1) + "/" + questions.Count;
        progressBar.value = (float)currentQuestionIndex / questions.Count;
    }

    void FinishGame()
    {
        int total = questions.Count;

        int stars = 0;
        int passed = 0;

        if (score >= 9) { stars = 3; passed = 1; }
        else if (score >= 7) { stars = 2; passed = 1; }
        else if (score >= 6) { stars = 1; passed = 1; }

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        StartCoroutine(SaveAndExit(moduleID, total, stars, passed));
    }

    IEnumerator SaveAndExit(int moduleID, int total, int stars, int passed)
    {
        DatabaseManager.Instance.SaveProgressBetter(1, moduleID, score, passed, stars);

        int coinsEarned = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("Passed", passed);
        PlayerPrefs.SetInt("CoinsEarned", coinsEarned);

        SceneManager.LoadScene("ResultScene");
        yield return null;
    }

    public void StartTimer()
    {
        timer = gameDuration;
        isTimerRunning = true;
    }

    void Update()
    {
        if (!isTimerRunning) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            TimeUp();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
        timerText.color = timer <= 5 ? Color.red : Color.black;
    }

    void TimeUp()
    {
        isTimerRunning = false;
        Invoke(nameof(NextQuestion), nextDelay);
    }

    IEnumerator StartGameWithDelay()
    {
        getReadyText.SetActive(true);
        TMP_Text txt = getReadyText.GetComponent<TMP_Text>();

        txt.text = "3"; yield return new WaitForSeconds(1);
        txt.text = "2"; yield return new WaitForSeconds(1);
        txt.text = "1"; yield return new WaitForSeconds(1);
        txt.text = "GO!"; yield return new WaitForSeconds(0.8f);

        getReadyText.SetActive(false);

        gameUI.SetActive(true);

        StartTimer();
        LoadQuestion();
    }
}

[System.Serializable]
public class Module4Question
{
    public string questionText;
    public string[] choices;
    public string correctAnswer;

    public Module4Question(string q, string a, string b, string c, string d, string correct)
    {
        questionText = q;
        choices = new string[] { a, b, c, d };
        correctAnswer = correct;
    }
}