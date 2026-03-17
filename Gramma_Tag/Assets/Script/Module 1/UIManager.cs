// UIManager.cs — attach to the GameManager object
// Drag all UI references into the Inspector slots.
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Header")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI scoreText;

    [Header("Drag-Drop")]
    public GameObject dragDropPanel;
    public TextMeshProUGUI sentenceText;
    public DropZone dropZone;
    public Transform wordsContainer;
    public GameObject wordChipPrefab;

    [Header("Matching")]
    public GameObject matchingPanel;
    public TextMeshProUGUI matchInstructionText;
    public Transform matchItemsContainer;
    public MatchZone[] matchZones;
    public GameObject matchItemPrefab;

    [Header("Feedback")]
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI explanationText;
    public Button nextButton;

    Color green = new Color(0.18f, 0.72f, 0.42f);
    Color red = new Color(0.93f, 0.29f, 0.29f);

    public void UpdateProgress(int current, int total)
    {
        progressText.text = "Question " + current + " / " + total;
        scoreText.text = "Score: " +
    GameManager.Instance.GetComponent<ScoreManager>().GetScore();
    }

    public void ShowDragDrop(QuestionData q)
    {
        dragDropPanel.SetActive(true);
        matchingPanel.SetActive(false);
        sentenceText.text = q.sentenceText;
        dropZone.correctAnswer = q.correctWord;
        dropZone.ResetZone();

        foreach (Transform c in wordsContainer) Destroy(c.gameObject);

        foreach (string w in q.wordChoices)
        {
            var chip = Instantiate(wordChipPrefab, wordsContainer);
            chip.GetComponentInChildren<TextMeshProUGUI>().text = CleanText(w);
            chip.GetComponent<DraggableWord>().wordValue = w;
        }
    }

    public void ShowMatching(QuestionData q)
    {
        dragDropPanel.SetActive(false);
        matchingPanel.SetActive(true);
        matchInstructionText.text = q.matchInstruction;

        foreach (var zone in matchZones) zone.ResetZone();

        // Destroy old items — use a list to avoid modifying during iteration
        List<GameObject> toDelete = new List<GameObject>();
        foreach (Transform c in matchItemsContainer)
            toDelete.Add(c.gameObject);
        foreach (var go in toDelete)
            Destroy(go);

        // Shuffle the pairs
        var pairs = new List<MatchPair>(q.matchPairs);
        for (int i = pairs.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var t = pairs[i]; pairs[i] = pairs[j]; pairs[j] = t;
        }

        // Spawn each item
        foreach (var pair in pairs)
        {
            GameObject item = Instantiate(matchItemPrefab, matchItemsContainer, false);

            // Find TMP text — search all children including nested ones
            TextMeshProUGUI[] allTexts = item.GetComponentsInChildren<TextMeshProUGUI>();
            if (allTexts.Length > 0)
            {
                allTexts[0].text = CleanText(pair.word);
            }
            else
            {
                Debug.LogError("No TextMeshProUGUI found in MatchItem prefab!");
            }

            MatchItem matchComp = item.GetComponent<MatchItem>();
            if (matchComp != null)
                matchComp.itemLabel = pair.word;
            else
                Debug.LogError("No MatchItem script on prefab!");
        }
    }

    public void ShowFeedback(bool correct, string explanation)
    {
        // hide gameplay panels
        dragDropPanel.SetActive(false);
        matchingPanel.SetActive(false);

        // show feedback
        feedbackPanel.SetActive(true);

        feedbackText.text = correct ? "Correct!" : "Not quite!";
        feedbackText.color = correct ? green : red;
        explanationText.text = explanation;

        nextButton.gameObject.SetActive(true);
    }

    public void HideFeedback()
    {
        feedbackPanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }

    string CleanText(string input)
    {
        return input
            .Replace("\u200B", "")
            .Replace("\uFEFF", "")
            .Replace("\u00A0", " ")
            .Replace("\uFFFC", ""); // 🔥 pinaka culprit
    }
}