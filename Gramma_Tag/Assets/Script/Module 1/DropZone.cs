// DropZone.cs
// Attach this to the DropZone Image object in DragDropPanel.
// The Image component is required for Unity to detect drops.
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [HideInInspector] public string correctAnswer;
    private bool hasAnswer = false;

    public void ResetZone()
    {
        hasAnswer = false;
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (hasAnswer) return;

        DraggableWord dragged =
            eventData.pointerDrag?.GetComponent<DraggableWord>();
        if (dragged == null) return;

        if (dragged.wordValue == correctAnswer)
        {
            hasAnswer = true;
            dragged.SnapToZone(transform);
            GameManager.Instance.OnDragDropCorrect();
        }
        else
        {
            dragged.ReturnToStart();
            GameManager.Instance.OnDragDropWrong(correctAnswer);
        }
    }
}