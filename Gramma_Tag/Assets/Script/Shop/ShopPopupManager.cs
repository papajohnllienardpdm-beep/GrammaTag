using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopPopupManager : MonoBehaviour
{
    public static ShopPopupManager Instance;

    [Header("Popup UI")]
    public GameObject popup;
    public Image itemIcon;
    public TMP_Text confirmText;

    private ShopItem currentItem;

    Coroutine currentCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (popup != null)
        {
            popup.SetActive(false);
        }
    }

    public void ShowPopup(ShopItem item)
    {
        currentItem = item;

        popup.SetActive(true);

        // 🔥 SET ICON
        itemIcon.sprite = item.icon;

        // 🔥 SET TEXT
        confirmText.text =
            "Buy " + item.amount + " " + item.itemName + "?";

        // 🔥 STOP OLD ANIMATION
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine =
            StartCoroutine(OpenPopupAnimation());
    }

    public void ConfirmBuy()
    {
        ShopSystem.Instance.BuyItem(currentItem);

        ClosePopup();
    }

    public void Cancel()
    {
        ClosePopup();
    }

    void ClosePopup()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine =
            StartCoroutine(ClosePopupAnimation());
    }

    IEnumerator OpenPopupAnimation()
    {
        RectTransform popupRect =
            popup.GetComponent<RectTransform>();

        popupRect.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float scale =
                Mathf.SmoothStep(0f, 1f, timer / duration);

            popupRect.localScale =
                new Vector3(scale, scale, scale);

            yield return null;
        }

        popupRect.localScale = Vector3.one;
    }

    IEnumerator ClosePopupAnimation()
    {
        RectTransform popupRect =
            popup.GetComponent<RectTransform>();

        float timer = 0f;
        float duration = 0.12f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.SmoothStep(0f, 1f, timer / duration);

            popupRect.localScale =
                Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        popupRect.localScale = Vector3.zero;

        popup.SetActive(false);
    }
}
