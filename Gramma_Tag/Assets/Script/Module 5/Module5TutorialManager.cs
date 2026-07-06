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
        public WordChoice[] choices = new WordChoice[4];
    }

    [Header("UI")]
    public GameObject gameplayPanel;

    public TMP_Text instructionText;
    public TMP_Text progressText;
    public Slider progressBar;
    public TMP_Text countdownText;

    [Header("Words")]
    public RectTransform wordsHolder;
    public GameObject wordPrefab;

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

    [Header("Audio")]
    public AudioSource audioSource;
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
    private int cleanedWrongWords = 0;
    private int wrongWordsNeeded = 3;

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
            "Keep only the PAST TENSE word.",
            "walked", true,
            "walk", false,
            "walks", false,
            "will walk", false
        );

        AddQuestion(
            "Keep only the PAST TENSE word.",
            "played", true,
            "play", false,
            "plays", false,
            "will play", false
        );

        AddQuestion(
            "Keep only the PAST TENSE word.",
            "jumped", true,
            "jump", false,
            "jumps", false,
            "will jump", false
        );
    }

    void LoadPresentTutorial()
    {
        AddQuestion(
            "Keep only the PRESENT TENSE word.",
            "plays", true,
            "play", false,
            "played", false,
            "will play", false
        );

        AddQuestion(
            "Keep only the PRESENT TENSE word.",
            "runs", true,
            "run", false,
            "ran", false,
            "will run", false
        );

        AddQuestion(
            "Keep only the PRESENT TENSE word.",
            "writes", true,
            "write", false,
            "wrote", false,
            "will write", false
        );
    }

    void LoadFutureTutorial()
    {
        AddQuestion(
            "Keep only the FUTURE TENSE word.",
            "will jump", true,
            "jump", false,
            "jumped", false,
            "jumps", false
        );

        AddQuestion(
            "Keep only the FUTURE TENSE word.",
            "will bake", true,
            "bake", false,
            "baked", false,
            "bakes", false
        );

        AddQuestion(
            "Keep only the FUTURE TENSE word.",
            "will clean", true,
            "clean", false,
            "cleaned", false,
            "cleans", false
        );
    }

    void LoadBeVerbTutorial()
    {
        AddQuestion(
            "Keep only the correct BE-VERB.",
            "They are", true,
            "They am", false,
            "They is", false,
            "They be", false
        );

        AddQuestion(
            "Keep only the correct BE-VERB.",
            "I am", true,
            "I is", false,
            "I are", false,
            "I be", false
        );

        AddQuestion(
            "Keep only the correct BE-VERB.",
            "She was", true,
            "She were", false,
            "She are", false,
            "She am", false
        );
    }

    void AddQuestion(
        string instruction,
        string w1, bool s1,
        string w2, bool s2,
        string w3, bool s3,
        string w4, bool s4)
    {
        TutorialQuestion q = new TutorialQuestion();

        q.instruction = instruction;

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

        cleanedWrongWords = 0;

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
                q.instruction;

        SpawnFourWords(q);

        UpdateProgressUI();
    }

    void SpawnFourWords(TutorialQuestion q)
    {
        activeWords.Clear();

        Vector2[] positions =
        {
            new Vector2(-350,120),
            new Vector2(350,120),
            new Vector2(-350,-120),
            new Vector2(350,-120)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            int rand =
                Random.Range(i, positions.Length);

            Vector2 temp = positions[i];
            positions[i] = positions[rand];
            positions[rand] = temp;
        }

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

        for (int i = 0; i < 4; i++)
        {
            Debug.Log("===== SPAWN " + i + " =====");

            Debug.Log("wordPrefab = " + wordPrefab);
            Debug.Log("wordsHolder = " + wordsHolder);

            GameObject obj = Instantiate(wordPrefab, wordsHolder);

            Debug.Log("Instantiated = " + obj);

            Module5TutorialWordItem item =
                obj.GetComponent<Module5TutorialWordItem>();

            Debug.Log("Tutorial Item = " + item);

            if (item == null)
            {
                Debug.LogError("Module5TutorialWordItem NOT FOUND on prefab!");
                return;
            }

            Debug.Log("Word = " + shuffled[i].word);

            item.Setup(
                this,
                shuffled[i].word,
                shuffled[i].shouldStay
            );

            RectTransform rect = obj.GetComponent<RectTransform>();

            Debug.Log("RectTransform = " + rect);

            rect.anchoredPosition = positions[i];

            activeWords.Add(item);
        }
    }

    public void OnWordCleaned(Module5TutorialWordItem item, bool shouldStay)
    {
        if (questionEnded)
            return;

        if (shouldStay)
        {
            PlaySound(wrongSFX);

            questionEnded = true;

            item.HideWord();

            tutorialProgress = 0;

            if (instructionText != null)
                instructionText.text =
                    "Oops! Let's start again.";

            UpdateProgressUI();

            StartCoroutine(RestartTutorial());

            return;
        }

        PlaySound(correctSFX);

        item.HideWord();

        cleanedWrongWords++;

        if (cleanedWrongWords >= wrongWordsNeeded)
        {
            questionEnded = true;

            tutorialProgress++;

            UpdateProgressUI();

            if (tutorialProgress >= requiredCorrect)
            {
                StartCoroutine(EndTutorial());
            }
            else
            {
                if (instructionText != null)
                    instructionText.text =
                        "Great! Try another one.";

                StartCoroutine(NextQuestion());
            }
        }
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

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}