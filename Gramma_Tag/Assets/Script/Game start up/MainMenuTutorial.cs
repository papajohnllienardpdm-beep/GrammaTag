using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTutorial : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public GameObject target;
        public string title;
        public string description;
    }

    public List<TutorialStep> steps;

    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Button nextButton;
    public Button skipButton;

    public GameObject tutorialCanvas;

    private Canvas currentCanvas;

    private int currentStep = 0;

    private Dictionary<GameObject, bool> originalOverrideSorting =
     new Dictionary<GameObject, bool>();

    private Dictionary<GameObject, int> originalSortingOrder =
        new Dictionary<GameObject, int>();


    void Start()
    {
        /*
        if (PlayerPrefs.GetInt("MainMenuTutorialDone", 0) == 1)
        {
            tutorialCanvas.SetActive(false);
            return;
        }
        */

        tutorialCanvas.SetActive(true);

        nextButton.onClick.AddListener(NextStep);
        skipButton.onClick.AddListener(SkipTutorial);

        ShowStep();
    }

    void ShowStep()
    {
        if (steps == null || steps.Count == 0)
        {
            Debug.LogError("Tutorial steps list is empty!");
            return;
        }

        if (currentStep < 0 || currentStep >= steps.Count)
        {
            Debug.LogError("Tutorial step index out of range!");
            return;
        }

        TutorialStep step = steps[currentStep];

        titleText.text = step.title;
        descriptionText.text = step.description;

        Highlight(step.target);
    }

    void Highlight(GameObject target)
    {
        if (target == null)
            return;

        Canvas canvas =
            target.GetComponent<Canvas>();

        if (canvas == null)
        {
            canvas =
                target.AddComponent<Canvas>();
        }

        GraphicRaycaster raycaster =
            target.GetComponent<GraphicRaycaster>();

        if (raycaster == null)
        {
            target.AddComponent<GraphicRaycaster>();
        }

        // SAVE ORIGINAL VALUES PER OBJECT
        originalOverrideSorting[target] =
            canvas.overrideSorting;

        originalSortingOrder[target] =
            canvas.sortingOrder;

        // APPLY HIGHLIGHT
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1001;

        Graphic[] graphics =
    target.GetComponentsInChildren<Graphic>(true);

foreach (Graphic g in graphics)
{
    g.canvasRenderer.SetAlpha(1f);
}
    }

    void RemoveHighlight(GameObject target)
    {
        if (target == null)
            return;

        Canvas canvas =
            target.GetComponent<Canvas>();

        if (canvas != null)
        {
            // RESTORE ORIGINAL VALUES
            if (originalOverrideSorting.ContainsKey(target))
            {
                canvas.overrideSorting =
                    originalOverrideSorting[target];
            }

            if (originalSortingOrder.ContainsKey(target))
            {
                canvas.sortingOrder =
                    originalSortingOrder[target];
            }
        }
    }

    public void NextStep()
    {
        TutorialStep current =
            steps[currentStep];

        RemoveHighlight(current.target);

        currentStep++;

        if (currentStep >= steps.Count)
        {
            FinishTutorial();
            return;
        }

        ShowStep();
    }

    public void SkipTutorial()
    {
        FinishTutorial();
    }

    void FinishTutorial()
    {
        if (currentStep >= 0 &&
            currentStep < steps.Count)
        {
            RemoveHighlight(
                steps[currentStep].target);
        }

        tutorialCanvas.SetActive(false);

        PlayerPrefs.SetInt(
            "MainMenuTutorialDone", 1);
    }
}