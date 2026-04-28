using UnityEngine;
using UnityEngine.EventSystems;

public class BlendDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    public Transform dragLayer; // ✅ NEW

    private Vector2 startPosition;
    private Transform originalParent;
    private bool isLocked = false;

    public BlendTutorialManager tutorialManager;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        startPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // ✅ FIXED: use drag layer instead of canvas root
        if (dragLayer != null)
        {
            transform.SetParent(dragLayer, true);
            rectTransform.SetAsLastSibling();
        }

        canvasGroup.blocksRaycasts = false;

        if (tutorialManager != null)
        {
            tutorialManager.OnWordTouched();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        canvasGroup.blocksRaycasts = true;

        if (transform.parent == dragLayer)
        {
            transform.SetParent(originalParent, true);
            rectTransform.anchoredPosition = startPosition;
        }
    }

    public void SnapToZone(Transform zone)
    {
        transform.SetParent(zone, true);
        rectTransform.anchoredPosition = Vector2.zero;

        isLocked = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void ResetPosition(Transform originalParent, Vector2 originalPos)
    {
        transform.SetParent(originalParent, true);
        rectTransform.anchoredPosition = originalPos;

        isLocked = false;
        canvasGroup.blocksRaycasts = true;
    }
}