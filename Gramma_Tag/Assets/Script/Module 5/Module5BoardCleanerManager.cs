using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Module5BoardCleanerManager : MonoBehaviour
{
    [System.Serializable]
    public class WordChoice
    {
        public string word;
        public bool shouldStay;
    }

    [System.Serializable]
    public class BoardQuestion
    {
        public string instruction;
        public WordChoice[] choices = new WordChoice[4];
    }

    [System.Serializable]
    public class WordSlotSettings
    {
        public Vector2 anchoredPosition;
        public Vector2 size = new Vector2(250f, 100f);
    }

    [Header("UI")]
    public TMP_Text instructionText;
    public TMP_Text progressText;
    public Slider progressBar;

    [Header("Words")]
    public RectTransform wordsHolder;
    public GameObject wordPrefab;

    [Header("Word Slot Layout")]
    public WordSlotSettings[] wordSlots = new WordSlotSettings[4];

    [Header("Word Text Style")]
    public TMP_FontAsset wordFontAsset;
    public float wordFontSize = 55f;
    public Color wordFontColor = Color.white;

    [Header("Eraser")]
    public RectTransform floatingEraser;
    public Module5EraserController eraserController;

    [Range(0.1f, 1f)]
    public float eraserHitboxScale = 0.45f;

    [Range(0.1f, 1f)]
    public float wordHitboxScale = 0.75f;

    [Header("Orientation")]
    public OrientationManager orientationManager;

    [Header("Result Scene")]
    public string resultSceneName = "ResultScene";
    public string gameSceneName = "Module5_GameScene";
    public string moduleName = "Module 5";

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private List<BoardQuestion> questions = new List<BoardQuestion>();
    private List<Module5WordItem> activeWords = new List<Module5WordItem>();

    private int currentQuestionIndex = 0;
    private int score = 0;
    private int cleanedWordsCount = 0;
    private int wordsToCleanBeforeCheck = 3;

    private bool questionEnded = false;

    void Start()
    {
        StartCoroutine(WaitForSystemsThenStart());
    }

    IEnumerator WaitForSystemsThenStart()
    {
        if (orientationManager != null)
        {
            orientationManager.SetLandscape();
        }

        if (floatingEraser != null)
            floatingEraser.gameObject.SetActive(false);

        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        while (HeartSystem.Instance == null)
            yield return null;

        PlayerPrefs.SetInt("HEART_USED_THIS_SESSION", 0);
        PlayerPrefs.Save();

        HeartSystem.Instance.ResetSession();

        LoadQuestionsFromDB();

        if (progressBar != null)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = questions.Count;
            progressBar.value = 0;
        }

        ShowQuestion();
    }

    void LoadQuestionsFromDB()
    {
        questions.Clear();

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        Debug.Log("MODULE 5 SELECTED MODULE ID: " + moduleID);

        var dbQuestions = DatabaseManager.Instance
            .GetQuestionsByModule(moduleID)
            .OrderBy(x => Random.value)
            .Take(10)
            .ToList();

        foreach (var dbQ in dbQuestions)
        {
            BoardQuestion q = new BoardQuestion();
            q.instruction = dbQ.QuestionText;
            q.choices = new WordChoice[4];

            q.choices[0] = new WordChoice
            {
                word = dbQ.ChoiceA,
                shouldStay = IsSameAnswer(dbQ.ChoiceA, dbQ.CorrectAnswer)
            };

            q.choices[1] = new WordChoice
            {
                word = dbQ.ChoiceB,
                shouldStay = IsSameAnswer(dbQ.ChoiceB, dbQ.CorrectAnswer)
            };

            q.choices[2] = new WordChoice
            {
                word = dbQ.ChoiceC,
                shouldStay = IsSameAnswer(dbQ.ChoiceC, dbQ.CorrectAnswer)
            };

            q.choices[3] = new WordChoice
            {
                word = dbQ.ChoiceD,
                shouldStay = IsSameAnswer(dbQ.ChoiceD, dbQ.CorrectAnswer)
            };

            questions.Add(q);
        }

        Debug.Log("MODULE 5 QUESTIONS LOADED: " + questions.Count);
    }

    bool IsSameAnswer(string choice, string correctAnswer)
    {
        if (string.IsNullOrWhiteSpace(choice)) return false;
        if (string.IsNullOrWhiteSpace(correctAnswer)) return false;

        return choice.Trim().ToLower() == correctAnswer.Trim().ToLower();
    }

    void ShowQuestion()
    {
        questionEnded = false;
        cleanedWordsCount = 0;

        if (eraserController != null)
            eraserController.StopHold();

        ClearOldWords();

        if (currentQuestionIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        if (instructionText != null)
            instructionText.text = questions[currentQuestionIndex].instruction;

        Debug.Log("QUESTION " + (currentQuestionIndex + 1) + "/" + questions.Count);
        Debug.Log("Current Score: " + score + "/" + questions.Count);

        SpawnFourWords(questions[currentQuestionIndex]);
        UpdateProgressUI();
    }

    void SpawnFourWords(BoardQuestion q)
    {
        activeWords.Clear();

        // Default fallback kung hindi pa na-set sa Inspector
        Vector2[] fallbackPositions =
        {
        new Vector2(-250f, 90f),
        new Vector2(250f, 90f),
        new Vector2(-250f, -90f),
        new Vector2(250f, -90f)
    };

        List<WordChoice> shuffledChoices = new List<WordChoice>(q.choices);

        for (int i = 0; i < shuffledChoices.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledChoices.Count);

            WordChoice temp = shuffledChoices[i];
            shuffledChoices[i] = shuffledChoices[randomIndex];
            shuffledChoices[randomIndex] = temp;
        }

        for (int i = 0; i < shuffledChoices.Count; i++)
        {
            GameObject obj = Instantiate(wordPrefab, wordsHolder);

            RectTransform rect = obj.GetComponent<RectTransform>();

            if (rect != null)
            {
                if (wordSlots != null && wordSlots.Length > i)
                {
                    rect.anchoredPosition = wordSlots[i].anchoredPosition;
                    rect.sizeDelta = wordSlots[i].size;
                }
                else
                {
                    rect.anchoredPosition = fallbackPositions[i];
                    rect.sizeDelta = new Vector2(250f, 100f);
                }
            }

            Module5WordItem item = obj.GetComponent<Module5WordItem>();

            item.Setup(
                this,
                shuffledChoices[i].word,
                shuffledChoices[i].shouldStay
            );

            ApplyWordTextStyle(item);

            activeWords.Add(item);
        }
    }

    void ApplyWordTextStyle(Module5WordItem item)
    {
        if (item == null) return;

        TMP_Text txt = item.wordText;

        if (txt == null)
            txt = item.GetComponentInChildren<TMP_Text>();

        if (txt == null) return;

        if (wordFontAsset != null)
            txt.font = wordFontAsset;

        txt.fontSize = wordFontSize;
        txt.color = wordFontColor;

        txt.enableAutoSizing = false;
        txt.alignment = TextAlignmentOptions.Center;
    }

    public void OnWordCleaned(Module5WordItem item, bool shouldStay)
    {
        if (questionEnded) return;

        int questionNumber = currentQuestionIndex + 1;
        string erasedWord = item.GetWord();

        Debug.Log("ERASED WORD: " + erasedWord);
        Debug.Log("Should Stay? " + shouldStay);

        item.HideWord();

        cleanedWordsCount++;

        Debug.Log("QUESTION " + questionNumber + ": erased word: " + erasedWord);
        Debug.Log("Words erased: " + cleanedWordsCount + "/" + wordsToCleanBeforeCheck);

        // Walang sound habang nagbubura pa.
        // Mag-sound lang kapag may isang word na lang at iche-check na.
        if (cleanedWordsCount < wordsToCleanBeforeCheck)
        {
            return;
        }

        questionEnded = true;

        if (eraserController != null)
            eraserController.StopHold();

        Module5WordItem remainingWord = GetRemainingActiveWord();

        if (remainingWord == null)
        {
            Debug.LogWarning("No remaining word found.");

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            StartCoroutine(NextQuestionDelay());
            return;
        }

        string remainingText = remainingWord.GetWord();
        bool isCorrectRemaining = remainingWord.ShouldStay();

        Debug.Log("REMAINING WORD: " + remainingText);
        Debug.Log("Is Correct Remaining? " + isCorrectRemaining);

        if (isCorrectRemaining)
        {
            score++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);

            Debug.Log("QUESTION " + questionNumber + " CORRECT!");
            Debug.Log("Score is now: " + score + "/" + questions.Count);
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            Debug.Log("QUESTION " + questionNumber + " WRONG!");
            Debug.Log("Remaining word is not the correct answer: " + remainingText);
            Debug.Log("Score stays: " + score + "/" + questions.Count);
        }

        StartCoroutine(NextQuestionDelay());
    }

    Module5WordItem GetRemainingActiveWord()
    {
        foreach (Module5WordItem item in activeWords)
        {
            if (item == null) continue;

            if (item.gameObject.activeSelf)
            {
                return item;
            }
        }

        return null;
    }

    IEnumerator NextQuestionDelay()
    {
        if (eraserController != null)
            eraserController.StopHold();

        yield return new WaitForSeconds(0.4f);

        currentQuestionIndex++;
        ShowQuestion();
    }

    public void CheckEraserCollision(Vector2 eraserPosition, float radius)
    {
        if (questionEnded) return;
        if (floatingEraser == null) return;

        Module5WordItem closestItem = null;
        float closestDistance = float.MaxValue;

        foreach (Module5WordItem item in activeWords)
        {
            if (item == null) continue;
            if (!item.gameObject.activeSelf) continue;

            RectTransform wordRect = item.GetComponent<RectTransform>();
            if (wordRect == null) continue;

            if (RectOverlaps(floatingEraser, wordRect))
            {
                float distance = Vector2.Distance(
                    floatingEraser.position,
                    wordRect.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }
        }

        if (closestItem != null)
        {
            closestItem.TryClean();
        }
    }

    bool RectOverlaps(RectTransform rectA, RectTransform rectB)
    {
        Rect a = GetShrunkWorldRect(rectA, eraserHitboxScale);
        Rect b = GetShrunkWorldRect(rectB, wordHitboxScale);

        return a.Overlaps(b);
    }

    Rect GetShrunkWorldRect(RectTransform rectTransform, float scale)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;

        float width = xMax - xMin;
        float height = yMax - yMin;

        float newWidth = width * scale;
        float newHeight = height * scale;

        float centerX = xMin + width / 2f;
        float centerY = yMin + height / 2f;

        return new Rect(
            centerX - newWidth / 2f,
            centerY - newHeight / 2f,
            newWidth,
            newHeight
        );
    }

    void ClearOldWords()
    {
        activeWords.Clear();

        foreach (Transform child in wordsHolder)
            Destroy(child.gameObject);
    }

    void UpdateProgressUI()
    {
        int display = currentQuestionIndex + 1;
        if (display > questions.Count) display = questions.Count;

        if (progressText != null)
            progressText.text = "Progress " + display + "/" + questions.Count;

        if (progressBar != null)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = questions.Count;
            progressBar.value = currentQuestionIndex;
        }
    }

    void EndGame()
    {
        ClearOldWords();

        if (eraserController != null)
            eraserController.StopHold();

        if (progressText != null)
            progressText.text = "Progress " + questions.Count + "/" + questions.Count;

        if (progressBar != null)
            progressBar.value = questions.Count;

        Debug.Log("GAME FINISHED!");
        Debug.Log("FINAL SCORE: " + score + "/" + questions.Count);

        if (HeartSystem.Instance != null)
        {
            HeartSystem.Instance.UseHeartSafe(1);
        }

        int total = questions.Count;

        int stars = 0;
        int passed = 0;

        if (score >= 9) { stars = 3; passed = 1; }
        else if (score >= 7) { stars = 2; passed = 1; }
        else if (score >= 6) { stars = 1; passed = 1; }

        int moduleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        StartCoroutine(SaveAndGoToResult(moduleID, total, stars, passed));
    }

    IEnumerator SaveAndGoToResult(int moduleID, int total, int stars, int passed)
    {
        int userID = DatabaseManager.Instance.GetUserID();

        int coinsEarned = DatabaseManager.Instance.GiveCoins(moduleID, score, passed);

        DatabaseManager.Instance.SaveProgressBetter(userID, moduleID, score, passed, stars);

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", total);
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("Passed", passed);
        PlayerPrefs.SetInt("CoinsEarned", coinsEarned);
        PlayerPrefs.SetString("LastScene", gameSceneName);
        PlayerPrefs.Save();

        if (orientationManager != null)
        {
            orientationManager.SetPortrait();
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(resultSceneName);
    }

    
}