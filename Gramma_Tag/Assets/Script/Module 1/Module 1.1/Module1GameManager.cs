using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Module1GameManager : MonoBehaviour
{
    #region Question Data

    [Header("Questions")]
    public List<Module1QuestionData> questions = new List<Module1QuestionData>();

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

    private void Start()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No Questions Found!");
            return;
        }

        currentQuestionIndex = 0;

        LoadQuestion();
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

        choiceAImage.sprite = currentQuestion.choiceAImage;
        choiceBImage.sprite = currentQuestion.choiceBImage;

        //-----------------------------------------

        scenarioImage.sprite = currentQuestion.questionImage;

        //-----------------------------------------

        UpdateProgress();
    }

    //=====================================================

    void UpdateProgress()
    {
        progressText.text =
            "Progress " +
            (currentQuestionIndex + 1) +
            "/" +
            questions.Count;

        progressBar.maxValue = questions.Count;

        progressBar.value = currentQuestionIndex + 1;
    }

    //=====================================================

    public void OnChoiceDropped(Module1DraggableChoice draggedChoice)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        bool isCorrect =
            draggedChoice.choiceKey ==
            currentQuestion.correctAnswer;

        //--------------------------------------------------

        if (draggedChoice.choiceKey == "A")
        {
            scenarioImage.sprite =
                currentQuestion.resultImageA;
        }
        else
        {
            scenarioImage.sprite =
                currentQuestion.resultImageB;
        }

        //--------------------------------------------------

        if (isCorrect)
        {
            score++;

            Debug.Log("CORRECT");
        }
        else
        {
            Debug.Log("WRONG");
        }

        //--------------------------------------------------

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
}