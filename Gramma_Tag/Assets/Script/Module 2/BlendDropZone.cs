using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // ADD THIS



public class BlendDropZone : MonoBehaviour, IDropHandler
{
    public string answerText;

    public BlendGameManager gameManager;
    public BlendTutorialManager tutorialManager;

    [Header("Tutorial Images")]
    public GameObject[] imagesToOpen;

    private int imageIndex = 0;

    public void OnDrop(PointerEventData eventData)
    {
        BlendDraggable dragged = eventData.pointerDrag?.GetComponent<BlendDraggable>();
        if (dragged == null) return;

        // ✅ TUTORIAL MODE
        if (tutorialManager != null)
        {
            // snap muna sa choice
            dragged.SnapToZone(transform);

            // kahit tama or mali, bubukas image
            OpenNextImage();

            tutorialManager.CheckAnswer(answerText);
            return;
        }

        // ✅ GAME MODE
        if (gameManager != null)
        {
            OpenNextImage();

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

    void OpenNextImage()
    {
        if (imageIndex >= imagesToOpen.Length) return;

        if (imagesToOpen[imageIndex] != null)
            imagesToOpen[imageIndex].SetActive(true);

        imageIndex++;
    }

    public void ResetImages()
    {
        imageIndex = 0;

        foreach (GameObject img in imagesToOpen)
        {
            if (img != null)
                img.SetActive(false);
        }
    }
}