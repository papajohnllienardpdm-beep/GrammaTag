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
            nameText.text = user.FirstName + " " + user.LastName;

            // GENDER IMAGE
            if (user.Gender == "Boy")
            {
                genderImage.sprite = boySprite;
            }
            else
            {
                genderImage.sprite = girlSprite;
            }
        }
        else
        {
            nameText.text = "Player";
        }

        // =========================
        // 📅 ACHIEVEMENT DATE
        // =========================
        Achievement achievement = db.GetAchievement(userID, moduleID);

        if (achievement != null)
        {
            System.DateTime parsedDate;

            if (System.DateTime.TryParse(achievement.dateEarned, out parsedDate))
            {
                dateText.text = parsedDate.ToString("MMMM dd, yyyy");
            }
            else
            {
                dateText.text = "Invalid Date";
            }
        }
        else
        {
            dateText.text = "Not Completed";
        }
    }
}
