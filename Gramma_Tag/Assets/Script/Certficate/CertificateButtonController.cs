using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
}
