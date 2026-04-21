using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // ADD THIS



public class BlendDropZone : MonoBehaviour, IDropHandler
{
    public string answerText; // 🔥 eto ang laman ng button
    public BlendGameManager gameManager;

    public void OnDrop(PointerEventData eventData)
    {
        BlendDraggable dragged = eventData.pointerDrag?.GetComponent<BlendDraggable>();
        if (dragged == null) return;

        // snap pabalik
        dragged.ResetPosition(gameManager.wordOriginalParent, gameManager.GetOriginalPos());

        // 🔥 CHECK using STRING
        if (answerText == gameManager.GetCurrentCorrectAnswer())
            gameManager.CorrectAnswer();
        else
            gameManager.WrongAnswer();
    }
}