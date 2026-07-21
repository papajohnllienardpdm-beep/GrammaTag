using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Module7TutorialManager : MonoBehaviour
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
    public TMP_Text tutorialText;
    public TMP_Text readyText;
    public GameObject tutorialGameplayGroup;

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

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    [Header("Settings")]
    public float nextDelay = 3f;
    public float flipDuration = 0.6f;
    public float liftHeight = 40f;
    public float readyDelay = 1.2f;
    public string mainGameSceneName = "Module7_GameScene";

    private List<Question> questions = new List<Question>();
    private int currentIndex = 0;
    private int correctStreak = 0;
    private bool answered = false;
    private bool tutorialFinished = false;

    private Vector3 normalFactScale;
    private Vector3 normalOpinionScale;

    

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(ApplyPortrait());

        Time.timeScale = 1f;

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

        if (readyText != null)
            readyText.gameObject.SetActive(false);

        CreateQuestions();
        ShuffleQuestions();

        progressBar.maxValue = 3;
        progressBar.value = 0;

        LoadQuestion();
        UpdateProgress();
    }

    IEnumerator ApplyPortrait()
    {
        yield return null;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void CreateQuestions()
    {
        questions.Clear();

        questions.Add(new Question(
            "The sun rises in the east.",
            true,
            "Fact: It can be proven true."
        ));

        questions.Add(new Question(
            "I think apples are the best fruit.",
            false,
            "Opinion: It uses 'I think'."
        ));

        questions.Add(new Question(
            "Fish live in water.",
            true,
            "Fact: It can be checked and proven."
        ));

        questions.Add(new Question(
            "Cats are the cutest animals.",
            false,
            "Opinion: Not everyone may agree."
        ));

        questions.Add(new Question(
            "Plants need sunlight to grow.",
            true,
            "Fact: Plants need sunlight for growth."
        ));
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

        if (currentIndex >= questions.Count)
        {
            currentIndex = 0;
            ShuffleQuestions();
        }

        Question currentQuestion = questions[currentIndex];
        sentenceText.text = currentQuestion.sentence;

        UpdateProgress();
    }

    void Answer(bool playerAnswerIsFact)
    {
        if (answered || tutorialFinished) return;

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

        // Save original rotation ng reveal text
        Quaternion revealTextOriginalRotation = Quaternion.identity;

        if (revealText != null)
            revealTextOriginalRotation = revealText.rectTransform.localRotation;

        float timer = 0f;
        bool changedFace = false;

        // =====================================================
        // FIRST FLIP: FRONT -> BACK
        // =====================================================

        while (timer < flipDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / flipDuration);

            // Aangat muna sa gitna ng animation,
            // tapos bababa ulit
            float height = Mathf.Sin(t * Mathf.PI) * liftHeight;

            rt.localPosition = startPos + Vector3.up * height;

            // Slight zoom habang umiikot
            rt.localScale = Vector3.Lerp(
                startScale,
                startScale * 1.08f,
                Mathf.Sin(t * Mathf.PI)
            );

            // Rotate from 0 to 180 degrees
            float angle = Mathf.Lerp(0f, 180f, t);

            rt.localRotation = Quaternion.Euler(
                0f,
                angle,
                0f
            );

            // Paglagpas ng 90 degrees,
            // palitan ang front papuntang back
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

                    // IMPORTANT:
                    // Dahil ang parent card ay matatapos sa Y = 180,
                    // i-counter rotate natin ang child text ng 180
                    // para hindi ito maging mirrored.
                    revealText.rectTransform.localRotation =
                        Quaternion.Euler(0f, 180f, 0f);
                }
            }

            yield return null;
        }

        // Siguraduhin exact ang final transform
        rt.localRotation = Quaternion.Euler(0f, 180f, 0f);
        rt.localScale = startScale;
        rt.localPosition = startPos;

        // Siguraduhin na readable ang text
        if (revealText != null)
        {
            revealText.rectTransform.localRotation =
                Quaternion.Euler(0f, 180f, 0f);
        }

        // =====================================================
        // EXISTING GAME LOGIC
        // HINDI BINAGO
        // =====================================================

        if (isCorrect)
        {
            correctStreak++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);
        }
        else
        {
            correctStreak = 0;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);
        }

        UpdateProgress();

        yield return new WaitForSeconds(nextDelay);

        if (correctStreak >= 3)
        {
            StartCoroutine(ReadyThenGoToGame());
            yield break;
        }

        // =====================================================
        // SECOND FLIP: BACK -> FRONT
        // =====================================================

        timer = 0f;
        changedFace = false;

        while (timer < flipDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / flipDuration);

            // Aangat ulit habang bumabalik
            float height = Mathf.Sin(t * Mathf.PI) * liftHeight;

            rt.localPosition = startPos + Vector3.up * height;

            // Slight zoom
            rt.localScale = Vector3.Lerp(
                startScale,
                startScale * 1.08f,
                Mathf.Sin(t * Mathf.PI)
            );

            // Continue rotation from 180 to 360
            float angle = Mathf.Lerp(180f, 360f, t);

            rt.localRotation = Quaternion.Euler(
                0f,
                angle,
                0f
            );

            // Kapag nasa 270 degrees na,
            // ibalik ang front
            if (!changedFace && angle >= 270f)
            {
                changedFace = true;

                if (backImage != null)
                    backImage.gameObject.SetActive(false);

                if (revealText != null)
                {
                    revealText.gameObject.SetActive(false);

                    // Ibalik original rotation ng text
                    revealText.rectTransform.localRotation =
                        revealTextOriginalRotation;
                }

                if (frontImage != null)
                    frontImage.gameObject.SetActive(true);
            }

            yield return null;
        }

        // =====================================================
        // RESET TRANSFORM
        // =====================================================

        rt.localRotation = startRot;
        rt.localScale = startScale;
        rt.localPosition = startPos;

        if (revealText != null)
        {
            revealText.rectTransform.localRotation =
                revealTextOriginalRotation;
        }

        // =====================================================
        // NEXT QUESTION
        // EXISTING LOGIC
        // =====================================================

        currentIndex++;

        LoadQuestion();
    }

    IEnumerator ReadyThenGoToGame()
    {
        tutorialFinished = true;

        factButton.interactable = false;
        opinionButton.interactable = false;

        progressText.text = "Tutorial 3 / 3";
        progressBar.value = 3;
        tutorialText.text = "Excellent! You are now ready to play.";

        yield return new WaitForSeconds(0.8f);

        SceneManager.LoadScene(mainGameSceneName);
    }




    void ResetCards()
    {
        factButton.transform.localScale = normalFactScale;
        opinionButton.transform.localScale = normalOpinionScale;

        // Reset card rotations
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

            // Reset text rotation
            factRevealText.rectTransform.localRotation =
                Quaternion.identity;
        }

        if (opinionRevealText != null)
        {
            opinionRevealText.gameObject.SetActive(false);

            // Reset text rotation
            opinionRevealText.rectTransform.localRotation =
                Quaternion.identity;
        }
    }

    void UpdateProgress()
    {
        progressText.text = "Tutorial " + correctStreak + " / 3";
        progressBar.value = correctStreak;

        if (correctStreak == 0)
            tutorialText.text = "Read the sentence. Choose Fact or Opinion.";
        else if (correctStreak == 1)
            tutorialText.text = "Good! Try another one.";
        else if (correctStreak == 2)
            tutorialText.text = "Last one! Get this right.";
        else if (correctStreak >= 3)
            tutorialText.text = "Great job! Starting the game...";
    }

    public void OpenSettings()
    {
        if (settingsController != null)
            settingsController.OpenSettings();
    }
}