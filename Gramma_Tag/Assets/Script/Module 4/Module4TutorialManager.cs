using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Module4TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text sentenceText;
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    public TMP_Text progressText;
    public Slider progressBar;
    public TMP_Text tutorialText;

    private int correctStreak = 0;
    private bool answered = false;

    public float nextDelay = 1.5f;

    Color correctColor = new Color(0.2f, 1f, 0.2f);
    Color wrongColor = new Color(1f, 0.3f, 0.3f);
    Color normalColor = Color.white;
    Color dimColor = new Color(0.7f, 0.7f, 0.7f);

    // ✅ NEW (IMPORTANT)
    List<QuestionData> questions = new List<QuestionData>();
    int currentIndex = 0;
    QuestionData currentQuestion;

    void Start()
    {
        CreateQuestions();
        ShuffleQuestions();

        tutorialText.text = "Read the sentence and choose the correct answer.";

        LoadQuestion();
        UpdateProgress();
    }

    void CreateQuestions()
    {
        questions.Clear();

        questions.Add(new QuestionData
        {
            sentenceText = "This is John's toy.\nIt is ____",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" }
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is Maria's bag.\nIt is ____",
            correctAnswer = "her",
            choices = new string[] { "his", "her", "their", "my" }
        });

        questions.Add(new QuestionData
        {
            sentenceText = "These are Ben and Ana's toys.\nThey are ____",
            correctAnswer = "their",
            choices = new string[] { "his", "her", "their", "my" }
        });
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            QuestionData temp = questions[i];
            int rand = Random.Range(i, questions.Count);
            questions[i] = questions[rand];
            questions[rand] = temp;
        }
    }

    void LoadQuestion()
    {
        answered = false;

        currentQuestion = questions[currentIndex];

        sentenceText.text = currentQuestion.sentenceText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;

            answerButtons[i].interactable = true;
            answerTexts[i].text = currentQuestion.choices[i];

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(currentQuestion.choices[index]));

            answerButtons[i].GetComponent<Image>().color = normalColor;
            answerTexts[i].color = Color.black;
        }
    }

    void SelectAnswer(string selected)
    {
        if (answered) return;
        answered = true;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button btn = answerButtons[i];
            btn.interactable = false;

            Image img = btn.GetComponent<Image>();
            TMP_Text txt = answerTexts[i];

            string choice = currentQuestion.choices[i];

            img.color = dimColor;
            txt.color = Color.gray;

            if (choice == currentQuestion.correctAnswer)
            {
                img.color = correctColor;
                txt.color = Color.white;
            }

            if (choice == selected && choice != currentQuestion.correctAnswer)
            {
                img.color = wrongColor;
                txt.color = Color.white;
            }
        }

        if (selected == currentQuestion.correctAnswer)
        {
            correctStreak++;
            tutorialText.text = "Correct!";
        }
        else
        {
            correctStreak = 0;
            tutorialText.text = "Oops! Try again from the start.";
        }

        Invoke(nameof(NextStep), nextDelay);
    }

    void NextStep()
    {
        if (correctStreak >= 3)
        {
            SceneManager.LoadScene("Module4Game");
            return;
        }

        currentIndex++;

        if (currentIndex >= questions.Count)
        {
            currentIndex = 0;
            ShuffleQuestions();
        }

        UpdateProgress();
        LoadQuestion();
    }

    void UpdateProgress()
    {
        progressText.text = "Tutorial " + (correctStreak + 1) + "/3";
        progressBar.value = (float)correctStreak / 3f;

        if (correctStreak == 0)
        {
            tutorialText.text = "Read the sentence and choose the correct answer.";
        }
        else if (correctStreak == 1)
        {
            tutorialText.text = "Good! Try another one.";
        }
        else if (correctStreak == 2)
        {
            tutorialText.text = "Last one! Get this right.";
        }
    }
}