using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class Module4GameManager : MonoBehaviour
{
    // ===== TIMER =====
    [Header("UI - Timer")]
    public TMP_Text timerText;

    [Header("Game Timer Settings")]
    public float gameDuration = 300f; // 5 minutes (editable sa inspector)

    private float timer;
    private bool isTimerRunning = false;

    // =================

    [Header("UI - Question")]
    public TMP_Text sentenceText;

    [Header("UI - Answers")]
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;



    [Header("UI - Navigation")]


    [Header("UI - Progress")]
    public TMP_Text progressText;
    public Slider progressBar;

    [Header("Questions")]
    public List<QuestionData> questions = new List<QuestionData>();

    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool answered = false;

    public float nextDelay = 1.5f; // ilang seconds bago next

    Color correctColor = new Color(0.2f, 0.8f, 0.2f); // bright green
    Color wrongColor = new Color(1f, 0.3f, 0.3f);     // soft red
    Color normalColor = Color.white;
    Color dimColor = new Color(0.7f, 0.7f, 0.7f);     // gray

    [Header("Start Countdown")]
    public GameObject getReadyText;
    public GameObject gameUI;

    void Start()
    {
        SetupSampleQuestions();

        timer = gameDuration;

        if (getReadyText != null)
        {
            gameUI.SetActive(false); // hide muna
            StartCoroutine(StartGameWithDelay());
        }
        else
        {
            gameUI.SetActive(true);
            StartTimer();
            LoadQuestion();
        }
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


        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = true;

            answerTexts[i].text = q.choices[i];

            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(q.choices[index]));
        }




        UpdateProgress();

        foreach (Button btn in answerButtons)
        {
            btn.interactable = true;

            Image img = btn.GetComponent<Image>();
            img.color = normalColor;
        }

        if (sentenceText != null)
            sentenceText.text = q.sentenceText;


    }

    // =============================
    // SELECT ANSWER
    // =============================
    void SelectAnswer(string selectedAnswer)
    {
        if (answered) return;

        answered = true;

        QuestionData q = questions[currentQuestionIndex];

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button btn = answerButtons[i];
            btn.interactable = false;

            Image img = btn.GetComponent<Image>();
            TMP_Text txt = answerTexts[i];

            string choice = q.choices[i];

            // 👉 default: dim lahat
            img.color = dimColor;
            txt.color = new Color(0.3f, 0.3f, 0.3f); // dark gray text

            // ✅ correct answer (ONLY THIS STANDS OUT)
            if (choice == q.correctAnswer)
            {
                img.color = new Color(0.2f, 1f, 0.2f); // bright green
                txt.color = Color.white;
            }

            // ❌ maling pinili mo (optional highlight)
            if (choice == selectedAnswer && choice != q.correctAnswer)
            {
                img.color = new Color(1f, 0.3f, 0.3f); // red
                txt.color = Color.white;
            }
        }

        if (selectedAnswer == q.correctAnswer)
        {
            score++;
        }

        Invoke(nameof(NextQuestion), nextDelay);
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
        progressBar.value = (float)(currentQuestionIndex) / questions.Count;
        progressBar.maxValue = 1f;


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
            sentenceText = "This is John's toy.\nIt is ____",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"His\" shows that the toy belongs to John."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is Maria's bag.\nIt is ____",
            correctAnswer = "her",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Her\" shows that the bag belongs to Maria."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "These are Ben and Ana's toys.\nThey are ____",
            correctAnswer = "their",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Their\" is used for more than one owner."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is my book.\nIt is ____",
            correctAnswer = "my",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"My\" shows that the speaker owns the book."
        });

        // 👉 dagdag pa
        questions.Add(new QuestionData
        {
            sentenceText = "This is Ben's hat.\nIt is ____",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"His\" shows that the hat belongs to Ben."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is Anna's dress.\nIt is ____",
            correctAnswer = "her",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Her\" shows that the dress belongs to Anna."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "These are the kids' shoes.\nThey are ____",
            correctAnswer = "their",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Their\" is used for plural owners."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is my pencil.\nIt is ____",
            correctAnswer = "my",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"My\" shows ownership of the speaker."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is Carlo's book.\nIt is ____",
            correctAnswer = "his",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"His\" shows that the book belongs to Carlo."
        });

        questions.Add(new QuestionData
        {
            sentenceText = "This is Mia's bag.\nIt is ____",
            correctAnswer = "her",
            choices = new string[] { "his", "her", "their", "my" },
            explanation = "\"Her\" shows that the bag belongs to Mia."
        });
    }

    public void StartTimer()
    {
        timer = gameDuration; // ✔ tama
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
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (timer <= 5)
                timerText.color = Color.red;
            else
                timerText.color = Color.black;
        }
    }

    void TimeUp()
    {
        isTimerRunning = false;

        if (answered) return;

        answered = true;

        QuestionData q = questions[currentQuestionIndex];


        // disable buttons
        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }




        // 👉 auto next after delay
        Invoke(nameof(NextQuestion), nextDelay);
    }

    IEnumerator StartGameWithDelay()
    {
        getReadyText.SetActive(true);

        TMP_Text txt = getReadyText.GetComponent<TMP_Text>();

        txt.text = "3";
        yield return new WaitForSeconds(1f);

        txt.text = "2";
        yield return new WaitForSeconds(1f);

        txt.text = "1";
        yield return new WaitForSeconds(1f);

        txt.text = "GO!";
        yield return new WaitForSeconds(0.8f);

        getReadyText.SetActive(false);

        gameUI.SetActive(true);

        StartTimer();
        LoadQuestion();
    }
}