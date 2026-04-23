using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;


public class Module6Manager : MonoBehaviour
{

    Color normalColor = Color.white;
    Color dimColor = new Color(0.7f, 0.7f, 0.7f); // gray
    Color correctColor = Color.green;
    Color wrongColor = Color.red;
    // ===== TIMER =====
    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Game Timer")]
    public float totalGameTime = 300f; // 5 minutes (editable sa Inspector)

    private float timer;
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
    public Image progressBar;

    private List<Module6QuestionData> allQuestions = new List<Module6QuestionData>();
    private List<Module6QuestionData> selectedQuestions = new List<Module6QuestionData>();

    private int currentQuestion = 0;
    private bool hasAnswered = false;

    private int score = 0;

    public Image overlayA;
    public Image overlayB;
    public Image overlayC;
    public Image overlayD;

    public GameObject getReadyText;

    public GameObject gameUI;
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
    timer = totalGameTime;

    if (getReadyText != null)
    {
        gameUI.SetActive(false); // 🔥 HIDE muna
        StartCoroutine(StartGameWithDelay());
    }
    else
    {
        gameUI.SetActive(true);
        isTimerRunning = true;
        UpdateTimerUI();
        LoadQuestion();
    }
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

                Debug.Log("⏰ TIME'S UP - GAME OVER");

                EndGameDueToTime(); // 🔥 bagong function
            }

            UpdateTimerUI();
        }
    }

    void CreateQuestions()
    {
        allQuestions.Add(new Module6QuestionData
        {
            question = "Maria woke up early in the morning. She wore her uniform, packed her bag, and waited for the school service outside their house.",
            choices = new string[]
            {
            "Maria will go to the market",
            "Maria will go to school",
            "Maria will visit a friend",
            "Maria will stay at home"
            },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "John brought an umbrella and wore boots. Dark clouds filled the sky.",
            choices = new string[]
            {
            "It is sunny",
            "It will rain",
            "It is very hot",
            "It is night time"
            },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Anna is holding a birthday cake with candles. Her friends are singing around her.",
            choices = new string[]
            {
            "Anna is at school",
            "Anna is celebrating her birthday",
            "Anna is cooking dinner",
            "Anna is going to sleep"
            },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Tom is wearing a jacket, scarf, and gloves. His breath can be seen in the air.",
            choices = new string[]
            {
            "It is very hot",
            "It is raining",
            "It is cold",
            "It is summer"
            },
            correctIndex = 2
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Lisa is holding a book and sitting quietly in a room full of shelves with many books.",
            choices = new string[]
            {
            "She is in a library",
            "She is in a playground",
            "She is in a market",
            "She is in a hospital"
            },
            correctIndex = 0
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Mark is sweating and drinking water while the sun is shining brightly above him.",
            choices = new string[]
            {
            "It is raining",
            "It is cold",
            "It is hot",
            "It is night"
            },
            correctIndex = 2
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "The ground is wet and people are carrying umbrellas.",
            choices = new string[]
            {
            "It is sunny",
            "It has rained",
            "It is windy",
            "It is night"
            },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Ben is wearing a swimsuit and playing in the water with a beach ball.",
            choices = new string[]
            {
            "He is at school",
            "He is at the beach",
            "He is in a hospital",
            "He is in a library"
            },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "Sara is carrying many shopping bags and walking out of a store.",
            choices = new string[]
            {
            "She is studying",
            "She is shopping",
            "She is cooking",
            "She is sleeping"
            },
            correctIndex = 1
        });

        allQuestions.Add(new Module6QuestionData
        {
            question = "The lights are off and the children are lying in their beds with their eyes closed.",
            choices = new string[]
            {
            "They are eating",
            "They are playing",
            "They are sleeping",
            "They are studying"
            },
            correctIndex = 2
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

        textA.text = q.choices[0];
        textB.text = q.choices[1];
        textC.text = q.choices[2];
        textD.text = q.choices[3];

        ResetButtons();

        progressBar.fillAmount = (float)(currentQuestion) / selectedQuestions.Count;
        levelText.text = "Question " + (currentQuestion + 1) + "/" + selectedQuestions.Count;




    }

    void CheckAnswer(int index)
    {


        if (hasAnswered) return;
        hasAnswered = true;

        Module6QuestionData q = selectedQuestions[currentQuestion];

        // 👉 dim lahat (overlay gray)
        SetAllOverlayColor(new Color(0, 0, 0, 0.4f));

        if (index == q.correctIndex)
        {
            score++;
            GetOverlay(index).color = new Color(0, 1, 0, 0.6f); // green
        }
        else
        {
            GetOverlay(index).color = new Color(1, 0, 0, 0.6f); // red
            GetOverlay(q.correctIndex).color = new Color(0, 1, 0, 0.6f); // green
        }

        DisableAllButtons();
        Invoke(nameof(NextQuestion), 1.5f);
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

        SetAllOverlayColor(new Color(0, 0, 0, 0)); // transparent
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

        if (hasAnswered) return;
        hasAnswered = true;

        Module6QuestionData q = selectedQuestions[currentQuestion];

        SetAllOverlayColor(new Color(0, 0, 0, 0.4f));
        GetOverlay(q.correctIndex).color = new Color(0, 1, 0, 0.6f);

        DisableAllButtons();
        Invoke(nameof(NextQuestion), 1.5f);
    }

    void SetAllButtonsColor(Color color)
    {
        buttonA.GetComponent<Image>().color = color;
        buttonB.GetComponent<Image>().color = color;
        buttonC.GetComponent<Image>().color = color;
        buttonD.GetComponent<Image>().color = color;
    }

    void SetAllOverlayColor(Color color)
    {
        overlayA.color = color;
        overlayB.color = color;
        overlayC.color = color;
        overlayD.color = color;
    }

    void EndGameDueToTime()
    {
        isTimerRunning = false;

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", selectedQuestions.Count);
        PlayerPrefs.SetString("LastScene", "Module6Game");

        SceneManager.LoadScene("ResultScene");
    }

    IEnumerator StartGameWithDelay()
{
    getReadyText.SetActive(true);

    TextMeshProUGUI txt = getReadyText.GetComponent<TextMeshProUGUI>();

    txt.text = "3";
    yield return new WaitForSeconds(1f);

    txt.text = "2";
    yield return new WaitForSeconds(1f);

    txt.text = "1";
    yield return new WaitForSeconds(1f);

    txt.text = "GO!";
    yield return new WaitForSeconds(0.8f);

    getReadyText.SetActive(false);

    gameUI.SetActive(true); // 🔥 SHOW ulit

    isTimerRunning = true;
    UpdateTimerUI();
    LoadQuestion();
}
}