using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Module1TutorialManager : MonoBehaviour
{
    [Header("Question UI")]
    public TMP_Text questionText;

    public Image scenarioImage;

    public Image choiceAImage;
    public Image choiceBImage;

    public TMP_Text choiceAText;
    public TMP_Text choiceBText;

    [Header("Tutorial UI")]
    public TMP_Text instructionText;

    public TMP_Text progressText;
    public Slider progressBar;

    [Header("Choice Cards")]
    public Module1DraggableChoice choiceCardA;
    public Module1DraggableChoice choiceCardB;

    [Header("Image Database")]
    public List<Module1ImageData> images =
        new List<Module1ImageData>();

    [Header("Scene")]
    public string gameSceneName = "Module1_GameScene";

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;


    private List<Module1QuestionData> questions =
    new List<Module1QuestionData>();

    private Module1QuestionData currentQuestion;

    private int currentQuestionIndex = 0;

    private int correctStreak = 0;

    private bool isTransitioning = false;

    public float nextQuestionDelay = 1f;

    void Start()
    {
        LoadTutorialQuestions();

        progressBar.maxValue = 3;
        progressBar.value = 0;

        instructionText.text =
            "Drag one of the picture cards.";

        LoadQuestion();
    }

    void LoadTutorialQuestions()
    {
        questions.Clear();

        questions.Add(new Module1QuestionData()
        {
            quizID = 1,
            moduleID = 1,

            question =
            "A house is on fire. What cause could make the house catch fire?",

            choiceA =
            "A child plays with matches.",

            choiceB =
            "A family watches television.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 2,
            moduleID = 2,

            question =
            "A room is dark. What cause could make the room dark?",

            choiceA =
            "Someone turns off the light.",

            choiceB =
            "Someone turns on the light.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 3,
            moduleID = 3,

            question =
            "The floor is wet. What cause could make the floor wet?",

            choiceA =
            "Someone spills water.",

            choiceB =
            "Someone sweeps the floor.",

            correctAnswer = "A"
        });
    }

    void LoadQuestion()
    {
        currentQuestion = questions[currentQuestionIndex];

        questionText.text =
            currentQuestion.question;

        choiceAText.text =
            currentQuestion.choiceA;

        choiceBText.text =
            currentQuestion.choiceB;

        Module1ImageData imageData =
            GetImageData(currentQuestion.quizID);

        if (imageData != null)
        {
            scenarioImage.sprite =
                imageData.questionImage;

            choiceAImage.sprite =
                imageData.choiceAImage;

            choiceBImage.sprite =
                imageData.choiceBImage;
        }

        progressText.text =
            "Tutorial " +
            correctStreak +
            " / 3";

        progressBar.value =
            correctStreak;

        choiceCardA.ResetCard();
        choiceCardB.ResetCard();

        instructionText.text =
            "Drag one of the picture cards.";
    }

    Module1ImageData GetImageData(int quizID)
    {
        foreach (Module1ImageData img in images)
        {
            if (img.quizID == quizID)
                return img;
        }

        return null;
    }

    public void OnChoiceDropped(Module1DraggableChoice draggedChoice)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        choiceCardA.SetCanDrag(false);
        choiceCardB.SetCanDrag(false);

        bool isCorrect =
            draggedChoice.choiceKey.Trim().ToUpper() ==
            currentQuestion.correctAnswer.Trim().ToUpper();

        Module1ImageData imageData =
            GetImageData(currentQuestion.quizID);

        if (imageData != null)
        {
            if (draggedChoice.choiceKey == "A")
                scenarioImage.sprite = imageData.resultImageA;
            else
                scenarioImage.sprite = imageData.resultImageB;
        }

        instructionText.text = isCorrect
    ? "Correct!"
    : "Try Again!";

        if (isCorrect)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);

            correctStreak++;

            progressBar.value = correctStreak;
            progressText.text = "Tutorial " + correctStreak + " / 3";

            StartCoroutine(NextTutorialQuestion());
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            StartCoroutine(RepeatCurrentQuestion());
        }

        draggedChoice.ReturnToStart();
    }

    IEnumerator NextTutorialQuestion()
    {
        yield return new WaitForSeconds(nextQuestionDelay);

        if (correctStreak >= 3)
        {
            FinishTutorial();
            yield break;
        }

        LoadNextQuestion();

        ResetInstruction();

        isTransitioning = false;
    }

    void FinishTutorial()
    {
        SceneManager.LoadScene(gameSceneName);
    }


    public void OnCardPicked(Module1DraggableChoice draggedCard)
    {
        instructionText.text =
            "Now drop the card onto the picture.";

        if (draggedCard == choiceCardA)
        {
            choiceCardB.SetCanDrag(false);
        }
        else if (draggedCard == choiceCardB)
        {
            choiceCardA.SetCanDrag(false);
        }
    }

    public void ResetInstruction()
    {
        instructionText.text =
            "Drag one of the picture cards.";

        choiceCardA.SetCanDrag(true);
        choiceCardB.SetCanDrag(true);
    }

    IEnumerator RepeatCurrentQuestion()
    {
        yield return new WaitForSeconds(nextQuestionDelay);

        correctStreak = 0;

        progressBar.value = 0;
        progressText.text = "Tutorial 0 / 3";

        LoadNextQuestion();

        ResetInstruction();

        isTransitioning = false;
    }

    void LoadNextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Count)
        {
            currentQuestionIndex = 0;
        }

        LoadQuestion();
    }
}