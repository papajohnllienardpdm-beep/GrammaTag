using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;


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

    [Header("UI - Feedback")]
 
    public TMP_Text feedbackText;


    [Header("UI - Answers")]
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    [Header("UI - Progress")]
    public TMP_Text progressText;
    public Slider progressBar;

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    [Header("Feedback Colors")]
    public Color correctFeedbackColor = new Color(0f, 0.65f, 0f);
    public Color wrongFeedbackColor = new Color(0.85f, 0f, 0f);

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
        StartCoroutine(WaitForDB());
    }

    IEnumerator WaitForDB()
    {
        // 🔥 Force Portrait pagpasok ng Game
        Screen.orientation = ScreenOrientation.Portrait;
        yield return null;

        // wait DB
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        // wait HeartSystem
        while (HeartSystem.Instance == null)
            yield return null;

        // 🔥 NEW GAME SESSION - allow 1 heart deduction later
        PlayerPrefs.SetInt("HEART_USED_THIS_SESSION", 0);
        PlayerPrefs.Save();

        if (HeartSystem.Instance != null)
        {
            HeartSystem.Instance.ResetSession();
        }

        LoadQuestionsFromDB();

        // ✅ setup slider
        progressBar.maxValue = totalQuestions;
        progressBar.value = 0;

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

        Debug.Log("MODULE ID LOADED: " + moduleID);

        var dbQuestions = DatabaseManager.Instance
            .GetQuestionsByModule(moduleID)
            .OrderBy(x => Random.value)
            .Take(totalQuestions)
            .ToList();

        foreach (var q in dbQuestions)
        {
            Debug.Log("QUESTION: " + q.QuestionText);
            Debug.Log("CORRECT: " + q.CorrectAnswer);
            Debug.Log("FEEDBACK FROM DB: " + q.Feedback);

            questions.Add(new Module4Question(
                q.QuestionText,
                q.ChoiceA,
                q.ChoiceB,
                q.ChoiceC,
                q.ChoiceD,
                q.CorrectAnswer,
                q.Feedback
            ));
        }
    }

    void LoadQuestion()
    {
        answered = false;

        if (feedbackText != null)
            feedbackText.text = "";

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

            btn.interactable = false;

            string choice = currentShuffledChoices[i];

            btn.GetComponent<Image>().color = dimColor;

            if (choice == q.correctAnswer)
            {
                btn.GetComponent<Image>().color = Color.green;
            }

            if (choice == selectedAnswer && choice != q.correctAnswer)
            {
                btn.GetComponent<Image>().color = Color.red;
            }
        }

        if (selectedAnswer == q.correctAnswer)
        {
            score++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);
        }

        if (feedbackText != null)
        {
            bool isCorrect = selectedAnswer == q.correctAnswer;

            if (!string.IsNullOrEmpty(q.feedback))
            {
                feedbackText.text = GetColoredFeedback(
                    q.feedback,
                    q.correctAnswer,
                    isCorrect
                );
            }
            else
            {
                feedbackText.text = "No feedback found for this question.";
            }
        }

        Invoke(nameof(NextQuestion), nextDelay);
    }

    public void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Count)
        {
            // ✅ full progress
            progressBar.value = questions.Count;
            progressText.text = questions.Count + "/" + questions.Count;

            FinishGame();
            return;
        }

        LoadQuestion();
    }

    void UpdateProgress()
    {
        progressText.text = currentQuestionIndex + "/" + questions.Count;
        progressBar.value = currentQuestionIndex;
    }

    void FinishGame()
    {
        // 🔥 BAWAS 1 HEART ONLY WHEN GAME ENDS
        if (HeartSystem.Instance != null)
        {
            HeartSystem.Instance.UseHeartSafe(1);
        }

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
        

        int coinsEarned = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

        DatabaseManager.Instance.SaveProgressBetter(1, moduleID, score, passed, stars);

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

    CancelInvoke(); // optional safety

    FinishGame(); // 🔥 diretso result na
}

    IEnumerator StartGameWithDelay()
    {
        getReadyText.SetActive(true);

        TMP_Text txt =
            getReadyText.GetComponent<TMP_Text>();

        // 🔥 NEW
        txt.text = "Let's Begin!";
        yield return new WaitForSeconds(1);

        txt.text = "3";
        yield return new WaitForSeconds(1);

        txt.text = "2";
        yield return new WaitForSeconds(1);

        txt.text = "1";
        yield return new WaitForSeconds(1);

        txt.text = "GO!";
        yield return new WaitForSeconds(0.8f);

        getReadyText.SetActive(false);

        gameUI.SetActive(true);

        StartTimer();

        LoadQuestion();
    }

    string GetColoredFeedback(string feedback, string correctAnswer, bool isCorrect)
    {
        Color color = isCorrect ? correctFeedbackColor : wrongFeedbackColor;

        string hexColor = ColorUtility.ToHtmlStringRGB(color);

        string pattern = @"\b" + Regex.Escape(correctAnswer) + @"\b";

        return Regex.Replace(
            feedback,
            pattern,
            "<color=#" + hexColor + ">$0</color>",
            RegexOptions.IgnoreCase
        );
    }
}

[System.Serializable]
public class Module4Question
{
    public string questionText;
    public string[] choices;
    public string correctAnswer;
    public string feedback;

    public Module4Question(string q, string a, string b, string c, string d, string correct, string fb)
    {
        questionText = q;
        choices = new string[] { a, b, c, d };
        correctAnswer = correct;
        feedback = fb;
    }
}