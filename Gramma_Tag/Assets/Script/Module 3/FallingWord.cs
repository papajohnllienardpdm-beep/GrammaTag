using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FallingWord : MonoBehaviour
{
    public string wordText;
    public GameManager_Module3 gameManager;

    private RectTransform rt;
    private bool answered = false;

    public float fallSpeed = 500f;
    public RectTransform gameArea;
    public RectTransform catchPoint;

    public TextMeshProUGUI wordTMP;

    [Header("Cloud References")]
    public Image cloudImage;
    public RectTransform textRect;
    public RectTransform cloudRect;

    

    [Header("Choice Cloud Visuals - SET THIS IN PREFAB")]
    public ChoiceCloudVisual[] choiceCloudVisuals;

    [Header("Runtime Choice Info")]
    public int currentModuleID;
    public int currentQuizID;
    public string currentChoiceKey;

    void Start()
    {
        rt = GetComponent<RectTransform>();

        float randomX = Random.Range(-200f, 200f);
        rt.anchoredPosition = new Vector2(randomX, 700f);
    }

    public void SetupChoiceVisual(int moduleID, int quizID, string choiceKey)
    {
        currentModuleID = moduleID;
        currentQuizID = quizID;
        currentChoiceKey = choiceKey;

        CloudData matchedCloud = GetVisualCloud(
            currentModuleID,
            currentQuizID,
            currentChoiceKey
        );

        if (matchedCloud != null)
        {
            ApplyCloudData(matchedCloud);

            Debug.Log(
                "✅ Cloud applied: ModuleID " + currentModuleID +
                " | QuizID " + currentQuizID +
                " | ChoiceKey " + currentChoiceKey
            );
        }
        else
        {
            Debug.LogWarning(
                "⚠️ No assigned cloud found for ModuleID: " + currentModuleID +
                " | QuizID: " + currentQuizID +
                " | ChoiceKey: " + currentChoiceKey
            );
        }
    }

    CloudData GetVisualCloud(int moduleID, int quizID, string choiceKey)
    {
        if (choiceCloudVisuals == null) return null;

        string cleanChoiceKey = choiceKey.Trim();

        foreach (ChoiceCloudVisual visual in choiceCloudVisuals)
        {
            if (visual.moduleID == moduleID &&
                visual.quizID == quizID &&
                visual.choiceKey.Trim() == cleanChoiceKey)
            {
                return visual.cloudData;
            }
        }

        return null;
    }



    public void ApplyCloudData(CloudData data)
    {
        if (data == null) return;

        if (cloudImage != null)
            cloudImage.sprite = data.sprite;

        if (cloudRect != null)
            cloudRect.sizeDelta = data.cloudSize;

        if (wordTMP != null)
            wordTMP.fontSize = data.fontSize;

        if (textRect != null)
        {
            textRect.sizeDelta = data.textSize;
            textRect.anchoredPosition = data.textPosition;
        }
    }

    void Update()
    {
        if (answered) return;

        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        if (CheckCollision())
        {
            bool isCorrect = gameManager.IsCorrectWord(wordText);
            Answer(isCorrect);
            return;
        }

        if (IsBelowScreen())
        {
            if (gameManager != null)
            {
                gameManager.OnFallingWordFinished();

                if (gameManager.isTutorial)
                    gameManager.SpawnNext();
            }

            Destroy(gameObject);
            return;
        }
    }

    bool CheckCollision()
    {
        Vector3[] catchCorners = new Vector3[4];
        catchPoint.GetWorldCorners(catchCorners);

        Vector3 wordCenter = rt.position;

        return wordCenter.x > catchCorners[0].x &&
               wordCenter.x < catchCorners[2].x &&
               wordCenter.y > catchCorners[0].y &&
               wordCenter.y < catchCorners[1].y;
    }

    bool IsBelowScreen()
    {
        Vector3[] corners = new Vector3[4];
        gameArea.GetWorldCorners(corners);

        float bottomY = corners[0].y;

        Vector3[] wordCorners = new Vector3[4];
        rt.GetWorldCorners(wordCorners);

        return wordCorners[0].y < bottomY;
    }

    void Answer(bool correct)
    {
        if (answered) return;

        answered = true;

        gameManager.Answer(correct);

        if (gameManager != null)
        {
            gameManager.OnFallingWordFinished();

            if (gameManager.isTutorial)
                gameManager.SpawnNext();
        }

        Destroy(gameObject);
    }
}

[System.Serializable]
public class CloudData
{
    public Sprite sprite;

    [Header("Cloud Size")]
    public Vector2 cloudSize;

    [Header("Text Settings")]
    public int fontSize;
    public Vector2 textSize;

    [Header("Text Position")]
    public Vector2 textPosition;
}

[System.Serializable]
public class ChoiceCloudVisual
{
    public int moduleID;
    public int quizID;

    [Tooltip("Use exactly: ChoiceA, ChoiceB, or ChoiceC")]
    public string choiceKey;

    public CloudData cloudData;
}