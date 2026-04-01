using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CertificatePanelManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject certificatePanel;

    public GameObject easyCertificate;
    public GameObject mediumCertificate;
    public GameObject hardCertificate;

    public void ShowEasyCertificate()
    {
        mainMenu.SetActive(false);
        certificatePanel.SetActive(true);

        easyCertificate.SetActive(true);
        mediumCertificate.SetActive(false);
        hardCertificate.SetActive(false);
    }

    public void ShowMediumCertificate()
    {
        mainMenu.SetActive(false);
        certificatePanel.SetActive(true);

        easyCertificate.SetActive(false);
        mediumCertificate.SetActive(true);
        hardCertificate.SetActive(false);
    }

    public void ShowHardCertificate()
    {
        mainMenu.SetActive(false);
        certificatePanel.SetActive(true);

        easyCertificate.SetActive(false);
        mediumCertificate.SetActive(false);
        hardCertificate.SetActive(true);
    }

    public void BackToMenu()
    {
        mainMenu.SetActive(true);
        certificatePanel.SetActive(false);
    }
}
