using UnityEngine;
using UnityEngine.UI;

public class Module5TutorialEraserController : MonoBehaviour
{
    [Header("References")]
    public Canvas canvas;
    public RectTransform originalEraser;
    public RectTransform floatingEraser;
    public Module5TutorialManager tutorialManager;

    [Header("Settings")]
    public float eraseRadius = 70f;

    private bool isHolding = false;
    private Image originalImage;

    void Start()
    {
        if (originalEraser != null)
            originalImage = originalEraser.GetComponent<Image>();

        if (floatingEraser != null)
            floatingEraser.gameObject.SetActive(false);
    }

    void Update()
    {
        if (canvas == null || originalEraser == null || floatingEraser == null)
            return;

        if (!isHolding)
        {
            if (IsPressedOnOriginalEraser())
                StartHold();
        }
        else
        {
            MoveFloatingEraser();
            CheckErase();

            if (IsReleased())
                StopHold();
        }
    }

    bool IsPressedOnOriginalEraser()
    {
        Vector2 screenPos;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            screenPos = Input.GetTouch(0).position;
        else if (Input.GetMouseButtonDown(0))
            screenPos = Input.mousePosition;
        else
            return false;

        Camera cam =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ?
            null :
            canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(
            originalEraser,
            screenPos,
            cam
        );
    }

    void StartHold()
    {
        isHolding = true;

        if (originalImage != null)
            originalImage.enabled = false;

        floatingEraser.gameObject.SetActive(true);

        MoveFloatingEraser();
    }

    void MoveFloatingEraser()
    {
        Vector2 screenPos;

        if (Input.touchCount > 0)
            screenPos = Input.GetTouch(0).position;
        else
            screenPos = Input.mousePosition;

        RectTransform canvasRect =
            canvas.GetComponent<RectTransform>();

        Camera cam =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ?
            null :
            canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            cam,
            out Vector2 localPos
        );

        floatingEraser.anchoredPosition = localPos;
    }

    void CheckErase()
    {
        if (tutorialManager != null)
            tutorialManager.CheckEraserCollision(
                floatingEraser.anchoredPosition,
                eraseRadius
            );
    }

    bool IsReleased()
    {
        if (Input.touchCount > 0)
        {
            TouchPhase phase = Input.GetTouch(0).phase;

            return phase == TouchPhase.Ended ||
                   phase == TouchPhase.Canceled;
        }

        return Input.GetMouseButtonUp(0);
    }

    public void StopHold()
    {
        isHolding = false;

        floatingEraser.gameObject.SetActive(false);

        if (originalImage != null)
            originalImage.enabled = true;
    }
}