using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;



public class Module4GameManager : MonoBehaviour
{
    // ===== TIMER =====
    [Header("UI - Timer")]
    public TMP_Text timerText;

    private float timer = 30f;
    private bool isTimerRunning = false;
    // =================

    [Header("UI - Question")]
    public TMP_Text sentenceText;
    public Image questionImage;

    [Header("UI - Answers")]
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    [Header("UI - Feedback")]
    public GameObject feedbackPanel;
    public TMP_Text feedbackText;
    public TMP_Text explanationText;

    [Header("UI - Navigation")]
    public Button nextButton;

    [Header("UI - Progress")]
    public TMP_Text progressText;
    public Slider progressBar;

    [Header("Questions")]
    public List<QuestionData> questions = new List<QuestionData>();

    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool answered = false;

    void Start()
    {
        feedbackPanel.SetActive(false);
        nextButton.interactable = false;

        SetupSampleQuestions(); // temporary (pwede mo palitan later)

        LoadQuestion();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                UpdateTimerUI();
                TimeUp();
            }

            UpdateTimerUI();
        }
    }

    // =============================
    // LOAD QUESTION
    // =============================
    void LoadQuestion()
    {
        answered = false;

        QuestionData q = questions[currentQuestionIndex];

        sentenceText.text = q.sentenceText;
        questionImage.sprite = q.image;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = true;

            answerTexts[i].text = q.choices[i];

            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(q.choices[index]));
        }

        feedbackPanel.SetActive(false);
        nextButton.interactable = false;

        UpdateProgress();
    }

    // =============================
    // SELECT ANSWER
    // =============================
    void SelectAnswer(string selectedAnswer)
    {
        StopTimer(); // 🔥 important

        if (answered) return;

        answered = true;

        QuestionData q = questions[currentQuestionIndex];

        feedbackPanel.SetActive(true);
        nextButton.interactable = true;

        // Disable buttons
        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }

        if (selectedAnswer == q.correctAnswer)
        {
            feedbackText.text = "CORRECT";
            feedbackText.color = Color.green;
            score++;
        }
        else
        {
            feedbackText.text = "WRONG";
            feedbackText.color = Color.red;
        }

        explanationText.text = q.explanation;
    }

    // =============================
    // NEXT BUTTON
    // =============================
    public void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Count)
        {
            FinishGame();
            return;
        }

        LoadQuestion();
    }

    // =============================
    // PROGRESS UPDATE
    // =============================
    void UpdateProgress()
    {
        progressText.text = (currentQuestionIndex + 1) + "/" + questions.Count;
        progressBar.maxValue = questions.Count;
        progressBar.value = currentQuestionIndex + 1;

        ResetTimer();
        StartTimer();
    }

    // =============================
    // FINISH GAME → RESULT SCENE
    // =============================

    void FinishGame()
    {
        int total = questions.Count;
        int coins = (score >= 7) ? 100 : 20;

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("CoinsEarned", coins);

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("ResultScene");
    }

    // =============================
    // SAMPLE QUESTIONS (TEMP ONLY)
    // =============================
    void SetupSampleQuestions()
    {
        questions.Clear();

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "This is ____ ball.",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"His\" shows that the ball belongs to the boy."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "That is ____ bag.",
            correctAnswer = "her",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Her\" shows that the bag belongs to the girl."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "These are ____ toys.",
            correctAnswer = "their",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Their\" is used for more than one owner."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "This is ____ book.",
            correctAnswer = "my",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"My\" shows that the speaker owns the book."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "That is ____ pencil.",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"His\" is used for a boy's possession."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "This is ____ dress.",
            correctAnswer = "her",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Her\" is used for a girl's possession."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "These are ____ books.",
            correctAnswer = "their",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Their\" shows ownership by many people."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "This is ____ toy car.",
            correctAnswer = "my",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"My\" shows that it belongs to the speaker."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "That is ____ hat.",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"His\" shows that the hat belongs to the boy."
        });

        questions.Add(new QuestionData
        {
            questionType = QuestionType.MultipleChoice,
            sentenceText = "These are ____ shoes.",
            correctAnswer = "their",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Their\" is used for plural ownership."
        });
    }

    public void StartTimer()
    {
        timer = 30f;
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        timer = 30f;
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 5)
                timerText.color = Color.red;
            else
                timerText.color = Color.white;
        }
    }

    void TimeUp()
    {
        isTimerRunning = false;

        if (answered) return;

        answered = true;

        QuestionData q = questions[currentQuestionIndex];

        feedbackPanel.SetActive(true);
        nextButton.interactable = true;

        // disable buttons
        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }

        feedbackText.text = "TIME'S UP!";
        feedbackText.color = Color.red;

        explanationText.text = q.explanation;
    }
}