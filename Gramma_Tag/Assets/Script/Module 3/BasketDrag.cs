using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BasketDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private CanvasGroup canvasGroup;
    private Vector2 startPos;
    private bool isDragging = false;

    public RectTransform gameArea;
    public RectTransform topLimit; // 👈 progress bar or text
    public static string activeBasket = "";


    private Vector2 offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.transform as RectTransform;

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        startPos = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        activeBasket = gameObject.name.Contains("CH") ? "CH" : "SH";

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameArea,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );

        offset = rectTransform.anchoredPosition - localPoint;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || gameArea == null) return;

        if (!isDragging) return;

        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameArea,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            Vector2 targetPos = localPoint + offset;

            float halfWidth = gameArea.rect.width / 2;
            float halfHeight = gameArea.rect.height / 2;

            float basketHalfWidth = rectTransform.rect.width / 2;
            float basketHalfHeight = rectTransform.rect.height / 2;

            float clampedX = Mathf.Clamp(
                targetPos.x,
                -halfWidth + basketHalfWidth,
                halfWidth - basketHalfWidth
            );

            float minY = -halfHeight + basketHalfHeight;

            float clampedY = targetPos.y;

            // 👉 kung may limit, saka lang i-clamp
            if (topLimit != null)
            {
                Vector2 limitPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gameArea,
                    RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, topLimit.position),
                    canvas.worldCamera,
                    out limitPos
                );

                float maxY = limitPos.y - basketHalfHeight + 50f;
                clampedY = Mathf.Clamp(targetPos.y, minY, maxY);
            }
            else
            {
                clampedY = Mathf.Clamp(targetPos.y, minY, halfHeight);
            }

            rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        activeBasket = "";

        ResetPosition(); // dito lang dapat
    }

    public void ResetPosition()
    {
        isDragging = false;
        StopAllCoroutines();
        StartCoroutine(SmoothReset());
    }

    IEnumerator SmoothReset()
    {
        Vector2 start = rectTransform.anchoredPosition;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            rectTransform.anchoredPosition = Vector2.Lerp(start, startPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = startPos;
    }
}