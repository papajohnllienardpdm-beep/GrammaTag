using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager_Module3 : MonoBehaviour
{
    public static GameManager_Module3 instance;

    public TextMeshProUGUI timerText;

    [Header("Game Timer")]
    public float gameDuration = 300f;

    private float timer;
    private bool isTimerRunning = false;


    public Image basketImage;
    public Sprite chSprite;
    public Sprite shSprite;
    public TextMeshProUGUI basketLabel;

    [Header("Tutorial Mode")]
    public bool isTutorial = false;
    public int tutorialTarget = 3;

    private int tutorialScore = 0;

    public TextMeshProUGUI tutorialText;
    public GameObject getReadyText;

    public WordSpawner spawner;

    public TextMeshProUGUI progressText;
    public Image progressBar;

    int current = 0;
    int score = 0;

    // 🔥 ROUNDS
    private List<Module3Round> rounds = new List<Module3Round>();
    private int currentRoundIndex = 0;
    private int spawnIndexInRound = 0;

    private bool roundAnswered = false;
    private List<string> currentRoundWords = new List<string>();

    public bool hasActiveWord = false;
    public bool isTransitioning = false;
    private bool isSpawning = false;

    [Header("Start Delay")]
public float startDelay = 5f;

    void Start()
    {
        StartCoroutine(WaitForDB());
    }

    IEnumerator WaitForDB()
    {
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        DatabaseManager.Instance.DeductHeart();

        LoadRoundsFromDB();
        UpdateTargetLabel(); // 🔥 ADD THIS

        if (spawner == null)
            spawner = FindObjectOfType<WordSpawner>();

        

        if (isTutorial)
{
    if (timerText != null)
        timerText.gameObject.SetActive(false);

    StartCoroutine(StartWithDelay()); // 👈 delay muna
}
        else
        {
            timer = gameDuration;

            if (getReadyText != null)
                StartCoroutine(StartGameWithDelay());
            else
            {
                StartTimer();
                SpawnNext();
            }
        }

        UpdateScoreUI();
    }

    void LoadRoundsFromDB()
    {
        rounds.Clear();

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        var dbQuestions = DatabaseManager.Instance
            .GetQuestionsByModule(moduleID)
            .OrderBy(x => Random.value)
            .Take(10)
            .ToList();

        foreach (var q in dbQuestions)
        {
            rounds.Add(new Module3Round(
                q.QuestionText,
                q.ChoiceA,
                q.ChoiceB,
                q.ChoiceC,
                q.CorrectAnswer
            ));
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

    // ✅ ANSWER LOGIC
    public void Answer(bool correct)
    {
        hasActiveWord = false;

        // 🔥 TUTORIAL
        if (isTutorial)
        {
            if (!correct)
            {
                ResetTutorial();
                SpawnNext();
                return;
            }

            tutorialScore++;
            UpdateScoreUI();
            UpdateTutorialText();

            if (tutorialScore >= tutorialTarget)
            {
                SceneManager.LoadScene("Module3_GameScene");
                return;
            }

            SpawnNext();
            return;
        }

        if (roundAnswered) return;

        if (correct)
        {
            score++;
            roundAnswered = true;
            isTransitioning = true;
            StartCoroutine(NextRoundDelay());
            return;
        }

        // ❌ WRONG = IGNORE
        SpawnNext();
    }

    public void MissCorrect()
    {
        if (roundAnswered || isTransitioning) return;

        roundAnswered = true;
        isTransitioning = true;
        StartCoroutine(NextRoundDelay());
    }

    IEnumerator NextRoundDelay()
    {
        yield return new WaitForSeconds(0.5f);
        NextRound();
    }

    void NextRound()
    {
        current++;
        currentRoundIndex++;

        spawnIndexInRound = 0;
        roundAnswered = false;
        isTransitioning = false;
        hasActiveWord = false;

        UpdateTargetLabel(); // 🔥 ADD THIS

        UpdateScoreUI();
        SpawnNext();
    }

    public void SpawnNext()
    {
        if (hasActiveWord || roundAnswered || isTransitioning || isSpawning) return;

        isSpawning = true;

        if (!isTutorial && currentRoundIndex >= rounds.Count)
        {
            FinishGame();
            return;
        }

        if (!roundAnswered && spawnIndexInRound >= 3)
        {
            MissCorrect();
            isSpawning = false;
            return;
        }

        if (spawnIndexInRound == 0)
        {
            PrepareRoundWords();
        }

        if (spawnIndexInRound < currentRoundWords.Count)
        {
            string word = currentRoundWords[spawnIndexInRound];
            spawner.SpawnWord(word);

            hasActiveWord = true;
            spawnIndexInRound++;
        }

        isSpawning = false;
    }

    void PrepareRoundWords()
    {
        currentRoundWords.Clear();

        var round = rounds[currentRoundIndex];

        currentRoundWords.Add(round.correct);

        foreach (var w in round.choices)
        {
            if (w != round.correct)
                currentRoundWords.Add(w);
        }

        currentRoundWords = currentRoundWords
            .OrderBy(x => Random.value)
            .ToList();
    }

    void FinishGame()
    {
        int total = rounds.Count;
        int stars = 0;
        int passed = 0;

        if (score >= 9)
        {
            stars = 3;
            passed = 1;
        }
        else if (score >= 7)
        {
            stars = 2;
            passed = 1;
        }
        else if (score >= 6)
        {
            stars = 1;
            passed = 1;
        }

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        StartCoroutine(SaveAndExit(moduleID, total, stars, passed));
    }

    IEnumerator SaveAndExit(int moduleID, int total, int stars, int passed)
    {
        DatabaseManager.Instance.SaveProgressBetter(1, moduleID, score, passed, stars);

        int coins = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("Passed", passed);
        PlayerPrefs.SetInt("CoinsEarned", coins);

        SceneManager.LoadScene("ResultScene");

        yield return null;
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

        if (progressText != null)
            progressText.text = "Progress " + current + "/10";

        if (progressBar != null)
            progressBar.fillAmount = (float)current / 10f;
    }

    public void StartTimer() => isTimerRunning = true;
    public void StopTimer() => isTimerRunning = false;

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";
            timerText.color = timer <= 10 ? Color.red : Color.black;
        }
    }

    void TimeUp()
    {
        isTimerRunning = false;

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", current);
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        SceneManager.LoadScene("ResultScene");
    }

    public bool IsCorrectWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;

        string cleanWord = word.Trim().ToLower();
        string correct = rounds[currentRoundIndex].correct.Trim().ToLower();

        return cleanWord == correct;
    }

    void UpdateTargetLabel()
    {
        if (isTutorial) return; // ❗ wag galawin tutorial

        if (basketLabel != null && currentRoundIndex < rounds.Count)
        {
            basketLabel.text = rounds[currentRoundIndex].target;
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

    public void ResetTutorial()
    {
        tutorialScore = 0;
        UpdateScoreUI();
        UpdateTutorialText();
    }

    public void ShowAvoidFeedback()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "Good! Avoid wrong words!";
            Invoke(nameof(UpdateTutorialText), 1.2f);
        }
    }

    IEnumerator StartGameWithDelay()
    {
        getReadyText.SetActive(true);

        var txt = getReadyText.GetComponent<TextMeshProUGUI>();

        txt.text = "3";
        yield return new WaitForSeconds(1);

        txt.text = "2";
        yield return new WaitForSeconds(1);

        txt.text = "1";
        yield return new WaitForSeconds(1);

        getReadyText.SetActive(false);

        StartTimer();
        SpawnNext();
    }

    IEnumerator StartWithDelay()
{
    yield return new WaitForSeconds(startDelay);

    SpawnNext();
}
}

[System.Serializable]
public class Module3Round
{
    public string target;
    public string[] choices;
    public string correct;

    public Module3Round(string t, string a, string b, string c, string correct)
    {
        target = t;
        choices = new string[] { a, b, c };
        this.correct = correct;
    }

    
}