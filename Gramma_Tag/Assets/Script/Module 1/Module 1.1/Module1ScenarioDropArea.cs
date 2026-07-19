using UnityEngine;
using UnityEngine.EventSystems;

public class Module1ScenarioDropArea : MonoBehaviour, IDropHandler
{
    public Module1GameManager gameManager;

    public void OnDrop(PointerEventData eventData)
    {
        Module1DraggableChoice draggedCard =
            eventData.pointerDrag.GetComponent<Module1DraggableChoice>();

        if (draggedCard != null)
        {
            gameManager.OnChoiceDropped(draggedCard);
        }

        if (draggedCard != null)
        {
            draggedCard.MarkDropped();
            gameManager.OnChoiceDropped(draggedCard);
        }
    }
}