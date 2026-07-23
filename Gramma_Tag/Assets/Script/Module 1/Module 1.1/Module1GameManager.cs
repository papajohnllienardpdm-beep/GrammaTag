using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Module1GameManager : MonoBehaviour
{
    #region Question Data

    private List<Module1QuestionData> questions =
        new List<Module1QuestionData>();

    [Header("Image Database")]
    public List<Module1ImageData> images =
        new List<Module1ImageData>();

    private int currentQuestionIndex = 0;
    private Module1QuestionData currentQuestion;

    #endregion

    //=====================================================

    #region UI

    [Header("Question UI")]

    public TMP_Text questionText;

    public Image scenarioImage;

    [Header("Choice A")]

    public Image choiceAImage;
    public TMP_Text choiceAText;

    [Header("Choice B")]

    public Image choiceBImage;
    public TMP_Text choiceBText;

    [Header("Progress")]

    public TMP_Text progressText;
    public Slider progressBar;

    [Header("Choice Cards")]

    public Module1DraggableChoice choiceCardA;
    public Module1DraggableChoice choiceCardB;

    #endregion

    //=====================================================

    #region Result

    private int score = 0;

    private bool isTransitioning = false;

    [Header("Delay")]

    public float nextQuestionDelay = 1f;

    [Header("Scene")]

    public string resultSceneName = "ResultScene";

    #endregion

    //=====================================================

    [Header("Countdown")]

    public GameObject gameUI;

    public GameObject countdownPanel;

    public TMP_Text countdownText;

    public float countdownSpeed = 1f;

    //-------------------------------------
    [Header("Orientation")]
    public OrientationManager orientationManager;

    [Header("Timer")]
    public TMP_Text timerText;

    public float gameDuration = 300f;

    private float timer;

    private bool isTimerRunning;

    private bool gameEnded;

    private Color defaultTimerColor;

    private int currentModuleID;

    public string gameSceneName = "Module1_GameScene";

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;


    private void Start()
    {
        StartCoroutine(WaitForSystemsThenStart());
    }

    IEnumerator WaitForSystemsThenStart()
    {
        if (orientationManager != null)
            orientationManager.SetLandscape();

        while (DatabaseManager.Instance == null ||
               !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        while (HeartSystem.Instance == null)
            yield return null;

        PlayerPrefs.SetInt("HEART_USED_THIS_SESSION", 0);
        PlayerPrefs.Save();

        HeartSystem.Instance.ResetSession();

        LoadQuestionsFromDB();

        if (questions.Count == 0)
        {
            Debug.LogError("No questions found.");

            yield break;
        }

        if (progressBar != null)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = questions.Count;
            progressBar.value = 0;
        }

        if (timerText != null)
            defaultTimerColor = timerText.color;

        currentQuestionIndex = 0;

        gameUI.SetActive(false);

        countdownPanel.SetActive(true);

        StartCoroutine(StartCountdown());
    }

    //=====================================================

    void LoadQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        currentQuestion = questions[currentQuestionIndex];

        questionText.text = currentQuestion.question;

        choiceAText.text = currentQuestion.choiceA;
        choiceBText.text = currentQuestion.choiceB;

        Module1ImageData imageData =
            GetImageData(
                currentQuestion.moduleID,
                currentQuestion.quizID);

        if (imageData != null)
        {
            scenarioImage.sprite =
                imageData.questionImage;

            choiceAImage.sprite =
                imageData.choiceAImage;

            choiceBImage.sprite =
                imageData.choiceBImage;
        }

        UpdateProgress();

        choiceCardA.ResetCard();
        choiceCardB.ResetCard();
    }

    //=====================================================

    void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text =
                "Progress " +
                currentQuestionIndex +
                "/" +
                questions.Count;
        }

        if (progressBar != null)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = questions.Count;
            progressBar.value = currentQuestionIndex;
        }
    }
    //=====================================================

    public void OnChoiceDropped(Module1DraggableChoice draggedChoice)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        choiceCardA.SetCanDrag(false);
        choiceCardB.SetCanDrag(false);

        string selectedAnswer =
    draggedChoice.choiceKey == "A"
    ? currentQuestion.choiceA
    : currentQuestion.choiceB;

        bool isCorrect =
            selectedAnswer.Trim().ToLower() ==
            currentQuestion.correctAnswer.Trim().ToLower();

        Module1ImageData imageData = GetImageData( currentQuestion.moduleID, currentQuestion.quizID);

        if (imageData != null)
        {
            if (draggedChoice.choiceKey == "A")
                scenarioImage.sprite =
                    imageData.resultImageA;
            else
                scenarioImage.sprite =
                    imageData.resultImageB;
        }

        if (isCorrect)
        {
            score++;

            Debug.Log("CORRECT");

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);
        }
        else
        {
            Debug.Log("WRONG");

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);
        }

        draggedChoice.ReturnToStart();

        StartCoroutine(NextQuestionRoutine());
    }

    IEnumerator NextQuestionRoutine()
    {
        yield return new WaitForSeconds(nextQuestionDelay);

        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Count)
        {
            EndGame();

            yield break;
        }

        LoadQuestion();

        isTransitioning = false;
    }

    void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        isTimerRunning = false;

        choiceCardA.SetCanDrag(false);
        choiceCardB.SetCanDrag(false);

        if (progressText != null)
        {
            progressText.text =
                "Progress " +
                questions.Count +
                "/" +
                questions.Count;
        }

        if (progressBar != null)
        {
            progressBar.value =
                questions.Count;
        }

        if (HeartSystem.Instance != null)
        {
            HeartSystem.Instance.UseHeartSafe(1);
        }

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

        int moduleID =
            PlayerPrefs.GetInt(
                "SelectedModuleID",
                1);

        StartCoroutine(
            SaveAndGoToResult(
                moduleID,
                questions.Count,
                stars,
                passed));
    }

    Module1ImageData GetImageData(int moduleID, int quizID)
    {
        foreach (var img in images)
        {
            if (img.moduleID == moduleID &&
               img.quizID == quizID)
            {
                return img;
            }
        }

        return null;
    }

    void LoadQuestionsFromDB()
    {
        questions.Clear();

        currentModuleID =
            PlayerPrefs.GetInt("SelectedModuleID", 1);

        var dbQuestions =
            DatabaseManager.Instance
            .GetQuestionsByModule(currentModuleID)
            .OrderBy(x => Random.value)
            .Take(10)
            .ToList();

        foreach (var dbQ in dbQuestions)
        {
            Module1QuestionData q =
                new Module1QuestionData();

            q.quizID = dbQ.QuizID;

            q.moduleID = dbQ.ModuleID;

            q.question = dbQ.QuestionText;

            q.choiceA = dbQ.ChoiceA;

            q.choiceB = dbQ.ChoiceB;

            q.correctAnswer =
                dbQ.CorrectAnswer;

            questions.Add(q);
        }

        if (questions.Count == 0)
        {
            Debug.LogError(
                "No AssessmentItems found for Module "
                + currentModuleID);
        }
        else
        {
            Debug.Log(
                "Questions Loaded : "
                + questions.Count);
        }
    }

    IEnumerator StartCountdown()
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(countdownSpeed);

        countdownText.text = "2";
        yield return new WaitForSeconds(countdownSpeed);

        countdownText.text = "1";
        yield return new WaitForSeconds(countdownSpeed);

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.6f);

        countdownPanel.SetActive(false);

        gameUI.SetActive(true);

        LoadQuestion();
        StartTimer();
    }

    public void StartTimer()
    {
        timer = gameDuration;
        isTimerRunning = true;
    }

    void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text =
            $"{minutes:00}:{seconds:00}";

        if (timer <= 5f)
            timerText.color = Color.red;
        else
            timerText.color = defaultTimerColor;
    }

    void TimeUp()
    {
        if (!isTimerRunning)
            return;

        isTimerRunning = false;

        EndGame();
    }

    void Update()
    {
        if (!isTimerRunning)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            TimeUp();
        }

        UpdateTimerUI();
    }

    IEnumerator SaveAndGoToResult(
    int moduleID,
    int total,
    int stars,
    int passed)
    {
        int userID =
            DatabaseManager.Instance.GetUserID();

        int coinsEarned =
            DatabaseManager.Instance.GiveCoins(
                moduleID,
                score,
                passed);

        DatabaseManager.Instance
            .SaveProgressBetter(
                userID,
                moduleID,
                score,
                passed,
                stars);

        PlayerPrefs.SetInt(
            "FinalScore",
            score);

        PlayerPrefs.SetInt(
            "TotalQ",
            total);

        PlayerPrefs.SetInt(
            "Stars",
            stars);

        PlayerPrefs.SetInt(
            "Passed",
            passed);

        PlayerPrefs.SetInt(
            "CoinsEarned",
            coinsEarned);

        PlayerPrefs.SetString(
            "LastScene",
            gameSceneName);

        PlayerPrefs.Save();

        if (orientationManager != null)
        {
            orientationManager.SetPortrait();
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(
            resultSceneName);
    }
}