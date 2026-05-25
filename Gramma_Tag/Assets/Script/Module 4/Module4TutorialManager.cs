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

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

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

        // ✅ IMPORTANT FOR SLIDER
        progressBar.maxValue = 3;
        progressBar.value = 0;

        LoadQuestion();
        UpdateProgress();
    }

    void CreateQuestions()
    {
        questions.Clear();

        questions.Add(new QuestionData
        {
            sentenceText = "This is John's toy.\nIt is ____.",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" }
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is Maria's bag.\nIt is ____.",
            correctAnswer = "her",
            choices = new string[] { "his", "hers", "their", "my" }
        });

        questions.Add(new QuestionData
        {
            sentenceText = "These are Ben and Ana's toys.\nThey are ____.",
            correctAnswer = "their",
            choices = new string[] { "his", "her", "theirs", "my" }
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

            // 🔊 PLAY CORRECT SFX
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);

            UpdateProgress();
        }
        else
        {
            correctStreak = 0;
            tutorialText.text = "Oops! Try again from the start.";

            // 🔊 PLAY WRONG SFX
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            UpdateProgress(); // para reset agad UI
        }

        Invoke(nameof(NextStep), nextDelay);
    }

    void NextStep()
    {
        if (correctStreak >= 3)
        {
            // ✅ para makita 3/3 + sound
            Invoke(nameof(GoToGame), 0.8f);
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

    void GoToGame()
    {
        SceneManager.LoadScene("Module4Game");
    }

    void UpdateProgress()
    {
        // ✅ FIXED TEXT (start 0/3)
        progressText.text = "Tutorial " + correctStreak + " / 3";

        // ✅ FIXED SLIDER (0 to 3)
        progressBar.value = correctStreak;

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