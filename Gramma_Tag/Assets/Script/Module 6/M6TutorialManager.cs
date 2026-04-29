using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Module6TutorialManager : MonoBehaviour
{

    public Image overlayA;
    public Image overlayB;
    public Image overlayC;
    public Image overlayD;

    public Slider progressBar;

    [Header("Buttons")]
    public Button buttonA, buttonB, buttonC, buttonD;

    [Header("Texts")]
    public TextMeshProUGUI textA, textB, textC, textD;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI progressText;

    public TextMeshProUGUI instructionText;

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private List<Module6QuestionData> questions = new List<Module6QuestionData>();

    private bool hasAnswered = false;

    int currentQuestionIndex = -1;
    int correctStreak = 0;

    [Header("Question Images")]
    public Image imageA;
    public Image imageB;
    public Image imageC;
    public Image imageD;

    [Header("Tutorial Sprites")]
    public ImageSet mariaSet;
    public ImageSet rainSet;
    public ImageSet sleepSet;

    void Start()
    {
        buttonA.onClick.AddListener(() => CheckAnswer(0));
        buttonB.onClick.AddListener(() => CheckAnswer(1));
        buttonC.onClick.AddListener(() => CheckAnswer(2));
        buttonD.onClick.AddListener(() => CheckAnswer(3));

        CreateTutorialQuestions();

        // ✅ SLIDER SETUP
        progressBar.maxValue = 3;
        progressBar.value = 0;

        LoadQuestion();
    }

    void CreateTutorialQuestions()
    {
        // ✅ Maria
        questions.Add(new Module6QuestionData
        {
            question = "Maria wore her uniform and packed her bag.",
            choices = new string[]
            {
            "She will sleep",
            "She will go to school",
            "She will eat",
            "She will watch TV"
            },
            correctIndex = 1,
            images = mariaSet
        });

        // ✅ Rain
        questions.Add(new Module6QuestionData
        {
            question = "Dark clouds filled the sky.",
            choices = new string[]
            {
            "It is sunny",
            "It will rain",
            "It is night",
            "It is windy"
            },
            correctIndex = 1,
            images = rainSet
        });

        // ✅ Sleep
        questions.Add(new Module6QuestionData
        {
            question = "Kids are lying in bed with eyes closed.",
            choices = new string[]
            {
            "They are eating",
            "They are playing",
            "They are sleeping",
            "They are running"
            },
            correctIndex = 2,
            images = sleepSet
        });
    }

    void LoadQuestion()
    {
        int newIndex;

        do
        {
            newIndex = Random.Range(0, questions.Count);
        }
        while (newIndex == currentQuestionIndex && questions.Count > 1);

        currentQuestionIndex = newIndex;

        var q = questions[currentQuestionIndex];

        imageA.sprite = q.images.imageA;
        imageB.sprite = q.images.imageB;
        imageC.sprite = q.images.imageC;
        imageD.sprite = q.images.imageD;

        hasAnswered = false;

        questionText.text = q.question;

        textA.text = q.choices[0];
        textB.text = q.choices[1];
        textC.text = q.choices[2];
        textD.text = q.choices[3];

        // ✅ FIXED PROGRESS
        progressText.text = "Tutorial " + correctStreak + " / 3";
        progressBar.value = correctStreak;

        EnableAllButtons();
        SetAllOverlayColor(new Color(0, 0, 0, 0));

        if (correctStreak == 0)
            instructionText.text = "Read the sentence and choose the correct answer.";
        else if (correctStreak == 1)
            instructionText.text = "Good! Try another one.";
        else if (correctStreak == 2)
            instructionText.text = "Last one! Get this right.";
    }

    void CheckAnswer(int index)
    {
        if (hasAnswered) return;
        hasAnswered = true;

        var q = questions[currentQuestionIndex];

        SetAllOverlayColor(new Color(0, 0, 0, 0.4f));

        if (index == q.correctIndex)
        {
            GetOverlay(index).color = new Color(0, 1, 0, 0.6f);

            instructionText.text = "Correct!";

            correctStreak++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);

            if (correctStreak >= 3)
            {
                progressBar.value = 3;
                progressText.text = "Tutorial 3 / 3";

                Invoke(nameof(GoToGame), 1f);
                return;
            }

            Invoke(nameof(LoadQuestion), 1f);
        }
        else
        {
            GetOverlay(index).color = new Color(1, 0, 0, 0.6f);
            GetOverlay(q.correctIndex).color = new Color(0, 1, 0, 0.6f);

            instructionText.text = "Oops! Try again from the start.";

            correctStreak = 0;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            progressBar.value = 0;
            progressText.text = "Tutorial 0 / 3";

            Invoke(nameof(LoadQuestion), 1.5f);
        }

        DisableAllButtons();
    }

    void GoToGame()
    {
        SceneManager.LoadScene("Module6Final");
    }

    Image GetOverlay(int index)
    {
        switch (index)
        {
            case 0: return overlayA;
            case 1: return overlayB;
            case 2: return overlayC;
            case 3: return overlayD;
        }
        return null;
    }

    void SetAllOverlayColor(Color color)
    {
        overlayA.color = color;
        overlayB.color = color;
        overlayC.color = color;
        overlayD.color = color;
    }

    void DisableAllButtons()
    {
        buttonA.interactable = false;
        buttonB.interactable = false;
        buttonC.interactable = false;
        buttonD.interactable = false;
    }

    void EnableAllButtons()
    {
        buttonA.interactable = true;
        buttonB.interactable = true;
        buttonC.interactable = true;
        buttonD.interactable = true;
    }

}