using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI coinsText;

    public Image starImage; // 🔥 ImageStar

    public Sprite[] starSprites; // 🔥 array ng images (0–3 stars)
    public string[] feedbackMessages; // 🔥 array ng messages

    void Start()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        int total = PlayerPrefs.GetInt("TotalQ", 10);
        int coins = PlayerPrefs.GetInt("CoinsEarned", 0);
        coinsText.text = "+" + coins + " Coins";


        scoreText.text = "Score: " + score + "/" + total;

        // 🔥 COMPUTE STARS
        int stars = GetStars(score);

        // 🔥 SET IMAGE
        if (starSprites != null && starSprites.Length > stars)
        {
            starImage.sprite = starSprites[stars];
        }

        // 🔥 SET FEEDBACK TEXT
        if (feedbackMessages != null && feedbackMessages.Length > stars)
        {
            feedbackText.text = feedbackMessages[stars];
        }
    }

    int GetStars(int score)
    {
        if (score >= 9) return 3;
        if (score >= 7) return 2;
        if (score >= 6) return 1;
        return 0;
    }

    public void PlayAgain()
    {
        string scene = PlayerPrefs.GetString("LastScene");
        SceneManager.LoadScene(scene);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}