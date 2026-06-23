using UnityEngine;
using UnityEngine.UI;

public class ZoomableImage : MonoBehaviour
{
    public GameObject zoomPanel;
    public Image zoomImage;

    private Image originalImage;
    private static ZoomableImage currentZoomedImage;

    public bool canZoom = true;

    void Start()
    {
        originalImage = GetComponent<Image>();

        Button btn = GetComponent<Button>();
        if (btn == null)
            btn = gameObject.AddComponent<Button>();

        btn.onClick.AddListener(ToggleZoom);
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
            OpenZoom();
        }
    }

    void OpenZoom()
    {
        zoomImage.sprite = originalImage.sprite;
        zoomPanel.SetActive(true);
        currentZoomedImage = this;
    }

    public void CloseZoom()
    {
        zoomPanel.SetActive(false);
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
}