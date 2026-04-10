using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultPopupUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI messageText;

    public void Show(string message)
    {
        panel.SetActive(true);
        messageText.text = message;
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
