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
    public Image progressBar;

    int current = 0;
    int score = 0;

    bool hasActiveWord = false;

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
        StopTimer(); // 🔥 stop timer when answered

        if (current >= 10) return;

        hasActiveWord = false;

        if (correct)
        {
            score++;
        }

        UpdateScoreUI();

        current++;

        SpawnNext();
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

        spawner.Spawn(current);
        hasActiveWord = true;

        ResetTimer();   // 🔥 reset timer every question
        StartTimer();   // 🔥 start again
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();

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
}