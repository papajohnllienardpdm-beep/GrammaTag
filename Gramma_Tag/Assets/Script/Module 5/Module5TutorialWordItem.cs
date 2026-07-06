using TMPro;
using UnityEngine;

public class Module5TutorialWordItem : MonoBehaviour
{
    private Module5TutorialManager manager;
    private bool shouldStay;
    private bool alreadyCleaned;
    private string currentWord;

    public TMP_Text wordText;

    public void Setup(Module5TutorialManager tutorialManager, string word, bool isCorrectWord)
    {
        manager = tutorialManager;
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
        if (alreadyCleaned)
            return;

        alreadyCleaned = true;

        if (manager != null)
            manager.OnWordCleaned(this, shouldStay);
    }

    public void HideWord()
    {
        gameObject.SetActive(false);
    }
}