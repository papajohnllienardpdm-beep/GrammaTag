using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;

    void Start()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        int total = PlayerPrefs.GetInt("TotalQ", 10);

        scoreText.text = "Score: " + score + "/" + total;

        float percent = (float)score / total;

        if (percent >= 0.8f)
            feedbackText.text = "Excellent! (3 Stars)";
        else if (percent >= 0.5f)
            feedbackText.text = "Good Job! (2 Stars)";
        else
            feedbackText.text = "Keep Trying! (1 Star)";
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Module1_GameScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}