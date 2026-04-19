using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // ADD THIS



public class BlendDropZone : MonoBehaviour, IDropHandler
{
    public bool isCorrect;
    public BlendGameManager gameManager;

    public void OnDrop(PointerEventData eventData)
    {
        BlendDraggable dragged = eventData.pointerDrag?.GetComponent<BlendDraggable>();
        if (dragged == null) return;

        // balik sa gitna
        dragged.ResetPosition(gameManager.wordOriginalParent, gameManager.GetOriginalPos());

        if (isCorrect)
            gameManager.CorrectAnswer();
        else
            gameManager.WrongAnswer();
    }
}