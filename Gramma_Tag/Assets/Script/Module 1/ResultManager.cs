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

        // 🔥 HIDE POPUP DEFAULT
        if (nextModulePanel != null)
        {
            nextModulePanel.SetActive(false);
        }

        // 🔥 GET CURRENT MODULE
        int currentModuleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

        // 🔥 CHECK IF PASSED
        int passed = PlayerPrefs.GetInt("Passed", 0);

        // 🔥 CHECK FIRST TIME PASS ONLY
        if (passed == 1)
        {
            string popupKey = "POPUP_SHOWN_MODULE_" + currentModuleID;

            // ❌ popup never shown before
            if (!PlayerPrefs.HasKey(popupKey))
            {
                for (int i = 0; i < popupModuleIDs.Length; i++)
                {
                    if (popupModuleIDs[i] == currentModuleID)
                    {
                        shouldShowPopup = true;

                        // 🔥 SET CUSTOM MESSAGE
                        if (popupMessages.Length > i)
                        {
                            nextModuleText.text = popupMessages[i];
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

            int currentModuleID = PlayerPrefs.GetInt("SelectedModuleID", 1);

            string popupKey = "POPUP_SHOWN_MODULE_" + currentModuleID;

            // 🔥 SAVE NA NAKITA NA YUNG POPUP
            PlayerPrefs.SetInt(popupKey, 1);
            PlayerPrefs.Save();

            // 🔥 STOP OLD ANIMATION
            if (popupCoroutine != null)
            {
                StopCoroutine(popupCoroutine);
            }

            // 🔥 PLAY OPEN ANIMATION
            popupCoroutine = StartCoroutine(OpenPopupAnimation());

            return;
        }

        // 🔥 NORMAL FLOW
        SceneManager.LoadScene("MainMenu");
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

        SceneManager.LoadScene("MainMenu");
    }
}