using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZoomableImage : MonoBehaviour
{
    public GameObject zoomPanel;
    public Image zoomImage;

    private Image originalImage;
    private static ZoomableImage currentZoomedImage;

    public bool canZoom = true;

    private Coroutine currentCoroutine;

    void Start()
    {
        originalImage = GetComponent<Image>();

        Button btn = GetComponent<Button>();
        if (btn == null)
            btn = gameObject.AddComponent<Button>();

        btn.onClick.AddListener(ToggleZoom);

        if (zoomPanel != null)
        {
            zoomPanel.SetActive(false);
            zoomPanel.transform.localScale = Vector3.zero;
        }
    }

    public void ToggleZoom()
    {
        if (!canZoom) return;

        if (currentZoomedImage == this)
        {
            CloseZoom();
        }
        else
        {
            if (currentZoomedImage != null)
                currentZoomedImage.CloseZoomInstant();

            OpenZoom();
        }
    }

    void OpenZoom()
    {
        if (zoomPanel == null || zoomImage == null || originalImage == null)
            return;

        zoomImage.sprite = originalImage.sprite;
        zoomPanel.SetActive(true);

        currentZoomedImage = this;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(OpenZoomAnimation());
    }

    public void CloseZoom()
    {
        if (zoomPanel == null)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(CloseZoomAnimation());
    }

    void CloseZoomInstant()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (zoomPanel != null)
        {
            zoomPanel.transform.localScale = Vector3.zero;
            zoomPanel.SetActive(false);
        }

        if (currentZoomedImage == this)
            currentZoomedImage = null;
    }

    IEnumerator OpenZoomAnimation()
    {
        RectTransform zoomRect = zoomPanel.GetComponent<RectTransform>();

        zoomRect.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float scale = Mathf.SmoothStep(0f, 1f, timer / duration);

            zoomRect.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        zoomRect.localScale = Vector3.one;
    }

    IEnumerator CloseZoomAnimation()
    {
        RectTransform zoomRect = zoomPanel.GetComponent<RectTransform>();

        float timer = 0f;
        float duration = 0.12f;

        Vector3 startScale = zoomRect.localScale;
        Vector3 endScale = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, timer / duration);

            zoomRect.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        zoomRect.localScale = Vector3.zero;
        zoomPanel.SetActive(false);

        if (currentZoomedImage == this)
            currentZoomedImage = null;
    }

    public void SetCanZoom(bool value)
    {
        canZoom = value;

        if (!value)
        {
            CloseZoom();
        }
    }

    void OnDisable()
    {
        CloseZoomInstant();
    }
}