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
    public Slider progressBar;

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    int current = 0; // total catches (correct + wrong)
    int score = 0;   // correct answers

    int wrong = 0;   // ❗ ADD
    int totalTarget = 10; // ❗ ADD

    // 🔥 ROUNDS
    private List<Module3Round> rounds = new List<Module3Round>();
    private int currentRoundIndex = 0;
    private int spawnIndexInRound = 0;


    private List<Module3WordSpawnData> currentRoundWords = new List<Module3WordSpawnData>();

    public bool hasActiveWord = false;
   
    private bool isSpawning = false;

    [Header("Start Delay")]
    public float startDelay = 5f;

    void Start()
    {
        StartCoroutine(WaitForDB());
    }

    IEnumerator WaitForDB()
    {
        // wait DB
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        // wait HeartSystem
        while (HeartSystem.Instance == null)
            yield return null;

        // 🔥 NEW GAME SESSION ONLY SA ACTUAL GAME
        // Tutorial = no heart deduction
        if (!isTutorial)
        {
            PlayerPrefs.SetInt("HEART_USED_THIS_SESSION", 0);
            PlayerPrefs.Save();

            if (HeartSystem.Instance != null)
            {
                HeartSystem.Instance.ResetSession();
            }
        }

        LoadRoundsFromDB();
        UpdateTargetLabel();

        // ✅ SLIDER SETUP
        if (isTutorial)
        {
            progressBar.maxValue = tutorialTarget;
            progressBar.value = 0;
        }
        else
        {
            progressBar.maxValue = totalTarget;
            progressBar.value = 0;
        }

        if (spawner == null)
            spawner = FindObjectOfType<WordSpawner>();

        if (isTutorial)
        {
            if (timerText != null)
                timerText.gameObject.SetActive(false);

            StartCoroutine(StartWithDelay());
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
                q.QuizID,
                q.ModuleID,
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
                AudioManager.Instance?.PlaySFX(wrongSFX);
                ResetTutorial();
                SpawnNext();
                return;
            }

            tutorialScore++;

            AudioManager.Instance?.PlaySFX(correctSFX);

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

        // ❗ DO NOT TOUCH tutorial above this

        // ✅ NEW LOGIC
        if (correct)
        {
            score++;
        }
        else
        {
            wrong++;
        }

        // 🔊 SFX
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(correct ? correctSFX : wrongSFX);
        }

        // ✅ ADD TOTAL PROGRESS
        current++;

        UpdateScoreUI();

        // ✅ END GAME CHECK
        if (current >= totalTarget)
        {
            FinishGame();
            return;
        }

        // ❗ CONTINUE SPAWNING
        hasActiveWord = false;
        SpawnNext();
    }





    public void SpawnNext()
    {
        if (hasActiveWord || isSpawning) return;

        isSpawning = true;

        if (current >= totalTarget)
        {
            FinishGame();
            isSpawning = false;
            return;
        }

        if (currentRoundIndex >= rounds.Count)
        {
            currentRoundIndex = 0;
        }

        if (spawnIndexInRound == 0)
        {
            PrepareRoundWords();
        }

        if (spawnIndexInRound < currentRoundWords.Count)
        {
            Module3WordSpawnData spawnData = currentRoundWords[spawnIndexInRound];

            spawner.SpawnWord(
                spawnData.word,
                spawnData.moduleID,
                spawnData.quizID,
                spawnData.choiceKey
            );

            hasActiveWord = true;
            spawnIndexInRound++;
        }
        else
        {
            currentRoundIndex++;
            spawnIndexInRound = 0;

            UpdateTargetLabel();

            isSpawning = false;
            SpawnNext();
            return;
        }

        isSpawning = false;
    }

    void PrepareRoundWords()
    {
        currentRoundWords.Clear();

        var round = rounds[currentRoundIndex];

        foreach (var choice in round.choices)
        {
            currentRoundWords.Add(new Module3WordSpawnData(
                choice.word,
                round.moduleID,
                round.quizID,
                choice.choiceKey
            ));
        }

        currentRoundWords = currentRoundWords
            .OrderBy(x => Random.value)
            .ToList();
    }

    void FinishGame()
    {
        // 🔥 BAWAS HEART ONLY SA ACTUAL GAME
        if (!isTutorial && HeartSystem.Instance != null)
        {
            HeartSystem.Instance.UseHeartSafe(1);
        }

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

        if (progressBar != null)
            progressBar.value = progressBar.maxValue;

        if (progressText != null)
            progressText.text = progressBar.maxValue + " / " + progressBar.maxValue;

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        StartCoroutine(SaveAndExit(moduleID, total, stars, passed));
    }

    IEnumerator SaveAndExit(int moduleID, int total, int stars, int passed)
    {
        

        int coins = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

        DatabaseManager.Instance.SaveProgressBetter(1, moduleID, score, passed, stars);

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
                progressBar.value = tutorialScore;

            return;
        }

        if (progressText != null)
            progressText.text = current + " / " + totalTarget;

        if (progressBar != null)
            progressBar.value = current;
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
        Debug.Log("⏰ TIME UP");

        isTimerRunning = false;

        // 🔥 BAWAS HEART ONLY SA ACTUAL GAME TIME UP
        if (!isTutorial && HeartSystem.Instance != null)
        {
            HeartSystem.Instance.UseHeartSafe(1);
        }

        // 🔥 COMPUTE RESULT USING CURRENT SCORE
        int total = totalTarget;

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
        else
        {
            stars = 0;
            passed = 0;
        }

        // 🔥 FORCE FULL PROGRESS BAR
        if (progressBar != null)
            progressBar.value = progressBar.maxValue;

        if (progressText != null)
            progressText.text =
                progressBar.maxValue + " / " + progressBar.maxValue;

        int moduleID =
            PlayerPrefs.GetInt("SelectedModuleID", 1);

        // 🔥 SAME FLOW AS NORMAL FINISH
        StartCoroutine(
            SaveAndExit(
                moduleID,
                total,
                stars,
                passed
            )
        );
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
        if (isTutorial) return;

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

        txt.text = "Let's Begin!";
        yield return new WaitForSeconds(1);

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
    public int quizID;
    public int moduleID;

    public string target;
    public Module3ChoiceData[] choices;
    public string correct;

    public Module3Round(int quizID, int moduleID, string t, string a, string b, string c, string correct)
    {
        this.quizID = quizID;
        this.moduleID = moduleID;

        target = t;
        this.correct = correct;

        choices = new Module3ChoiceData[]
        {
            new Module3ChoiceData("ChoiceA", a),
            new Module3ChoiceData("ChoiceB", b),
            new Module3ChoiceData("ChoiceC", c)
        };
    }
}

[System.Serializable]
public class Module3ChoiceData
{
    public string choiceKey;
    public string word;

    public Module3ChoiceData(string choiceKey, string word)
    {
        this.choiceKey = choiceKey;
        this.word = word;
    }
}

[System.Serializable]
public class Module3WordSpawnData
{
    public string word;
    public int moduleID;
    public int quizID;
    public string choiceKey;

    public Module3WordSpawnData(string word, int moduleID, int quizID, string choiceKey)
    {
        this.word = word;
        this.moduleID = moduleID;
        this.quizID = quizID;
        this.choiceKey = choiceKey;
    }
}