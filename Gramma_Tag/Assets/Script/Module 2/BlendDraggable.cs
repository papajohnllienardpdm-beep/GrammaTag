using UnityEngine;
using UnityEngine.EventSystems;

public class BlendDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector2 startPosition;
    private Transform originalParent;
    private bool isLocked = false;

    public BlendTutorialManager tutorialManager; // ADD THIS

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

    transform.SetParent(canvas.transform, true);
    canvasGroup.blocksRaycasts = false;

    // ✅ NEW: notify tutorial na hinawakan na
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

        // if not dropped properly, return to start
        if (transform.parent == canvas.transform)
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