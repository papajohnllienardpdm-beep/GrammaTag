using UnityEngine;
using UnityEngine.EventSystems;

public class Module5WordItem : MonoBehaviour, IPointerClickHandler
{
    private Module5BoardCleanerManager manager;
    private bool shouldStay;

    public void Setup(Module5BoardCleanerManager gameManager, bool correctWord)
    {
        manager = gameManager;
        shouldStay = correctWord;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnWordClicked(this, shouldStay);
    }
}