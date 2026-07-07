using System.Collections;
using System.Collections.Generic;
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

    [Header("UI")]
    public TMP_Text progressText;
    public Slider progressBar;

    [Header("Words")]
    public RectTransform wordsHolder;
    public GameObject wordPrefab;

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


    [Header("Audio Optional")]
    public AudioSource audioSource;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private List<BoardQuestion> questions = new List<BoardQuestion>();
    private List<Module5WordItem> activeWords = new List<Module5WordItem>();

    private int currentQuestionIndex = 0;
    private int score = 0;
    private int cleanedWrongWords = 0;
    private int wrongWordsNeeded = 3;

    private bool questionEnded = false;

    void Start()
    {
        if (orientationManager != null)
        {
            orientationManager.SetLandscape();
        }

        if (floatingEraser != null)
            floatingEraser.gameObject.SetActive(false);

        LoadQuestions();
        ShowQuestion();
    }

    void LoadQuestions()
    {
        questions.Clear();

        AddQuestion("Keep only the PAST TENSE verb", "walked", true, "walk", false, "walks", false, "will walk", false);
        AddQuestion("Keep only the PAST TENSE verb", "played", true, "play", false, "plays", false, "will play", false);
        AddQuestion("Keep only the PAST TENSE verb", "jumped", true, "jump", false, "jumps", false, "will jump", false);
        AddQuestion("Keep only the PAST TENSE verb", "ran", true, "run", false, "runs", false, "will run", false);
        AddQuestion("Keep only the PAST TENSE verb", "baked", true, "bake", false, "bakes", false, "will bake", false);
        AddQuestion("Keep only the PAST TENSE verb", "cleaned", true, "clean", false, "cleans", false, "will clean", false);
        AddQuestion("Keep only the PAST TENSE verb", "cried", true, "cry", false, "cries", false, "will cry", false);
        AddQuestion("Keep only the PAST TENSE verb", "studied", true, "study", false, "studies", false, "will study", false);
        AddQuestion("Keep only the PAST TENSE verb", "danced", true, "dance", false, "dances", false, "will dance", false);
        AddQuestion("Keep only the PAST TENSE verb", "washed", true, "wash", false, "washes", false, "will wash", false);
    }

    void AddQuestion(string instruction, string w1, bool s1, string w2, bool s2, string w3, bool s3, string w4, bool s4)
    {
        BoardQuestion q = new BoardQuestion();
        q.instruction = instruction;
        q.choices = new WordChoice[4];

        q.choices[0] = new WordChoice { word = w1, shouldStay = s1 };
        q.choices[1] = new WordChoice { word = w2, shouldStay = s2 };
        q.choices[2] = new WordChoice { word = w3, shouldStay = s3 };
        q.choices[3] = new WordChoice { word = w4, shouldStay = s4 };

        questions.Add(q);
    }

    void ShowQuestion()
    {
        questionEnded = false;
        cleanedWrongWords = 0;

        if (eraserController != null)
            eraserController.StopHold();

        ClearOldWords();

        if (currentQuestionIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        Debug.Log("QUESTION " + (currentQuestionIndex + 1) + "/" + questions.Count);
        Debug.Log("Current Score: " + score + "/" + questions.Count);

        SpawnFourWords(questions[currentQuestionIndex]);
        UpdateProgressUI();
    }

    void SpawnFourWords(BoardQuestion q)
    {
        activeWords.Clear();

        Vector2[] positions =
        {
        new Vector2(-350f, 120f),
        new Vector2(350f, 120f),
        new Vector2(-350f, -120f),
        new Vector2(350f, -120f)
    };

        // Randomize positions
        for (int i = 0; i < positions.Length; i++)
        {
            int randomIndex = Random.Range(i, positions.Length);

            Vector2 temp = positions[i];
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }

        // Randomize choices
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

            Module5WordItem item = obj.GetComponent<Module5WordItem>();

            item.Setup(
                this,
                shuffledChoices[i].word,
                shuffledChoices[i].shouldStay
            );

            obj.GetComponent<RectTransform>().anchoredPosition = positions[i];

            activeWords.Add(item);
        }
    }

    public void OnWordCleaned(Module5WordItem item, bool shouldStay)
    {
        if (questionEnded) return;

        int questionNumber = currentQuestionIndex + 1;
        string erasedWord = item.GetWord();

        Debug.Log("ERASED WORD: " + erasedWord);
        Debug.Log("Should Stay? " + shouldStay);

        if (shouldStay)
        {
            PlaySound(wrongSFX);

            Debug.Log("QUESTION " + questionNumber + " WRONG!");
            Debug.Log("Binura mo ang TAMANG word: " + erasedWord);
            Debug.Log("Score stays: " + score + "/" + questions.Count);

            questionEnded = true;

            item.HideWord();

            if (eraserController != null)
                eraserController.StopHold();

            currentQuestionIndex++;
            ShowQuestion();
            return;
        }

        PlaySound(correctSFX);
        item.HideWord();

        cleanedWrongWords++;

        Debug.Log("QUESTION " + questionNumber + ": nabura ang maling word: " + erasedWord);
        Debug.Log("Wrong words erased: " + cleanedWrongWords + "/3");

        if (cleanedWrongWords >= wrongWordsNeeded)
        {
            score++;

            Debug.Log("QUESTION " + questionNumber + " CORRECT!");
            Debug.Log("Score is now: " + score + "/" + questions.Count);

            questionEnded = true;
            StartCoroutine(NextQuestionDelay());
        }
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

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("TotalQ", questions.Count);

        PlayerPrefs.SetString("LastScene", gameSceneName);

        PlayerPrefs.SetInt("CoinsEarned", 0);

        // Passed kapag 6 pataas
        PlayerPrefs.SetInt("Passed", score >= 6 ? 1 : 0);

        PlayerPrefs.Save();

        StartCoroutine(GoToResultScene());
    }
    IEnumerator GoToResultScene()
    {
        // Ibalik muna sa portrait
        if (orientationManager != null)
        {
            orientationManager.SetPortrait();
        }

        // Bigyan ng konting oras para mag-rotate ang screen
        yield return new WaitForSeconds(0.5f);

        // Saka lang pumunta sa Result Scene
        SceneManager.LoadScene(resultSceneName);
    }
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}