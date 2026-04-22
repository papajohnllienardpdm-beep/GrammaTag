using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager_Module3 : MonoBehaviour
{
    public static GameManager_Module3 instance;

    public TextMeshProUGUI timerText;
    [Header("Game Timer")]
    public float gameDuration = 300f; // 5 minutes default

    private float timer;
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


    [Header("Tutorial Mode")]
    public bool isTutorial = false;
    public int tutorialTarget = 3;

    private int tutorialScore = 0;

    public TextMeshProUGUI tutorialText; // instruction text

    public GameObject getReadyText;


    void Awake()
    {
        Debug.Log("GameManager Loaded: " + gameObject.name);
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
        if (hasActiveWord) return;

        if (spawner == null)
            spawner = FindObjectOfType<WordSpawner>();

        SetupBasket();

        if (isTutorial)
        {
            // ✅ TUTORIAL FLOW
            if (timerText != null)
                timerText.gameObject.SetActive(false);

            SpawnNext(); // diretso laro
        }
        else
        {
            // ✅ GAME FLOW
            timer = gameDuration;

            if (getReadyText != null)
            {
                StartCoroutine(StartGameWithDelay()); // 🔥 once lang
            }
            else
            {
                StartTimer();
                SpawnNext();
            }
        }

        UpdateScoreUI();
        UpdateTutorialText();
    }

    void SetupBasket()
    {
        if (basketLabel == null) return;

        if (currentSubtopic == SubtopicType.BeginningCH || currentSubtopic == SubtopicType.EndingCH)
        {
            basketImage.sprite = chSprite;
            basketLabel.text = "CH"; // ✅ IMPORTANT
        }
        else
        {
            basketImage.sprite = shSprite;
            basketLabel.text = "SH"; // ✅ IMPORTANT
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
        hasActiveWord = false;

        // 🔥 TUTORIAL MODE
        if (isTutorial)
        {
            if (!correct)
            {
                Debug.Log("WRONG → RESET");

                ResetTutorial(); // 🔥 reset to 0
                SpawnNext();

                return;
            }

            // ✅ tama
            tutorialScore++;
            UpdateScoreUI();
            UpdateTutorialText();

            if (tutorialScore >= tutorialTarget)
            {
                Debug.Log("TUTORIAL COMPLETE");
                SceneManager.LoadScene("Module3_GameScene");
                return;
            }

            SpawnNext();
            return;
        }



        // 🔽 NORMAL GAME
        if (current >= 10) return;

        if (correct)
        {
            score++;
        }

        current++;
        UpdateScoreUI();

        SpawnNext();

        Debug.Log("ANSWER: " + correct);
    }

    void SpawnNext()
    {
        if (hasActiveWord) return;

        if (!isTutorial && current >= 10)
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


    }

    void UpdateScoreUI()
    {
        if (isTutorial)
        {
            if (progressText != null)
                progressText.text = tutorialScore + " / " + tutorialTarget;

            if (progressBar != null)
                progressBar.fillAmount = (float)tutorialScore / tutorialTarget;

            return;
        }

        // normal game
        if (progressText != null)
            progressText.text = "Progress " + current + "/10";

        if (progressBar != null)
            progressBar.fillAmount = (float)current / 10f;
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }



    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (timer <= 10)
                timerText.color = Color.red;
            else
                timerText.color = Color.black;
        }
    }

    void TimeUp()
    {
        isTimerRunning = false;

        Debug.Log("TIME'S UP - GAME OVER");

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", current);
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        SceneManager.LoadScene("ResultScene");
    }

    public bool IsCorrectWord(string word)
    {
        string basket = basketLabel.text.ToLower();

        Debug.Log("WORD: " + word);
        Debug.Log("BASKET: " + basket);

        if (currentSubtopic == SubtopicType.BeginningCH || currentSubtopic == SubtopicType.BeginningSH)
        {
            return word.StartsWith(basket);
        }
        else
        {
            return word.EndsWith(basket);
        }
    }

    void UpdateTutorialText()
    {
        if (!isTutorial || tutorialText == null) return;

        if (tutorialScore == 0)
            tutorialText.text = "Catch a word that starts with " + basketLabel.text;

        else if (tutorialScore == 1)
            tutorialText.text = "Good! Catch another one!";

        else if (tutorialScore == 2)
            tutorialText.text = "Great! One more!";
    }

    void ShowWrongFeedback()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "Oops! Try again!";

            CancelInvoke(nameof(UpdateTutorialText));
            Invoke(nameof(UpdateTutorialText), 1.5f);
        }
    }



    public void ResetTutorial()
    {
        tutorialScore = 0;

        UpdateScoreUI();        // 🔥 IMPORTANT (ito ang kulang)
        UpdateTutorialText();

        ShowWrongFeedback();
    }

    public void ShowAvoidFeedback()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "Good! Avoid wrong words!";

            CancelInvoke(nameof(UpdateTutorialText));
            Invoke(nameof(UpdateTutorialText), 1.2f);
        }
    }

    IEnumerator StartGameWithDelay()
    {
        if (getReadyText == null)
        {
            StartTimer(); // 🔥 fallback
            SpawnNext();
            yield break;
        }

        getReadyText.SetActive(true);

        TextMeshProUGUI txt = getReadyText.GetComponent<TextMeshProUGUI>();

        txt.text = "3";
        yield return new WaitForSeconds(1f);

        txt.text = "2";
        yield return new WaitForSeconds(1f);

        txt.text = "1";
        yield return new WaitForSeconds(1f);

        getReadyText.SetActive(false);

        StartTimer();      // 🔥 START TIMER HERE
        UpdateTimerUI();   // 🔥 UPDATE UI
        SpawnNext();
    }
}