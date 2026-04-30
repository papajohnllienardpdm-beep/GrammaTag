using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CertificateUIManager : MonoBehaviour
{
    [System.Serializable]
    public class CertificateSlot
    {
        public int moduleID;

        public TextMeshProUGUI moduleNameText;
        public TextMeshProUGUI scoreText;
        public Image starImage;
        public GameObject imageLock; // 🔥 ADD THIS
    }

    public CertificateSlot[] slots;

    [Header("Star Sprites")]
    public Sprite zeroStar;
    public Sprite oneStar;
    public Sprite twoStar;
    public Sprite threeStar;

    IEnumerator Start()
    {
        // wait database
        yield return new WaitUntil(() => DatabaseManager.Instance != null);
        yield return new WaitUntil(() => DatabaseManager.Instance.IsDatabaseReady());

        LoadCertificates();
    }

    void LoadCertificates()
    {
        int userID = DatabaseManager.Instance.GetUserID();

        List<CertificateData> dataList = DatabaseManager.Instance.GetCertificateData(userID);

        foreach (var slot in slots)
        {
            var data = dataList.Find(x => x.ModuleID == slot.moduleID);

            if (data == null)
            {
                Debug.LogWarning("No data for ModuleID: " + slot.moduleID);

                if (slot.imageLock != null)
                    slot.imageLock.SetActive(true);

                continue;
            }

            // 🔥 MODULE NAME
            if (slot.moduleNameText != null)
                slot.moduleNameText.text = data.ModuleName;

            // 🔥 SCORE
            if (slot.scoreText != null)
                slot.scoreText.text = "Score:" + data.Score + "/10";

            // 🔥 STAR IMAGE
            if (slot.starImage != null)
                slot.starImage.sprite = GetStarSprite(data.Score);

            // 🔒 LOCK SYSTEM
            if (slot.imageLock != null)
            {
                if (data.Stars > 0)
                    slot.imageLock.SetActive(false);
                else
                    slot.imageLock.SetActive(true);
            }
        }
    }

    Sprite GetStarSprite(int score)
    {
        if (score >= 9)
            return threeStar;
        else if (score >= 7)
            return twoStar;
        else if (score >= 6)
            return oneStar;
        else
            return zeroStar;
    }
}
