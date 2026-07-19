using UnityEngine;
using UnityEngine.EventSystems;

public class Module1TutorialDropArea : MonoBehaviour, IDropHandler
{
    public Module1TutorialManager tutorialManager;

    public void OnDrop(PointerEventData eventData)
    {
        Module1DraggableChoice draggedCard =
            eventData.pointerDrag.GetComponent<Module1DraggableChoice>();

        if (draggedCard != null)
        {
            tutorialManager.OnChoiceDropped(draggedCard);
        }

        if (draggedCard != null)
        {
            draggedCard.MarkDropped();
            tutorialManager.OnChoiceDropped(draggedCard);
        }
    }
}