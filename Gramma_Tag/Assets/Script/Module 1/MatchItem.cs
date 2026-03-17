// MatchItem.cs
// Attach to each word chip in the LeftColumn of the Matching panel.
// Also add a CanvasGroup component to the same object.
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatchItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string itemLabel;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;
    private Vector2 startPosition;
    private Transform startParent;
    private LayoutElement layoutElement;
    private bool dropped = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
        layoutElement = GetComponent<LayoutElement>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsLocked()) return;

        dropped = false;

        startPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;

        if (layoutElement != null)
            layoutElement.ignoreLayout = true;

        transform.SetParent(rootCanvas.transform, true);
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!dropped && layoutElement != null)
            layoutElement.ignoreLayout = false;

        // If item was NOT dropped on a zone
        if (!dropped)
        {
            transform.SetParent(startParent, false);
            rectTransform.anchoredPosition = startPosition;
        }
    }

    public void SnapToZone(Transform zone)
    {
        dropped = true;

        if (layoutElement != null)
            layoutElement.ignoreLayout = true;

        transform.SetParent(zone, false);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void ReturnToStart()
    {
        if (layoutElement != null)
            layoutElement.ignoreLayout = false;

        transform.SetParent(startParent, false);
        rectTransform.anchoredPosition = startPosition;
    }
}