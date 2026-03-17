using UnityEngine;
using UnityEngine.EventSystems;

public class MatchZone : MonoBehaviour, IDropHandler
{
    public string zoneLabel;

    public void ResetZone()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<MatchItem>() != null)
                Destroy(child.gameObject);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (GetComponentInChildren<MatchItem>() != null) return;

        MatchItem item = eventData.pointerDrag?.GetComponent<MatchItem>();
        if (item == null) return;

        string expected = GameManager.Instance.GetExpectedZone(item.itemLabel);
        bool correct = expected.Trim().ToLower() == zoneLabel.Trim().ToLower();

        item.SnapToZone(transform);
        GameManager.Instance.OnMatchItemPlaced(correct);
    }
}