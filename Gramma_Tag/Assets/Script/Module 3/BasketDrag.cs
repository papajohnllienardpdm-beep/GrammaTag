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

    private Vector2 lastLocalPoint;

    public RectTransform bottomLimit; // 👈 CLOUDS

    public RectTransform cloudTopPoint;

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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out lastLocalPoint
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Vector2 delta = localPoint - lastLocalPoint; // 🔥 movement difference

        MoveBasket(delta);

        // update lang kung hindi na-clamp

    }



    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        hasMoved = false;

        // ❌ REMOVE THIS
        // ResetPosition();
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

    void MoveBasket(Vector2 delta)
    {
        float basketHalfWidth = rectTransform.rect.width / 2;
        float basketHalfHeight = rectTransform.rect.height / 2;

        float halfWidth = canvasRect.rect.width / 2;
        float halfHeight = canvasRect.rect.height / 2;

        float minX = -halfWidth + basketHalfWidth;
        float maxX = halfWidth - basketHalfWidth;

        Vector3[] corners = new Vector3[4];
        bottomLimit.GetWorldCorners(corners);

        float pivotOffset = rectTransform.pivot.y * rectTransform.rect.height;
        float minY = cloudTopPoint.anchoredPosition.y + pivotOffset;


        float maxY = halfHeight - basketHalfHeight;

        Vector2 newPos = rectTransform.anchoredPosition + delta;

        float clampedX = Mathf.Clamp(newPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(newPos.y, minY, maxY);

        rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);

        // ✅ anti-stuck fix
        if (clampedX > minX && clampedX < maxX)
            lastLocalPoint.x += delta.x;

        if (clampedY > minY && clampedY < maxY)
            lastLocalPoint.y += delta.y;
    }

    void Update()
    {
        // 🖱 PC: kapag hindi na naka-hold ang mouse
        if (isDragging && !Input.GetMouseButton(0))
        {
            ForceRelease();
        }

        // 📱 Mobile: kapag wala nang touch
        if (isDragging && Input.touchCount == 0)
        {
            ForceRelease();
        }
    }

    void ForceRelease()
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        ResetPosition(); // 🔥 balik sa start
    }
}