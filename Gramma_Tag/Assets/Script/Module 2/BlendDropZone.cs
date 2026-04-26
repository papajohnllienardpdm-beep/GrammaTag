using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // ADD THIS



public class BlendDropZone : MonoBehaviour, IDropHandler
{
    public string answerText; // 🔥 eto ang laman ng button
    public BlendGameManager gameManager;
    public BlendTutorialManager tutorialManager; // ADD

    public void OnDrop(PointerEventData eventData)
    {
        BlendDraggable dragged = eventData.pointerDrag?.GetComponent<BlendDraggable>();
        if (dragged == null) return;

        // ✅ TUTORIAL MODE
        if (tutorialManager != null)
        {
            // 👉 SNAP muna (para dumikit)
            dragged.SnapToZone(transform);

            tutorialManager.CheckAnswer(answerText);
            return;
        }

        // ✅ GAME MODE
        if (gameManager != null)
        {
            dragged.ResetPosition(
                gameManager.wordOriginalParent,
                gameManager.GetOriginalPos()
            );

            if (answerText == gameManager.GetCurrentCorrectAnswer())
                gameManager.CorrectAnswer();
            else
                gameManager.WrongAnswer();
        }
    }
}