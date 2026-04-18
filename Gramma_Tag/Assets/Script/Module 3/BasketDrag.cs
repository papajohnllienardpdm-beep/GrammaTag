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

    public static bool hasMoved = false;

    private Vector2 offset;

    public RectTransform bottomLimit; // 👈 CLOUDS

    void Start()
    {
        if (gameObject.name.Contains("CH"))
            activeBasket = "CH";
        else
            activeBasket = "SH";
    }

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

        canvasGroup.blocksRaycasts = false;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameArea,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );

        offset = rectTransform.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameArea,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );

        Vector2 targetPos = localPoint + offset;

        float basketHalfWidth = rectTransform.rect.width / 2;
        float basketHalfHeight = rectTransform.rect.height / 2;

        float halfWidth = gameArea.rect.width / 2;
        float halfHeight = gameArea.rect.height / 2;

        float minX = -halfWidth + basketHalfWidth;
        float maxX = halfWidth - basketHalfWidth;

        float minY = bottomLimit.anchoredPosition.y + 20f;
        float maxY = halfHeight - basketHalfHeight; // 🔥 FIXED TOP

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

        rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        hasMoved = false; // ✅ RESET

        ResetPosition();
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