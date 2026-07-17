using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Module7GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string sentence;
        public bool isFact;
        public string explanation;

        public Question(string sentence, bool isFact, string explanation)
        {
            this.sentence = sentence;
            this.isFact = isFact;
            this.explanation = explanation;
        }
    }

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

    [Header("SFX Optional")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;
    public AudioSource audioSource;

    [Header("Game Settings")]
    public float nextDelay = 3f;
    public float flipDuration = 0.6f;
    public float liftHeight = 40f;

    private List<Question> questions = new List<Question>();
    private int currentIndex = 0;
    private int score = 0;
    private bool answered = false;
    private bool isGameOver = false;

    private float remainingTime;

    private Vector3 normalFactScale;
    private Vector3 normalOpinionScale;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
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

        LoadStaticQuestions();
        ShuffleQuestions();

        if (progressBar != null)
            progressBar.maxValue = questions.Count;

        LoadQuestion();
    }

    void Update()
    {
        if (!isGameOver)
            UpdateTimer();
    }

    void LoadStaticQuestions()
    {
        questions = new List<Question>()
        {
            new Question("The sun rises in the east.", true,
                "This is a fact because it can be proven true."),

            new Question("Blue is my favorite color.", false,
                "This is an opinion because it tells a personal feeling or preference."),

            new Question("A dog is an animal.", true,
                "This is a fact because a dog can be proven to be an animal."),

            new Question("I think apples are the best fruit.", false,
                "This is an opinion because it uses the signal words 'I think'."),

            new Question("Fish live in water.", true,
                "This is a fact because it can be checked and proven."),

            new Question("Basketball is more fun than volleyball.", false,
                "This is an opinion because people may have different choices."),

            new Question("Plants need sunlight to grow.", true,
                "This is a fact because plants need sunlight for growth."),

            new Question("In my opinion, reading books is boring.", false,
                "This is an opinion because it uses the signal words 'in my opinion'."),

            new Question("Water freezes when it is very cold.", true,
                "This is a fact because water can freeze when the temperature is low enough."),

            new Question("Cats are the cutest animals.", false,
                "This is an opinion because people may not all agree with it.")
        };
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            Question temp = questions[i];
            int randomIndex = Random.Range(i, questions.Count);
            questions[i] = questions[randomIndex];
            questions[randomIndex] = temp;
        }
    }

    void LoadQuestion()
    {
        answered = false;

        ResetCards();

        factButton.interactable = true;
        opinionButton.interactable = true;

        Question currentQuestion = questions[currentIndex];
        sentenceText.text = currentQuestion.sentence;

        if (progressText != null)
            progressText.text = "Progress " + (currentIndex + 1) + "/" + questions.Count;

        if (progressBar != null)
            progressBar.value = currentIndex + 1;
    }

    void Answer(bool playerAnswerIsFact)
    {
        if (answered || isGameOver) return;

        answered = true;

        factButton.interactable = false;
        opinionButton.interactable = false;

        Question currentQuestion = questions[currentIndex];
        bool isCorrect = playerAnswerIsFact == currentQuestion.isFact;

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
            currentQuestion.explanation
        ));
    }

    IEnumerator FlipCard(
    Button selectedButton,
    Image frontImage,
    Image backImage,
    TMP_Text revealText,
    bool isCorrect,
    string explanation)
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
                        (isCorrect ? "CORRECT!\n\n" : "WRONG!\n\n")
                        + explanation;

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

            if (audioSource != null && correctSFX != null)
                audioSource.PlayOneShot(correctSFX);
        }
        else
        {
            if (audioSource != null && wrongSFX != null)
                audioSource.PlayOneShot(wrongSFX);
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

        if (currentIndex >= questions.Count)
            GoToResultScene();
        else
            LoadQuestion();
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

        int total = questions.Count;
        int passingScore = 8;
        int passed = score >= passingScore ? 1 : 0;
        int coinsEarned = passed == 1 ? 200 : 50;

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("CoinsEarned", coinsEarned);
        PlayerPrefs.SetInt("Passed", passed);

        PlayerPrefs.SetInt("SelectedModuleID", 7);
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        PlayerPrefs.Save();

        SceneManager.LoadScene("ResultScene");
    }
}