using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;



public class Module5Manager : MonoBehaviour
{
    [Header("IMAGES")]
    public Image[] images; // drag Image1–Image4

    [Header("ANSWERS")]
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    [Header("UI TEXT")]
    public TMP_Text questionCounter;

    [Header("NEXT BUTTON")]
    public GameObject nextButton;

    [Header("HEARTS")]
    public Image[] hearts;

    [Header("QUESTIONS")]
    public List<Module5QuestionData> questions = new List<Module5QuestionData>();

    int currentQuestion = 0;
    int score = 0;
    int lives = 5;
    bool answered = false;

    void Start()
    {
        GenerateSampleQuestions(); // auto create sample data
        LoadQuestion();
        nextButton.SetActive(false);
        UpdateHearts();
    }

    void GenerateSampleQuestions()
    {
        Module5QuestionData q1 = new Module5QuestionData();

        q1.images = new Sprite[4]; // temporary muna (lalagyan natin sa inspector)

        q1.choices = new string[4];
        q1.choices[0] = "Sunny Day";
        q1.choices[1] = "Lots of Water";
        q1.choices[2] = "Go to the Park";
        q1.choices[3] = "Play Baseball";

        q1.correctAnswer = "Lots of Water";

        questions.Add(q1);
    }

    void LoadQuestion()
    {
        answered = false;
        nextButton.SetActive(false);

        Module5QuestionData q = questions[currentQuestion];

        // 🖼 LOAD 4 IMAGES
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = q.images[i];
        }

        // 🎯 LOAD ANSWERS
        for (int i = 0; i < 4; i++)
        {
            answerTexts[i].text = q.choices[i];

            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        questionCounter.text = "Question " + (currentQuestion + 1) + "/10";
    }

    void CheckAnswer(int index)
    {
        if (answered) return;

        answered = true;

        string selected = answerTexts[index].text;
        string correct = questions[currentQuestion].correctAnswer;

        if (selected == correct)
        {
            score++;
            answerButtons[index].image.color = Color.green;
        }
        else
        {
            answerButtons[index].image.color = Color.red;
            lives--;
            UpdateHearts();
        }

        nextButton.SetActive(true);
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < lives;
        }

        if (lives <= 0)
        {
            EndGame();
        }
    }

    public void Next()
    {
        currentQuestion++;

        if (currentQuestion >= questions.Count)
        {
            EndGame();
        }
        else
        {
            ResetButtons();
            LoadQuestion();
        }
    }

    void ResetButtons()
    {
        foreach (Button btn in answerButtons)
        {
            btn.image.color = Color.white;
        }
    }

    void EndGame()
    {
        int coins = score >= 7 ? 100 : 20;
        int stars = score >= 10 ? 3 : score >= 8 ? 2 : score >= 6 ? 1 : 0;

        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("CoinsEarned", coins);
        PlayerPrefs.SetInt("Stars", stars);

        SceneManager.LoadScene("ResultScene");
    }
}