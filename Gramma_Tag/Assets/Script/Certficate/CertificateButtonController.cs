using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CertificateButtonController : MonoBehaviour
{
    [Header("SET IN INSPECTOR")]
    public int moduleID;

    public GameObject lockPanel;
    public Button buttonCertificate;

    IEnumerator Start()
    {
        // 🔥 wait until DB is ready
        while (!DatabaseManager.Instance.IsDatabaseReady())
        {
            yield return null;
        }

        CheckCertificateStatus();
    }

    void CheckCertificateStatus()
    {
        int userID = DatabaseManager.Instance.GetUserID();

        bool isPassed = DatabaseManager.Instance.IsSubtopicPassed(moduleID);

        if (isPassed)
        {
            // ✅ UNLOCK
            lockPanel.SetActive(false);
            buttonCertificate.interactable = true;
        }
        else
        {
            // 🔒 LOCK
            lockPanel.SetActive(true);
            buttonCertificate.interactable = false;
        }
    }
}
