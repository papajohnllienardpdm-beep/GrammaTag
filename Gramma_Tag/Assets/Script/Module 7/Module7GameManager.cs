using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Module7GameManager : MonoBehaviour
{


    [Header("Start Countdown")]
    public GameObject getReadyText;
    public GameObject gameUI;

    [Header("UI")]
    public TMP_Text sentenceText;
    public TMP_Text progressText;
    public Slider progressBar;

    public Button factButton;
    public Button opinionButton;

    [Header("Settings Button")]
    public Button settingsButton;
    public SettingsPanelController settingsController;

    [Header("Card Images")]
    public Image factFrontImage;
    public Image factBackImage;
    public Image opinionFrontImage;
    public Image opinionBackImage;

    [Header("Reveal Text")]
    public TMP_Text factRevealText;
    public TMP_Text opinionRevealText;

    [Header("Timer")]
    public TMP_Text timerText;
    public float totalTime = 300f;

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    [Header("Game Settings")]
    public float nextDelay = 3f;
    public float flipDuration = 0.6f;
    public float liftHeight = 40f;

    private List<Module7Question> questions = new List<Module7Question>();

    private int totalQuestions = 10;
    private int currentIndex = 0;
    private int score = 0;
    private bool answered = false;
    private bool isGameOver = false;

    private float remainingTime;
    private bool isTimerRunning = false;

    private Vector3 normalFactScale;
    private Vector3 normalOpinionScale;



    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(ApplyPortrait());
        Time.timeScale = 1f;

        remainingTime = totalTime;

        normalFactScale = factButton.transform.localScale;
        normalOpinionScale = opinionButton.transform.localScale;

        factButton.onClick.RemoveAllListeners();
        opinionButton.onClick.RemoveAllListeners();

        factButton.onClick.AddListener(() => Answer(true));
        opinionButton.onClick.AddListener(() => Answer(false));

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OpenSettings);
        }

        StartCoroutine(WaitForDB());
    }

    IEnumerator ApplyPortrait()
    {
        yield return null;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    IEnumerator WaitForDB()
    {
        while (DatabaseManager.Instance == null ||
               !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        while (HeartSystem.Instance == null)
            yield return null;

        PlayerPrefs.SetInt("HEART_USED_THIS_SESSION", 0);
        PlayerPrefs.Save();

        HeartSystem.Instance.ResetSession();

        LoadQuestionsFromDB();

        progressBar.maxValue = totalQuestions;
        progressBar.value = 0;

        remainingTime = totalTime;

        if (getReadyText != null)
        {
            gameUI.SetActive(false);
            StartCoroutine(StartGameWithDelay());
        }
        else
        {
            gameUI.SetActive(true);
            LoadQuestion();
        }
    }

    IEnumerator StartGameWithDelay()
    {
        getReadyText.SetActive(true);

        TMP_Text txt =
            getReadyText.GetComponent<TMP_Text>();

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

        // 🔥 START TIMER AFTER GO!
        StartTimer();

        LoadQuestion();
    }

    void LoadQuestionsFromDB()
    {
        questions.Clear();

        int moduleID =
            PlayerPrefs.GetInt("SelectedModuleID", 7);

        var dbQuestions =
            DatabaseManager.Instance
            .GetQuestionsByModule(moduleID)
            .OrderBy(x => Random.value)
            .Take(totalQuestions)
            .ToList();

        foreach (var q in dbQuestions)
        {
            questions.Add(
                new Module7Question(
                    q.QuestionText,
                    q.CorrectAnswer,
                    q.Feedback
                ));
        }
    }

    public void StartTimer()
    {
        remainingTime = totalTime;
        isTimerRunning = true;
    }


    void Update()
    {
        if (!isTimerRunning || isGameOver)
            return;

        UpdateTimer();
    }





    void LoadQuestion()
    {
        answered = false;

        ResetCards();

        factButton.interactable = true;
        opinionButton.interactable = true;

        Module7Question currentQuestion = questions[currentIndex];
        sentenceText.text = currentQuestion.sentence;

        if (progressText != null)
            progressText.text = "Progress " + currentIndex + "/" + questions.Count;

        if (progressBar != null)
            progressBar.value = currentIndex;
    }

    void Answer(bool playerAnswerIsFact)
    {
        if (answered || isGameOver) return;

        answered = true;

        factButton.interactable = false;
        opinionButton.interactable = false;

        Module7Question currentQuestion = questions[currentIndex];
        bool correctIsFact = currentQuestion.correctAnswer.ToLower() == "fact";

        bool isCorrect = playerAnswerIsFact == correctIsFact;

        Button selectedButton = playerAnswerIsFact ? factButton : opinionButton;
        Image selectedFront = playerAnswerIsFact ? factFrontImage : opinionFrontImage;
        Image selectedBack = playerAnswerIsFact ? factBackImage : opinionBackImage;
        TMP_Text selectedRevealText = playerAnswerIsFact ? factRevealText : opinionRevealText;

        StartCoroutine(FlipCard(
            selectedButton,
            selectedFront,
            selectedBack,
            selectedRevealText,
            isCorrect,
            currentQuestion.feedback
        ));
    }

    IEnumerator FlipCard(
    Button selectedButton,
    Image frontImage,
    Image backImage,
    TMP_Text revealText,
    bool isCorrect,
    string feedback)
    {
        RectTransform rt = selectedButton.GetComponent<RectTransform>();

        Vector3 startPos = rt.localPosition;
        Vector3 startScale = rt.localScale;
        Quaternion startRot = rt.localRotation;

        Quaternion revealTextOriginalRotation = Quaternion.identity;

        if (revealText != null)
            revealTextOriginalRotation = revealText.rectTransform.localRotation;

        float timer = 0f;
        bool changedFace = false;

        while (timer < flipDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / flipDuration);

            float height = Mathf.Sin(t * Mathf.PI) * liftHeight;

            rt.localPosition = startPos + Vector3.up * height;

            rt.localScale = Vector3.Lerp(
                startScale,
                startScale * 1.08f,
                Mathf.Sin(t * Mathf.PI));

            float angle = Mathf.Lerp(0f, 180f, t);

            rt.localRotation = Quaternion.Euler(
                0f,
                angle,
                0f);

            if (!changedFace && angle >= 90f)
            {
                changedFace = true;

                if (frontImage != null)
                    frontImage.gameObject.SetActive(false);

                if (backImage != null)
                    backImage.gameObject.SetActive(true);

                if (revealText != null)
                {
                    revealText.gameObject.SetActive(true);

                    revealText.text =
                        (isCorrect ? "CORRECT!\n\n" : "NICE TRY!\n\n")
                        + feedback;

                    revealText.rectTransform.localRotation =
                        Quaternion.Euler(0f, 180f, 0f);
                }
            }

            yield return null;
        }

        rt.localRotation = Quaternion.Euler(0f, 180f, 0f);
        rt.localScale = startScale;
        rt.localPosition = startPos;

        if (revealText != null)
        {
            revealText.rectTransform.localRotation =
                Quaternion.Euler(0f, 180f, 0f);
        }

        // ======================================
        // GAME LOGIC (same as original)
        // ======================================

        if (isCorrect)
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

        yield return new WaitForSeconds(nextDelay);

        timer = 0f;
        changedFace = false;

        while (timer < flipDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / flipDuration);

            float height = Mathf.Sin(t * Mathf.PI) * liftHeight;

            rt.localPosition = startPos + Vector3.up * height;

            rt.localScale = Vector3.Lerp(
                startScale,
                startScale * 1.08f,
                Mathf.Sin(t * Mathf.PI));

            float angle = Mathf.Lerp(180f, 360f, t);

            rt.localRotation = Quaternion.Euler(
                0f,
                angle,
                0f);

            if (!changedFace && angle >= 270f)
            {
                changedFace = true;

                if (backImage != null)
                    backImage.gameObject.SetActive(false);

                if (revealText != null)
                {
                    revealText.gameObject.SetActive(false);

                    revealText.rectTransform.localRotation =
                        revealTextOriginalRotation;
                }

                if (frontImage != null)
                    frontImage.gameObject.SetActive(true);
            }

            yield return null;
        }

        rt.localRotation = startRot;
        rt.localScale = startScale;
        rt.localPosition = startPos;

        if (revealText != null)
            revealText.rectTransform.localRotation =
                revealTextOriginalRotation;

        currentIndex++;

        if (progressText != null)
            progressText.text = "Progress " + currentIndex + "/" + questions.Count;

        if (progressBar != null)
            progressBar.value = currentIndex;

        if (currentIndex >= questions.Count)
        {
            GoToResultScene();
        }
        else
        {
            LoadQuestion();
        }
    }



    void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GoToResultScene();
        }

        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        if (remainingTime <= 10)
            timerText.color = Color.red;
    }

    void ResetCards()
    {
        factButton.transform.localScale = normalFactScale;
        opinionButton.transform.localScale = normalOpinionScale;

        factButton.transform.localRotation = Quaternion.identity;
        opinionButton.transform.localRotation = Quaternion.identity;

        if (factFrontImage != null)
            factFrontImage.gameObject.SetActive(true);

        if (opinionFrontImage != null)
            opinionFrontImage.gameObject.SetActive(true);

        if (factBackImage != null)
            factBackImage.gameObject.SetActive(false);

        if (opinionBackImage != null)
            opinionBackImage.gameObject.SetActive(false);

        if (factRevealText != null)
        {
            factRevealText.gameObject.SetActive(false);
            factRevealText.rectTransform.localRotation =
                Quaternion.identity;
        }

        if (opinionRevealText != null)
        {
            opinionRevealText.gameObject.SetActive(false);
            opinionRevealText.rectTransform.localRotation =
                Quaternion.identity;
        }
    }

    public void OpenSettings()
    {
        if (settingsController != null)
            settingsController.OpenSettings();
    }

    void GoToResultScene()
    {
        if (isGameOver) return;

        isGameOver = true;
        isTimerRunning = false;

        if (HeartSystem.Instance != null)
        {
            HeartSystem.Instance.UseHeartSafe(1);
        }

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

        int moduleID =
            PlayerPrefs.GetInt("SelectedModuleID", 7);

        StartCoroutine(
            SaveAndExit(
                moduleID,
                total,
                stars,
                passed
            ));
    }

    IEnumerator SaveAndExit(
    int moduleID,
    int total,
    int stars,
    int passed)
    {
        int coinsEarned =
            DatabaseManager.Instance.GiveCoins(
                moduleID,
                score,
                passed);

        DatabaseManager.Instance.SaveProgressBetter(
            1,
            moduleID,
            score,
            passed,
            stars);

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("Passed", passed);
        PlayerPrefs.SetInt("CoinsEarned", coinsEarned);

        SceneManager.LoadScene("ResultScene");

        yield return null;
    }
}

[System.Serializable]
public class Module7Question
{
    public string sentence;
    public string correctAnswer;
    public string feedback;

    public Module7Question(
        string sentence,
        string correctAnswer,
        string feedback)
    {
        this.sentence = sentence;
        this.correctAnswer = correctAnswer;
        this.feedback = feedback;
    }
}