using UnityEngine;
using UnityEngine.EventSystems;

public class Module1DraggableChoice : MonoBehaviour,
IBeginDragHandler,
IDragHandler,
IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector2 startPosition;

    public string choiceKey; // A or B

    private Transform originalParent;
    private int originalSiblingIndex;

    private bool canDrag = true;

    private bool droppedOnTarget = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        startPosition = rectTransform.anchoredPosition;
    }

    public void SetCanDrag(bool value)
    {
        canDrag = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;

        Module1TutorialManager tutorial =
            FindObjectOfType<Module1TutorialManager>();

        if (tutorial != null)
            tutorial.OnCardPicked();

        droppedOnTarget = false;
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Module1TutorialManager tutorial =
            FindObjectOfType<Module1TutorialManager>();

        if (tutorial != null)
            tutorial.ResetInstruction();

        if (!droppedOnTarget)
        {
            ReturnToStart();
        }
    }

    public void ReturnToStart()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);

        rectTransform.anchoredPosition = startPosition;
    }

    public void ResetCard()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);

        rectTransform.anchoredPosition = startPosition;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        canDrag = true;
    }

    public void MarkDropped()
    {
        droppedOnTarget = true;
    }
}