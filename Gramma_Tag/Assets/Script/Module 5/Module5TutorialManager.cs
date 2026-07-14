using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Module5TutorialManager : MonoBehaviour
{
    public enum TutorialType
    {
        PastTense,
        PresentTense,
        FutureTense,
        BeVerb
    }

    [Header("Tutorial Type")]
    public TutorialType tutorialType;

    [System.Serializable]
    public class WordChoice
    {
        public string word;
        public bool shouldStay;
    }

    [System.Serializable]
    public class TutorialQuestion
    {
        public string instruction;

        [TextArea]
        public string wordToHighlight;

        public WordChoice[] choices = new WordChoice[4];
    }

    [Header("UI")]
    public GameObject gameplayPanel;

    public TMP_Text instructionText;
    public TMP_Text progressText;
    public Slider progressBar;
    public TMP_Text countdownText;

    public TMP_Text tutorialHintText;

    [Header("Tutorial Hint Messages")]
    [TextArea]
    public string holdEraserHint = "Touch and hold the eraser.";

    [TextArea]
    public string eraseWordsHint = "Erase the incorrect words.";

    [Header("Words")]
    public RectTransform wordsHolder;
    public GameObject wordPrefab;

    [Header("Word Slot Layout")]
    public WordSlotSettings[] wordSlots = new WordSlotSettings[4];

    [Header("Word Text Style")]
    public TMP_FontAsset wordFontAsset;
    public float wordFontSize = 55f;
    public Color wordFontColor = Color.white;

    [Header("Question Highlight")]
    public Color highlightColor = Color.yellow;

    [Header("Eraser")]
    public RectTransform floatingEraser;
    public Module5TutorialEraserController eraserController;

    [Range(0.1f, 1f)]
    public float eraserHitboxScale = 0.45f;

    [Range(0.1f, 1f)]
    public float wordHitboxScale = 0.75f;

    [Header("Orientation")]
    public OrientationManager orientationManager;

    [Header("Scene")]
    public string gameSceneName = "Module5_GameScene";

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private List<TutorialQuestion> questions =
        new List<TutorialQuestion>();

    private List<Module5TutorialWordItem> activeWords =
        new List<Module5TutorialWordItem>();

    private int currentQuestion = 0;

    // Tutorial Progress
    private int tutorialProgress = 0;

    // Need 3 consecutive correct
    private const int requiredCorrect = 3;

    // Need to erase 3 wrong words
    // Need to erase 3 words, then check the last remaining word
    private int cleanedWordsCount = 0;
    private int wordsToCleanBeforeCheck = 3;

    private bool questionEnded = false;

    

    void Start()
    {
        if (orientationManager != null)
            orientationManager.SetLandscape();

        if (floatingEraser != null)
            floatingEraser.gameObject.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        progressBar.minValue = 0;
        progressBar.maxValue = requiredCorrect;

        LoadTutorialQuestions();

        ShuffleQuestions();

        ShowQuestion();

        UpdateProgressUI();
    }

    void LoadTutorialQuestions()
    {
        questions.Clear();

        switch (tutorialType)
        {
            case TutorialType.PastTense:
                LoadPastTutorial();
                break;

            case TutorialType.PresentTense:
                LoadPresentTutorial();
                break;

            case TutorialType.FutureTense:
                LoadFutureTutorial();
                break;

            case TutorialType.BeVerb:
                LoadBeVerbTutorial();
                break;
        }
    }
    void LoadPastTutorial()
    {
        AddQuestion(
            "Yesterday, I walk to school. What is the past tense of walk?",
            "walk",

            "walked", true,
            "walk", false,
            "walks", false,
            "will walk", false
        );

        AddQuestion(
            "Last Sunday, we play basketball. What is the past tense of play?",
            "play",

            "played", true,
            "play", false,
            "plays", false,
            "will play", false
        );

        AddQuestion(
            "Yesterday, she jump over the box. What is the past tense of jump?",
            "jump",

            "jumped", true,
            "jump", false,
            "jumps", false,
            "will jump", false
        );
    }

    void LoadBeVerbTutorial()
    {
        AddQuestion(
    "They ___ playing outside. Which be-verb correctly completes the sentence?",
    "They",

    "are", true,
    "am", false,
    "is", false,
    "was", false
);

        AddQuestion(
    "I ___ happy today. Which be-verb correctly completes the sentence?",
    "I",

    "am", true,
    "is", false,
    "are", false,
    "were", false
);

        AddQuestion(
    "She ___ my best friend. Which be-verb correctly completes the sentence?",
    "She",

    "is", true,
    "am", false,
    "are", false,
    "were", false
);
    }

    void LoadPresentTutorial()
    {
        AddQuestion(
            "Every day, Ana walk to school. What is the correct present tense of walk?",
            "walk",

            "walks", true,
            "walk", false,
            "walked", false,
            "will walk", false
        );

        AddQuestion(
            "Every Saturday, Ben play basketball. What is the correct present tense of play?",
            "play",

            "plays", true,
            "play", false,
            "played", false,
            "will play", false
        );

        AddQuestion(
            "Every morning, Dad cook breakfast. What is the correct present tense of cook?",
            "cook",

            "cooks", true,
            "cook", false,
            "cooked", false,
            "will cook", false
        );
    }

    void LoadFutureTutorial()
    {
        AddQuestion(
            "Tomorrow, I walk to school. What is the future tense of walk?",
            "walk",

            "will walk", true,
            "walk", false,
            "walked", false,
            "walks", false
        );

        AddQuestion(
            "Next week, we clean the room. What is the future tense of clean?",
            "clean",

            "will clean", true,
            "clean", false,
            "cleaned", false,
            "cleans", false
        );

        AddQuestion(
            "Later, she jump over the rope. What is the future tense of jump?",
            "jump",

            "will jump", true,
            "jump", false,
            "jumped", false,
            "jumps", false
        );
    }

    void AddQuestion(
    string instruction,
    string highlightWord,

    string w1, bool s1,
    string w2, bool s2,
    string w3, bool s3,
    string w4, bool s4)
    {
        TutorialQuestion q =
            new TutorialQuestion();

        q.instruction = instruction;

        q.wordToHighlight = highlightWord;

        q.choices = new WordChoice[4];

        q.choices[0] =
            new WordChoice()
            {
                word = w1,
                shouldStay = s1
            };

        q.choices[1] =
            new WordChoice()
            {
                word = w2,
                shouldStay = s2
            };

        q.choices[2] =
            new WordChoice()
            {
                word = w3,
                shouldStay = s3
            };

        q.choices[3] =
            new WordChoice()
            {
                word = w4,
                shouldStay = s4
            };

        questions.Add(q);
    }

    string GetHighlightedInstruction(TutorialQuestion question)
    {
        string text = question.instruction;

        if (string.IsNullOrWhiteSpace(question.wordToHighlight))
            return text;

        int index =
            text.IndexOf(
                question.wordToHighlight,
                System.StringComparison.OrdinalIgnoreCase);

        if (index < 0)
            return text;

        string originalWord =
            text.Substring(
                index,
                question.wordToHighlight.Length);

        string color =
            ColorUtility.ToHtmlStringRGB(highlightColor);

        string coloredWord =
            "<color=#"
            + color +
            ">"
            + originalWord +
            "</color>";

        text =
            text.Remove(
                index,
                originalWord.Length);

        text =
            text.Insert(
                index,
                coloredWord);

        return text;
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            TutorialQuestion temp = questions[i];

            int random =
                Random.Range(i, questions.Count);

            questions[i] = questions[random];
            questions[random] = temp;
        }
    }

    void ShowQuestion()
    {
        questionEnded = false;

        cleanedWordsCount = 0;

        if (eraserController != null)
            eraserController.StopHold();

        ClearOldWords();

        if (currentQuestion >= questions.Count)
        {
            currentQuestion = 0;

            ShuffleQuestions();
        }

        TutorialQuestion q =
            questions[currentQuestion];

        if (instructionText != null)
            instructionText.text =
    GetHighlightedInstruction(q);

        UpdateTutorialHint(false);

        SpawnFourWords(q);

        UpdateProgressUI();
    }

    void SpawnFourWords(TutorialQuestion q)
    {
        activeWords.Clear();

        Vector2[] fallbackPositions =
        {
        new Vector2(-250f, 90f),
        new Vector2(250f, 90f),
        new Vector2(-250f, -90f),
        new Vector2(250f, -90f)
    };

        List<WordChoice> shuffled =
            new List<WordChoice>(q.choices);

        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand =
                Random.Range(i, shuffled.Count);

            WordChoice temp = shuffled[i];
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }

        for (int i = 0; i < shuffled.Count; i++)
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

            Module5TutorialWordItem item =
                obj.GetComponent<Module5TutorialWordItem>();

            if (item == null)
            {
                Debug.LogError("Module5TutorialWordItem NOT FOUND on prefab!");
                return;
            }

            item.Setup(
                this,
                shuffled[i].word,
                shuffled[i].shouldStay
            );

            ApplyWordTextStyle(item);

            activeWords.Add(item);
        }
    }

    void ApplyWordTextStyle(Module5TutorialWordItem item)
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

    public void OnWordCleaned(Module5TutorialWordItem item, bool shouldStay)
    {
        if (questionEnded)
            return;

        item.HideWord();

        cleanedWordsCount++;

        Debug.Log("Words erased: " + cleanedWordsCount + "/" + wordsToCleanBeforeCheck);

        // Walang sound habang nagbubura pa.
        // Saka lang magche-check kapag isang word na lang ang natira.
        if (cleanedWordsCount < wordsToCleanBeforeCheck)
        {
            return;
        }

        questionEnded = true;

        if (eraserController != null)
            eraserController.StopHold();

        Module5TutorialWordItem remainingWord =
            GetRemainingActiveWord();

        if (remainingWord == null)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            tutorialProgress = 0;

            if (instructionText != null)
                instructionText.text = "Oops! Let's start again.";

            UpdateProgressUI();

            StartCoroutine(RestartTutorial());
            return;
        }

        bool isCorrectRemaining =
            remainingWord.ShouldStay();

        string remainingText =
            remainingWord.GetWord();

        Debug.Log("REMAINING WORD: " + remainingText);
        Debug.Log("Is Correct Remaining? " + isCorrectRemaining);

        if (isCorrectRemaining)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);

            tutorialProgress++;

            UpdateProgressUI();

            if (tutorialProgress >= requiredCorrect)
            {
                StartCoroutine(EndTutorial());
            }
            else
            {
                if (instructionText != null)
                    instructionText.text = "Great! Try another one.";

                StartCoroutine(NextQuestion());
            }
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            tutorialProgress = 0;

            if (instructionText != null)
                instructionText.text = "Oops! Let's start again.";

            UpdateProgressUI();

            StartCoroutine(RestartTutorial());
        }
    }

    Module5TutorialWordItem GetRemainingActiveWord()
    {
        foreach (Module5TutorialWordItem item in activeWords)
        {
            if (item == null)
                continue;

            if (item.gameObject.activeSelf)
            {
                return item;
            }
        }

        return null;
    }

    IEnumerator NextQuestion()
    {
        if (eraserController != null)
            eraserController.StopHold();

        yield return new WaitForSeconds(1f);

        currentQuestion++;

        ShowQuestion();
    }

    IEnumerator RestartTutorial()
    {
        if (eraserController != null)
            eraserController.StopHold();

        yield return new WaitForSeconds(1.2f);

        currentQuestion = 0;

        ShuffleQuestions();

        ShowQuestion();
    }

    public void CheckEraserCollision(Vector2 eraserPosition, float radius)
    {
        if (questionEnded)
            return;

        if (floatingEraser == null)
            return;

        Module5TutorialWordItem closest = null;

        float closestDistance = float.MaxValue;

        foreach (Module5TutorialWordItem item in activeWords)
        {
            if (item == null)
                continue;

            if (!item.gameObject.activeSelf)
                continue;

            RectTransform wordRect =
                item.GetComponent<RectTransform>();

            if (RectOverlaps(floatingEraser, wordRect))
            {
                float d =
                    Vector2.Distance(
                        floatingEraser.position,
                        wordRect.position);

                if (d < closestDistance)
                {
                    closestDistance = d;
                    closest = item;
                }
            }
        }

        if (closest != null)
            closest.TryClean();
    }

    bool RectOverlaps(RectTransform a, RectTransform b)
    {
        Rect r1 =
            GetShrunkWorldRect(
                a,
                eraserHitboxScale);

        Rect r2 =
            GetShrunkWorldRect(
                b,
                wordHitboxScale);

        return r1.Overlaps(r2);
    }

    Rect GetShrunkWorldRect(RectTransform rect, float scale)
    {
        Vector3[] corners =
            new Vector3[4];

        rect.GetWorldCorners(corners);

        float xmin = corners[0].x;
        float ymin = corners[0].y;
        float xmax = corners[2].x;
        float ymax = corners[2].y;

        float width = xmax - xmin;
        float height = ymax - ymin;

        float newWidth = width * scale;
        float newHeight = height * scale;

        float centerX = xmin + width / 2f;
        float centerY = ymin + height / 2f;

        return new Rect(
            centerX - newWidth / 2f,
            centerY - newHeight / 2f,
            newWidth,
            newHeight);
    }

    void ClearOldWords()
    {
        activeWords.Clear();

        foreach (Transform child in wordsHolder)
            Destroy(child.gameObject);
    }

    public void UpdateTutorialHint(bool isHolding)
    {
        if (tutorialHintText == null)
            return;

        tutorialHintText.text =
            isHolding ?
            eraseWordsHint :
            holdEraserHint;
    }

    

    void UpdateProgressUI()
    {
        if (progressText != null)
        {
            progressText.text =
                "Tutorial " +
                tutorialProgress +
                " / " +
                requiredCorrect;
        }

        if (progressBar != null)
        {
            progressBar.value =
                tutorialProgress;
        }
    }

    IEnumerator EndTutorial()
    {
        if (eraserController != null)
            eraserController.StopHold();

        if (instructionText != null)
            instructionText.text =
                "Awesome!\nYou're ready to play!";

        progressBar.value = requiredCorrect;
        progressText.text = "Tutorial 3 / 3";

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ShowCountdown());

        GoToGame();

        if (tutorialHintText != null)
            tutorialHintText.text = "";
    }

    IEnumerator ShowCountdown()
    {
        if (countdownText == null)
            yield break;

        // Itago ang buong gameplay
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        // Siguraduhing wala nang eraser
        if (eraserController != null)
            eraserController.StopHold();

        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.8f);

        countdownText.gameObject.SetActive(false);
    }

    void GoToGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    [System.Serializable]
    public class WordSlotSettings
    {
        public Vector2 anchoredPosition;
        public Vector2 size = new Vector2(250f, 100f);
    }
}