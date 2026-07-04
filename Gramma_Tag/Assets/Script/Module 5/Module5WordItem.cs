using TMPro;
using UnityEngine;

public class Module5WordItem : MonoBehaviour
{
    private Module5BoardCleanerManager manager;
    private bool shouldStay;
    private bool alreadyCleaned;
    private string currentWord;

    public TMP_Text wordText;

    public void Setup(Module5BoardCleanerManager gameManager, string word, bool isCorrectWord)
    {
        manager = gameManager;
        shouldStay = isCorrectWord;
        currentWord = word;
        alreadyCleaned = false;

        if (wordText == null)
            wordText = GetComponentInChildren<TMP_Text>();

        if (wordText != null)
            wordText.text = word;

        gameObject.SetActive(true);
    }

    public string GetWord()
    {
        return currentWord;
    }

    public bool ShouldStay()
    {
        return shouldStay;
    }

    public void TryClean()
    {
        if (alreadyCleaned) return;

        alreadyCleaned = true;

        if (manager != null)
            manager.OnWordCleaned(this, shouldStay);
    }

    public void HideWord()
    {
        gameObject.SetActive(false);
    }
}