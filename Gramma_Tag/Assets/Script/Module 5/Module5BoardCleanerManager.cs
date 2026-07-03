using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Module5BoardCleanerManager : MonoBehaviour
{
    [System.Serializable]
    public class BoardWord
    {
        public string word;
        public bool shouldStay; // true = tamang sagot, dapat maiwan sa board
    }

    [Header("UI")]
    public TMP_Text instructionText;
    public TMP_Text progressText;
    public Slider progressBar;

    [Header("Word Setup")]
    public RectTransform wordsHolder;
    public GameObject wordPrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private List<BoardWord> words = new List<BoardWord>();
    private int totalWrongWords;
    private int cleanedWrongWords;

    void Start()
    {
        StartPastTenseRound();
    }

    void StartPastTenseRound()
    {
        instructionText.text = "Clean the board!\nKeep only the verbs in PAST TENSE";

        words.Clear();

        // shouldStay = true means past tense, huwag buburahin
        words.Add(new BoardWord { word = "walked", shouldStay = true });
        words.Add(new BoardWord { word = "played", shouldStay = true });
        words.Add(new BoardWord { word = "jumped", shouldStay = true });
        words.Add(new BoardWord { word = "baked", shouldStay = true });
        words.Add(new BoardWord { word = "cleaned", shouldStay = true });

        // shouldStay = false means mali, dapat burahin
        words.Add(new BoardWord { word = "walk", shouldStay = false });
        words.Add(new BoardWord { word = "plays", shouldStay = false });
        words.Add(new BoardWord { word = "will jump", shouldStay = false });
        words.Add(new BoardWord { word = "is baking", shouldStay = false });
        words.Add(new BoardWord { word = "clean", shouldStay = false });

        cleanedWrongWords = 0;
        totalWrongWords = 0;

        foreach (BoardWord w in words)
        {
            if (!w.shouldStay)
                totalWrongWords++;
        }

        ClearOldWords();
        SpawnWords();
        UpdateProgressUI();
    }

    void ClearOldWords()
    {
        foreach (Transform child in wordsHolder)
        {
            Destroy(child.gameObject);
        }
    }

    void SpawnWords()
    {
        foreach (BoardWord data in words)
        {
            GameObject obj = Instantiate(wordPrefab, wordsHolder);

            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
            text.text = data.word;

            Module5WordItem wordItem = obj.GetComponent<Module5WordItem>();
            wordItem.Setup(this, data.shouldStay);

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = GetRandomPosition();
        }
    }

    Vector2 GetRandomPosition()
    {
        float x = Random.Range(-650f, 650f);
        float y = Random.Range(-230f, 230f);

        return new Vector2(x, y);
    }

    public void OnWordClicked(Module5WordItem item, bool shouldStay)
    {
        if (shouldStay)
        {
            PlaySound(wrongSFX);
            Debug.Log("Wrong! Past tense word ito, dapat maiwan.");
            return;
        }

        PlaySound(correctSFX);

        cleanedWrongWords++;
        item.gameObject.SetActive(false);

        UpdateProgressUI();

        if (cleanedWrongWords >= totalWrongWords)
        {
            Debug.Log("Round Complete!");
            instructionText.text = "Great job!\nYou cleaned the board!";
        }
    }

    void UpdateProgressUI()
    {
        progressText.text = "Progress " + cleanedWrongWords + "/" + totalWrongWords;

        if (progressBar != null)
        {
            progressBar.maxValue = totalWrongWords;
            progressBar.value = cleanedWrongWords;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}