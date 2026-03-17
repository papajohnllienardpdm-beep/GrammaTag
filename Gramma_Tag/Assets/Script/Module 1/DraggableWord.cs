// DraggableWord.cs
// Attach this to each WordChip button.
// Also add a CanvasGroup component to the same WordChip object.
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWord : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string wordValue;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;
    private Vector2 startPosition;
    private Transform startParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;
        transform.SetParent(rootCanvas.transform, true);
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (transform.parent == rootCanvas.transform)
        {
            transform.SetParent(startParent, true);
            rectTransform.anchoredPosition = startPosition;
        }
    }

    public void SnapToZone(Transform zone)
    {
        transform.SetParent(zone, true);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void ReturnToStart()
    {
        transform.SetParent(startParent, true);
        rectTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}