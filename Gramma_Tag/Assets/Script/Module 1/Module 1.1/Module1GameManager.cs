using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        LoadQuestionData();

        if (questions.Count == 0)
        {
            Debug.LogError("No Questions Loaded.");
            return;
        }

        currentQuestionIndex = 0;

        gameUI.SetActive(false);

        countdownPanel.SetActive(true);

        StartCoroutine(StartCountdown());
    }

    //=====================================================

    void LoadQuestion()
    {
        currentQuestion = questions[currentQuestionIndex];

        //-----------------------------------------

        questionText.text = currentQuestion.question;

        //-----------------------------------------

        choiceAText.text = currentQuestion.choiceA;
        choiceBText.text = currentQuestion.choiceB;

        //-----------------------------------------

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

        //-----------------------------------------

        UpdateProgress();

        choiceCardA.ResetCard();
        choiceCardB.ResetCard();
    }

    //=====================================================

    void UpdateProgress()
    {
        progressText.text =
            "Progress " +
            currentQuestionIndex +
            "/" +
            questions.Count;

        progressBar.maxValue = questions.Count;

        progressBar.value = currentQuestionIndex;
    }

    //=====================================================

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
        }
        else
        {
            Debug.Log("WRONG");
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
            FinishGame();

            yield break;
        }

        LoadQuestion();

        isTransitioning = false;
    }

    void FinishGame()
    {
        PlayerPrefs.SetInt("FinalScore", score);

        PlayerPrefs.SetInt("TotalQ", questions.Count);

        PlayerPrefs.SetInt("CoinsEarned", 0);

        PlayerPrefs.SetInt("Passed", score >= 6 ? 1 : 0);

        SceneManager.LoadScene(resultSceneName);
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

    void LoadQuestionData()
    {
        questions.Clear();

        questions.Add(new Module1QuestionData()
        {
            quizID = 1,
            moduleID = 1,

            question = "A house is on fire. What cause could make the house catch fire?",

            choiceA = "A child plays with matches.",

            choiceB = "A family watches television.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 2,
            moduleID = 1,

            question = "A room is dark. What cause could make the room dark?",

            choiceA = "Someone turns off the light.",

            choiceB = "Someone turns on the light.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 3,
            moduleID = 1,

            question = "The floor is wet. What cause could make the floor wet?",

            choiceA = "Someone spills water.",

            choiceB = "Someone sweeps the floor.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 4,
            moduleID = 1,

            question = "The plants are dry. What cause could make the plants dry?",

            choiceA = "They are not watered for many days.",

            choiceB = "They are watered every day.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 5,
            moduleID = 1,

            question = "The trash can is full. What cause could make the trash can full?",

            choiceA = "People throw their trash into it.",

            choiceB = "People keep their trash in their bags.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 6,
            moduleID = 1,

            question = "The books are on the floor. What cause could make the books fall to the floor?",

            choiceA = "Someone bumps the shelf.",

            choiceB = "Someone reads a book.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 7,
            moduleID = 1,

            question = "A cake is burned. What cause could make the cake burn?",

            choiceA = "It is left in the oven too long.",

            choiceB = "Someone decorates the cake.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 8,
            moduleID = 1,

            question = "The fire is getting bigger. What cause could make the fire grow bigger?",

            choiceA = "More wood is added.",

            choiceB = "Water is poured on the fire.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 9,
            moduleID = 1,

            question = "The clothes are wet. What cause could make the clothes wet?",

            choiceA = "It starts to rain.",

            choiceB = "Someone folds the clothes.",

            correctAnswer = "A"
        });

        questions.Add(new Module1QuestionData()
        {
            quizID = 10,
            moduleID = 1,

            question = "The TV is too loud. What cause could make the TV too loud?",

            choiceA = "Someone turns up the volume.",

            choiceB = "Someone turns off the television.",

            correctAnswer = "A"
        });
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
    }
}