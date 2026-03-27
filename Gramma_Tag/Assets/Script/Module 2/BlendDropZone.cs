using UnityEngine;
using UnityEngine.EventSystems;

public class BlendDropZone : MonoBehaviour, IDropHandler
{
    public string category;
    public BlendGameManager gameManager;

    public void OnDrop(PointerEventData eventData)
    {
        BlendDraggable dragged = eventData.pointerDrag?.GetComponent<BlendDraggable>();

        if (dragged == null) return;

        dragged.SnapToZone(transform); // 🔥 snap muna
        gameManager.SubmitAnswer(category);
    }
}