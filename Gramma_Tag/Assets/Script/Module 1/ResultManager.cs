using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI coinsText;

    public Image starImage; // 🔥 ImageStar

    public Sprite[] starSprites; // 🔥 array ng images (0–3 stars)
    public string[] feedbackMessages; // 🔥 array ng messages

    [Header("Next Module Popup")]
    public GameObject nextModulePanel;
    public TextMeshProUGUI nextModuleText;

    [Header("Popup Data")]
    public int[] popupModuleIDs;

    [TextArea]
    public string[] popupMessages;

    private bool shouldShowPopup = false;

    [Header("Popup Animation")]
    public float popupDuration = 0.15f;

    private Coroutine popupCoroutine;

    [Header("Ending Scene")]
    public int endingModuleID = 20;

    public string endingSceneName = "EndingScene";

    [Header("Ending Settings")]
    public string endingShownKey = "ENDING_SCENE_SHOWN";

    void Start()
    {
        StartCoroutine(InitializeResult());
    }


    IEnumerator InitializeResult()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        yield return null;

        int score = PlayerPrefs.GetInt("FinalScore", 0);
        int total = PlayerPrefs.GetInt("TotalQ", 10);
        int coins = PlayerPrefs.GetInt("CoinsEarned", 0);

        coinsText.text = "+" + coins + " Coins";

        scoreText.text = "Score: " + score + "/" + total;

        int stars = GetStars(score);

        if (starSprites != null && starSprites.Length > stars)
        {
            starImage.sprite = starSprites[stars];
        }

        if (feedbackMessages != null && feedbackMessages.Length > stars)
        {
            feedbackText.text = feedbackMessages[stars];
        }

        if (nextModulePanel != null)
        {
            nextModulePanel.SetActive(false);
        }

        int currentModuleID =
            PlayerPrefs.GetInt("SelectedModuleID", 1);

        int passed =
            PlayerPrefs.GetInt("Passed", 0);

        if (passed == 1)
        {
            string popupKey =
                "POPUP_SHOWN_MODULE_" + currentModuleID;

            if (!PlayerPrefs.HasKey(popupKey))
            {
                for (int i = 0; i < popupModuleIDs.Length; i++)
                {
                    if (popupModuleIDs[i] == currentModuleID)
                    {
                        shouldShowPopup = true;

                        if (popupMessages.Length > i)
                        {
                            nextModuleText.text =
                                popupMessages[i];
                        }

                        break;
                    }
                }
            }
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
        // 🔥 SHOW POPUP FIRST
        if (shouldShowPopup)
        {
            nextModulePanel.SetActive(true);

            int currentModuleID =
                PlayerPrefs.GetInt("SelectedModuleID", 1);

            string popupKey =
                "POPUP_SHOWN_MODULE_" + currentModuleID;

            PlayerPrefs.SetInt(popupKey, 1);
            PlayerPrefs.Save();

            if (popupCoroutine != null)
            {
                StopCoroutine(popupCoroutine);
            }

            popupCoroutine =
                StartCoroutine(OpenPopupAnimation());

            return;
        }

        GoToNextScene();
    }



    public void CloseNextModulePopup()
    {
        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
        }

        popupCoroutine = StartCoroutine(ClosePopupAnimation());
    }

    IEnumerator OpenPopupAnimation()
    {
        RectTransform popupRect =
            nextModulePanel.GetComponent<RectTransform>();

        popupRect.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < popupDuration)
        {
            timer += Time.deltaTime;

            float scale =
                Mathf.SmoothStep(0f, 1f, timer / popupDuration);

            popupRect.localScale =
                new Vector3(scale, scale, scale);

            yield return null;
        }

        popupRect.localScale = Vector3.one;
    }

    IEnumerator ClosePopupAnimation()
    {
        RectTransform popupRect =
            nextModulePanel.GetComponent<RectTransform>();

        float timer = 0f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < popupDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.SmoothStep(0f, 1f, timer / popupDuration);

            popupRect.localScale =
                Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        popupRect.localScale = Vector3.zero;

        nextModulePanel.SetActive(false);

        GoToNextScene();
    }

    void GoToNextScene()
    {
        if (DatabaseManager.Instance != null &&
            DatabaseManager.Instance.IsDatabaseReady())
        {
            int userID =
                DatabaseManager.Instance.GetUserID();

            bool hasCompletedEndingModule =
                DatabaseManager.Instance.HasPassedModule(
                    userID,
                    endingModuleID);

            // ✅ Show ending only once
            bool endingAlreadyShown =
                PlayerPrefs.GetInt(endingShownKey, 0) == 1;

            if (hasCompletedEndingModule &&
                !endingAlreadyShown)
            {
                PlayerPrefs.SetInt(endingShownKey, 1);
                PlayerPrefs.Save();

                SceneManager.LoadScene(endingSceneName);
                return;
            }
        }

        SceneManager.LoadScene("MainMenu");
    }
}