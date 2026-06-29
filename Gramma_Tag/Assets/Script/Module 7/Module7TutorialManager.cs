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
    public float flipSpeed = 0.15f;
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

    IEnumerator FlipCard(Button selectedButton, Image frontImage, Image backImage, TMP_Text revealText, bool isCorrect, string explanation)
    {
        Vector3 originalScale = selectedButton.transform.localScale;

        yield return FlipToThin(selectedButton, originalScale);

        if (frontImage != null)
            frontImage.gameObject.SetActive(false);

        if (backImage != null)
            backImage.gameObject.SetActive(true);

        if (revealText != null)
        {
            revealText.gameObject.SetActive(true);
            revealText.text = (isCorrect ? "CORRECT!\n\n" : "WRONG!\n\n") + explanation;
        }

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

        yield return FlipToFull(selectedButton, originalScale);

        yield return new WaitForSeconds(nextDelay);

        if (correctStreak >= 3)
        {
            StartCoroutine(ReadyThenGoToGame());
            yield break;
        }

        yield return FlipToThin(selectedButton, originalScale);

        if (backImage != null)
            backImage.gameObject.SetActive(false);

        if (revealText != null)
            revealText.gameObject.SetActive(false);

        if (frontImage != null)
            frontImage.gameObject.SetActive(true);

        yield return FlipToFull(selectedButton, originalScale);

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

        if (tutorialGameplayGroup != null)
            tutorialGameplayGroup.SetActive(false);

        if (readyText != null)
        {
            readyText.gameObject.SetActive(true);
            readyText.text = "Get Ready...";
        }

        yield return new WaitForSeconds(readyDelay);

        if (readyText != null)
            readyText.text = "Starting Game!";

        yield return new WaitForSeconds(0.8f);

        SceneManager.LoadScene(mainGameSceneName);
    }

    IEnumerator FlipToThin(Button selectedButton, Vector3 originalScale)
    {
        float timer = 0f;

        while (timer < flipSpeed)
        {
            timer += Time.deltaTime;
            float xScale = Mathf.Lerp(originalScale.x, 0.02f, timer / flipSpeed);
            selectedButton.transform.localScale = new Vector3(xScale, originalScale.y, originalScale.z);
            yield return null;
        }

        selectedButton.transform.localScale = new Vector3(0.02f, originalScale.y, originalScale.z);
    }

    IEnumerator FlipToFull(Button selectedButton, Vector3 originalScale)
    {
        float timer = 0f;

        while (timer < flipSpeed)
        {
            timer += Time.deltaTime;
            float xScale = Mathf.Lerp(0.02f, originalScale.x, timer / flipSpeed);
            selectedButton.transform.localScale = new Vector3(xScale, originalScale.y, originalScale.z);
            yield return null;
        }

        selectedButton.transform.localScale = originalScale;
    }

    void ResetCards()
    {
        factButton.transform.localScale = normalFactScale;
        opinionButton.transform.localScale = normalOpinionScale;

        if (factFrontImage != null)
            factFrontImage.gameObject.SetActive(true);

        if (opinionFrontImage != null)
            opinionFrontImage.gameObject.SetActive(true);

        if (factBackImage != null)
            factBackImage.gameObject.SetActive(false);

        if (opinionBackImage != null)
            opinionBackImage.gameObject.SetActive(false);

        if (factRevealText != null)
            factRevealText.gameObject.SetActive(false);

        if (opinionRevealText != null)
            opinionRevealText.gameObject.SetActive(false);
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