using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Module6Manager : MonoBehaviour
{
    // ===== TIMER =====
    [Header("Timer")]
    public TextMeshProUGUI timerText;

    private float timer = 30f;
    private bool isTimerRunning = false;
    // =================

    [Header("Buttons")]
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;
    public Button buttonD;

    [Header("Texts")]
    public TextMeshProUGUI textA;
    public TextMeshProUGUI textB;
    public TextMeshProUGUI textC;
    public TextMeshProUGUI textD;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI levelText;

    [Header("UI")]
    public Slider progressBar;
    public Button nextButton;

    private List<Module6QuestionData> allQuestions = new List<Module6QuestionData>();
    private List<Module6QuestionData> selectedQuestions = new List<Module6QuestionData>();

    private int currentQuestion = 0;
    private bool hasAnswered = false;

    private int score = 0;

    void Start()
    {
        buttonA.onClick.AddListener(() => CheckAnswer(0));
        buttonB.onClick.AddListener(() => CheckAnswer(1));
        buttonC.onClick.AddListener(() => CheckAnswer(2));
        buttonD.onClick.AddListener(() => CheckAnswer(3));

        CreateQuestions();
        PickRandomQuestions();

        if (selectedQuestions.Count > 0)
        {
            LoadQuestion();
        }

        nextButton.interactable = false;
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

    void CreateQuestions()
    {
        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Leaving lights on",
            choices = new string[] { "Saving Money", "Using Less Energy", "No Power", "High Bill" },
            correctIndex = 3
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Heavy rain",
            choices = new string[] { "Flood", "Sunny Day", "Dry Ground", "Clean Sky" },
            correctIndex = 0
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Not studying",
            choices = new string[] { "High Grades", "Failing Test", "Smart", "Top Student" },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Eating junk food",
            choices = new string[] { "Healthy Body", "Strong Muscles", "Getting Sick", "Good Health" },
            correctIndex = 2
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: No sleep",
            choices = new string[] { "Energetic", "Sleepy", "Healthy", "Strong" },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Exercising daily",
            choices = new string[] { "Weak body", "Healthy body", "Sick", "Tired always" },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Studying hard",
            choices = new string[] { "Failing", "Low grades", "High grades", "Confused" },
            correctIndex = 2
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Drinking dirty water",
            choices = new string[] { "Healthy", "Strong", "Sick", "Happy" },
            correctIndex = 2
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Planting trees",
            choices = new string[] { "Dirty air", "Clean environment", "Pollution", "Hot weather" },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Cause: Throwing trash anywhere",
            choices = new string[] { "Clean surroundings", "Flood", "Beautiful place", "Healthy area" },
            correctIndex = 1
        });
    }

    void PickRandomQuestions()
    {
        List<Module6QuestionData> temp = new List<Module6QuestionData>(allQuestions);

        int questionCount = Mathf.Min(10, temp.Count);

        for (int i = 0; i < questionCount; i++)
        {
            int rand = Random.Range(0, temp.Count);
            selectedQuestions.Add(temp[rand]);
            temp.RemoveAt(rand);
        }
    }

    void LoadQuestion()
    {
        if (currentQuestion >= selectedQuestions.Count)
        {
            Debug.LogError("No more questions!");
            return;
        }

        hasAnswered = false;

        Module6QuestionData q = selectedQuestions[currentQuestion];

        questionText.text = q.question;

        textA.text = "[A] " + q.choices[0];
        textB.text = "[B] " + q.choices[1];
        textC.text = "[C] " + q.choices[2];
        textD.text = "[D] " + q.choices[3];

        ResetButtons();

        progressBar.value = (float)(currentQuestion + 1) / selectedQuestions.Count;
        levelText.text = "Question " + (currentQuestion + 1) + "/" + selectedQuestions.Count;

        nextButton.interactable = false;

        ResetTimer();
        StartTimer();
    }

    void CheckAnswer(int index)
    {
        StopTimer(); // 🔥 important

        if (hasAnswered) return;

        hasAnswered = true;

        Module6QuestionData q = selectedQuestions[currentQuestion];

        if (index == q.correctIndex)
        {
            score++; // 🔥 ADD THIS
            GetButton(index).GetComponent<Image>().color = Color.green;
        }
        else
        {
            GetButton(index).GetComponent<Image>().color = Color.red;
            GetButton(q.correctIndex).GetComponent<Image>().color = Color.green;
        }

        DisableAllButtons();
        nextButton.interactable = true;
    }

    public void NextQuestion()
    {
        currentQuestion++;

        if (currentQuestion >= selectedQuestions.Count)
        {
            Debug.Log("GAME COMPLETE");
            PlayerPrefs.SetInt("FinalScore", score);
            PlayerPrefs.SetInt("TotalQ", selectedQuestions.Count);
            PlayerPrefs.SetString("LastScene", "Module6Game");

            SceneManager.LoadScene("ResultScene");
            return;
        }

        LoadQuestion();
    }

    void DisableAllButtons()
    {
        buttonA.interactable = false;
        buttonB.interactable = false;
        buttonC.interactable = false;
        buttonD.interactable = false;
    }

    void ResetButtons()
    {
        buttonA.interactable = true;
        buttonB.interactable = true;
        buttonC.interactable = true;
        buttonD.interactable = true;

        buttonA.GetComponent<Image>().color = Color.white;
        buttonB.GetComponent<Image>().color = Color.white;
        buttonC.GetComponent<Image>().color = Color.white;
        buttonD.GetComponent<Image>().color = Color.white;
    }

    Button GetButton(int index)
    {
        switch (index)
        {
            case 0: return buttonA;
            case 1: return buttonB;
            case 2: return buttonC;
            case 3: return buttonD;
        }
        return null;
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

        if (hasAnswered) return;

        hasAnswered = true;

        Module6QuestionData q = selectedQuestions[currentQuestion];

        // highlight correct answer
        GetButton(q.correctIndex).GetComponent<Image>().color = Color.green;

        DisableAllButtons();

        nextButton.interactable = true;
    }
}