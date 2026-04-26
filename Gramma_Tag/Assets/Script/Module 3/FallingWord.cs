using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void Start()
    {
        rt = GetComponent<RectTransform>();
        float randomX = Random.Range(-200f, 200f);
        rt.anchoredPosition = new Vector2(randomX, 700f);
    }

    void Update()
    {
        if (answered) return;

        if (gameManager != null && gameManager.isTransitioning)
        {
            Destroy(gameObject);
            return;
        }

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

            if (gameManager != null && !gameManager.isTransitioning)
            {
                gameManager.hasActiveWord = false;
                gameManager.SpawnNext(); // 🔥 ALWAYS CONTINUE
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
