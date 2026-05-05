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


    [Header("Cloud Variations")]
    public Image cloudImage;        // 👈 reference sa Cloud Image
    public CloudData[] cloudVariants;
    public RectTransform textRect; // 👈 para ma-resize text box
    public RectTransform cloudRect; // 👈 para ma-resize cloud

    void Start()
    {
        rt = GetComponent<RectTransform>();

        float randomX = Random.Range(-200f, 200f);
        rt.anchoredPosition = new Vector2(randomX, 700f);

        // 🎨 RANDOM CLOUD
        if (cloudVariants != null && cloudVariants.Length > 0)
        {
            int index = Random.Range(0, cloudVariants.Length);
            CloudData data = cloudVariants[index];

            // 🖼️ SET SPRITE
            if (cloudImage != null)
                cloudImage.sprite = data.sprite;

            // 📦 SET CLOUD SIZE
            if (cloudRect != null)
                cloudRect.sizeDelta = data.cloudSize;

            // 🔤 SET TEXT SIZE
            if (wordTMP != null)
                wordTMP.fontSize = data.fontSize;

            // 📐 SET TEXT BOX SIZE
            // 📐 SET TEXT BOX SIZE
            if (textRect != null)
                textRect.sizeDelta = data.textSize;

            // 📍 SET TEXT POSITION
            if (textRect != null)
                textRect.anchoredPosition = data.textPosition;
        }
    }

    void Update()
    {
        if (answered) return;

        

        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        // ✅ NASALO
        if (CheckCollision())
        {
            bool isCorrect = gameManager.IsCorrectWord(wordText);
            Answer(isCorrect);
            return;
        }

        // ✅ HINDI NASALO
        if (IsBelowScreen())
        {
            Destroy(gameObject);

            if (gameManager != null)
            {
                gameManager.hasActiveWord = false;
                gameManager.SpawnNext();
            }

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

        Destroy(gameObject);

        if (gameManager != null)
        {
            gameManager.hasActiveWord = false;
        }
    }

}

[System.Serializable]
public class CloudData
{
    public Sprite sprite;

    [Header("Cloud Size")]
    public Vector2 cloudSize; // width, height

    [Header("Text Settings")]
    public int fontSize;
    public Vector2 textSize; // width, height

    [Header("Text Position")]
    public Vector2 textPosition; // 👈 NEW
}