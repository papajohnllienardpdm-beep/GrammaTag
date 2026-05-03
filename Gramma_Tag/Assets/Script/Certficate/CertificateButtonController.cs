using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
public class CertificateButtonController : MonoBehaviour
{
    [Header("MODULE SETTINGS")]
    public int moduleID;

    [Header("LOCK SYSTEM")]
    public GameObject lockPanel;
    public Button buttonCertificate;

    [Header("TEXT UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dateText;

    [Header("GENDER IMAGE")]
    public Image genderImage;
    public Sprite boySprite;
    public Sprite girlSprite;

    [Header("VIEWER PANEL")]
    public GameObject certificateViewerPanel;

    [Header("VIEWER UI")]
    public TextMeshProUGUI viewerNameText;
    public TextMeshProUGUI viewerDateText;
    public Image viewerGenderImage;

    [Header("DOWNLOAD")]
    public RectTransform certificateToCapture;

    public GameObject dlButton;
    public GameObject backButton;

    IEnumerator Start()
    {
        // wait for DB
        while (!DatabaseManager.Instance.IsDatabaseReady())
        {
            yield return null;
        }

        SetupCertificate();
    }

    void SetupCertificate()
    {
        var db = DatabaseManager.Instance;
        int userID = db.GetUserID();

        // =========================
        // 🔒 LOCK SYSTEM
        // =========================
        bool isPassed = db.IsSubtopicPassed(moduleID);

        lockPanel.SetActive(!isPassed);
        buttonCertificate.interactable = isPassed;

        // =========================
        // 👤 USER DATA (NAME + GENDER)
        // =========================
        User user = db.GetUserData();

        if (user != null)
        {
            // FULL NAME
            string fullName = user.FirstName + " " + user.LastName;

            nameText.text = fullName;
            viewerNameText.text = fullName;

            // GENDER IMAGE
            if (user.Gender == "Boy")
            {
                genderImage.sprite = boySprite;
                viewerGenderImage.sprite = boySprite;
            }
            else
            {
                genderImage.sprite = girlSprite;
                viewerGenderImage.sprite = girlSprite;
            }
        }
        else
        {
            nameText.text = "Player";
            viewerNameText.text = "Player";
        }

        // =========================
        // 📅 ACHIEVEMENT DATE
        // =========================
        Achievement achievement = db.GetAchievement(userID, moduleID);

        string formattedDate = "Not Completed";

        if (achievement != null)
        {
            System.DateTime parsedDate;

            if (System.DateTime.TryParse(achievement.dateEarned, out parsedDate))
            {
                formattedDate = parsedDate.ToString("MMMM d, yyyy");
            }
        }

        dateText.text = formattedDate;
        viewerDateText.text = formattedDate;
    }

    public void OpenCertificate()
    {
        certificateViewerPanel.SetActive(true);

        // force landscape
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void CloseCertificate()
    {
        certificateViewerPanel.SetActive(false);

        // back to portrait
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void DownloadCertificate()
    {
        StartCoroutine(CaptureAndSave());
    }

    IEnumerator CaptureAndSave()
    {
        // 🔽 hide buttons
        dlButton.SetActive(false);
        backButton.SetActive(false);

        // 🔥 WAIT para sigurado naka-landscape na
        yield return new WaitForSeconds(0.2f);
        yield return new WaitForEndOfFrame();

        // capture
        Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTex.Apply();

        Texture2D finalTex = screenTex;

        // 🔥 FIX: kung portrait pa rin, i-rotate natin
        if (screenTex.height > screenTex.width)
        {
            finalTex = RotateTexture(screenTex);
        }

        byte[] bytes = finalTex.EncodeToPNG();

#if UNITY_ANDROID && !UNITY_EDITOR
    NativeGallery.SaveImageToGallery(bytes, "GrammaTag", "certificate.png");
    Debug.Log("✅ Saved to Gallery");
#else
        string path = Path.Combine(Application.persistentDataPath, "certificate.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("Saved locally: " + path);
#endif

        // 🔼 show buttons
        dlButton.SetActive(true);
        backButton.SetActive(true);
    }

    Texture2D RotateTexture(Texture2D original)
    {
        Texture2D rotated = new Texture2D(original.height, original.width);

        for (int i = 0; i < original.width; i++)
        {
            for (int j = 0; j < original.height; j++)
            {
                rotated.SetPixel(j, original.width - i - 1, original.GetPixel(i, j));
            }
        }

        rotated.Apply();
        return rotated;
    }
}
