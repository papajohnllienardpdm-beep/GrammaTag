using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CatchTheSoundGame : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI progressText;

    public Button chButton;
    public Button shButton;

    public Image[] progressItems; // 10 boxes

    bool isGameFinished = false;

    string[] chWords = {
        "chair","chest","chin","beach","teach","lunch","church","peach","torch","watch",
        "match","witch","speech","coach","bench","march","search","touch","branch","catch"
    };

    string[] shWords = {
        "ship","shoe","shelf","brush","fish","wash","dish","flash","crash","fresh",
        "blush","finish","push","rush","splash","trash","wish","clash","flesh","gush"
    };

    List<string> gameWords = new List<string>();

    int currentIndex = 0;
    int score = 0;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        score = 0;
        currentIndex = 0;

        gameWords.Clear();
        gameWords.AddRange(chWords);
        gameWords.AddRange(shWords);

        // shuffle
        for (int i = 0; i < gameWords.Count; i++)
        {
            string temp = gameWords[i];
            int randomIndex = Random.Range(i, gameWords.Count);
            gameWords[i] = gameWords[randomIndex];
            gameWords[randomIndex] = temp;
        }

        // take first 10
        gameWords = gameWords.GetRange(0, 10);

        ShowWord();
        UpdateUI();

        // reset all progress colors
        for (int i = 0; i < progressItems.Length; i++)
        {
            progressItems[i].color = Color.white;
        }
    }

    void ShowWord()
    {
        wordText.text = gameWords[currentIndex].ToUpper();
    }

    public void ChooseCH()
    {
        CheckAnswer("CH");
    }

    public void ChooseSH()
    {
        CheckAnswer("SH");
    }

    void CheckAnswer(string choice)
    {
        // 🛑 stop if game already finished
        if (isGameFinished)
            return;

        // 🛑 Prevent crash if index is already finished
        if (currentIndex >= gameWords.Count)
            return;

        string word = gameWords[currentIndex];

        bool correct = false;

        if (word.Contains("ch") && choice == "CH")
            correct = true;
        else if (word.Contains("sh") && choice == "SH")
            correct = true;

        if (correct)
        {
            score++;
            progressItems[currentIndex].color = Color.green; // 🟩 correct
        }
        else
        {
            progressItems[currentIndex].color = Color.red; // 🟥 wrong
        }

        currentIndex++;

        if (currentIndex >= gameWords.Count)
        {
            UpdateUI(); // ✅ update muna UI (IMPORTANT)
            EndGame();
            return;
        }

        ShowWord();
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = score + "/10";
        progressText.text = currentIndex + "/10 words answered";
    }

    void EndGame()
    {
        isGameFinished = true;

        // Save score
        PlayerPrefs.SetInt("FinalScore", score);

        // Save total questions
        PlayerPrefs.SetInt("TotalQ", 10);

        // Save current scene name (para sa Play Again)
        PlayerPrefs.SetString("LastScene", "Module3_CatchSound");

        // Optional: make sure it saves immediately
        PlayerPrefs.Save();

        // Load Result Scene
        SceneManager.LoadScene("ResultScene");
    }
}