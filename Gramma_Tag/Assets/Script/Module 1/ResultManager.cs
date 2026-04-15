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
        int stars = PlayerPrefs.GetInt("Stars", 0);

        scoreText.text = "Score: " + score + "/" + total;

        switch (stars)
        {
            case 3:
                feedbackText.text = "Excellent!";
                break;
            case 2:
                feedbackText.text = "Good Job!";
                break;
            case 1:
                feedbackText.text = "Nice!";
                break;
            default:
                feedbackText.text = "Try Again!";
                break;
        }
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