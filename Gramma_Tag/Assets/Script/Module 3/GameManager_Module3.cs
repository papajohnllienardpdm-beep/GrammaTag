using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager_Module3 : MonoBehaviour
{
    public static GameManager_Module3 instance;

    public TextMeshProUGUI timerText;
    public float timer = 30f;
    private bool isTimerRunning = false;

    public enum SubtopicType
    {
        BeginningCH,
        EndingCH,
        BeginningSH,
        EndingSH
    }

    public SubtopicType currentSubtopic;

    public Image basketImage;
    public Sprite chSprite;
    public Sprite shSprite;

    public TextMeshProUGUI basketLabel;

    int lastIndex = -1;

    [Header("Feedback UI")]
    public GameObject correctPanel;
    public GameObject wrongPanel;



    void Awake()
    {
        Debug.Log("GameManager Awake: " + gameObject.name);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("DUPLICATE GameManager DESTROYED");
            Destroy(gameObject);
            return;
        }
    }

    public WordSpawner spawner;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI progressText;
    public Image progressBar;

    int current = 0;
    int score = 0;

    public bool hasActiveWord = false;

    void Start()
    {
        Debug.Log("ScoreText: " + scoreText);
        Debug.Log("ProgressBar: " + progressBar);

        Debug.Log("GameManager instance: " + this);

        if (hasActiveWord) return;

        if (spawner == null)
            spawner = FindObjectOfType<WordSpawner>();

        SpawnNext();
        UpdateScoreUI();
        StartTimer(); // 🔥 ADD THIS

        SetupBasket();


    }

    void SetupBasket()
    {
        if (basketImage == null) return;

        if (currentSubtopic == SubtopicType.BeginningCH || currentSubtopic == SubtopicType.EndingCH)
        {
            basketImage.sprite = chSprite;
            basketLabel.text = "";
        }
        else
        {
            basketImage.sprite = shSprite;
            basketLabel.text = "";
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
    public void Answer(bool correct)
    {
        StopTimer();

        if (current >= 10) return;

        hasActiveWord = false;

        ShowFeedback(correct); // ✅

        if (correct)
        {
            score++;
        }

        current++;
        UpdateScoreUI();

        Invoke(nameof(SpawnNext), 1.5f); // delay

        Debug.Log("ANSWER: " + correct);
    }

    void SpawnNext()
    {
        if (hasActiveWord) return;

        if (current >= 10)
        {
            PlayerPrefs.SetInt("FinalScore", score);
            PlayerPrefs.SetInt("TotalQ", 10);
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();

            SceneManager.LoadScene("ResultScene");
            return; // 🔥 VERY IMPORTANT
        }

        int index;

        do
        {
            index = Random.Range(0, spawner.words.Length);
        }
        while (index == lastIndex);

        lastIndex = index;
        spawner.Spawn(index);
        hasActiveWord = true;

        ResetTimer();   // 🔥 reset timer every question
        StartTimer();   // 🔥 start again
    }

    void UpdateScoreUI()
    {
        // optional: kung ayaw mo na ng score
        // scoreText.text = "Score: " + score.ToString();

        if (progressText != null)
        {
            progressText.text = "Progress " + current + "/10";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)current / 10f;
        }
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

        Debug.Log("Time's up!");

        // treat as WRONG answer
        Answer(false);
    }

    public bool IsCorrectWord(string word)
    {
        switch (currentSubtopic)
        {
            case SubtopicType.BeginningCH:
                return word.StartsWith("ch");

            case SubtopicType.EndingCH:
                return word.EndsWith("ch");

            case SubtopicType.BeginningSH:
                return word.StartsWith("sh");

            case SubtopicType.EndingSH:
                return word.EndsWith("sh");
        }

        return false;
    }

    public void ShowFeedback(bool correct)
    {
        // 🔥 siguraduhing nasa ibabaw
        correctPanel.transform.SetAsLastSibling();
        wrongPanel.transform.SetAsLastSibling();

        if (correct)
        {
            correctPanel.SetActive(true);
            wrongPanel.SetActive(false);
        }
        else
        {
            wrongPanel.SetActive(true);
            correctPanel.SetActive(false);
        }

        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 1.5f);
    }

    void HideFeedback()
    {
        correctPanel.SetActive(false);
        wrongPanel.SetActive(false);
    }


}