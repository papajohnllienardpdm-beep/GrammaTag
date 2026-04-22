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

    public TextMeshProUGUI wordTMP; // 👈 ADD THIS



    void Start()
    {
        rt = GetComponent<RectTransform>();

        float randomX = Random.Range(-200f, 200f);
        rt.anchoredPosition = new Vector2(randomX, 700f);

    }

    void Update()
    {
        if (answered) return;

        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        // ✅ 1. NASALO
        if (CheckCollision())
        {
            bool isCorrect = gameManager.IsCorrectWord(wordText);

            // 👉 parehong may feedback
            Answer(isCorrect);

            return;
        }

        // ✅ 2. HINDI NASALO
        if (IsBelowScreen())
        {
            bool isCorrect = gameManager.IsCorrectWord(wordText);

            if (isCorrect)
            {
                // ❌ dapat nasalo pero hindi
                if (gameManager.isTutorial)
                {
                    gameManager.ResetTutorial(); // 🔥 ADD THIS
                }

                Answer(false);
            }
            else
            {
                // ✅ tama na hindi sinalo
                if (gameManager.isTutorial)
                {
                    gameManager.ShowAvoidFeedback();
                }

                Destroy(gameObject);

                // 🔥 IMPORTANT: sabihin sa GameManager na pwede na ulit mag spawn
                if (gameManager != null)
                {
                    gameManager.hasActiveWord = false;
                    gameManager.Invoke("SpawnNext", 0.2f);
                }
            }

            return;
        }
    }
    bool CheckCollision()
    {
        Vector3[] catchCorners = new Vector3[4];
        catchPoint.GetWorldCorners(catchCorners);

        float left = catchCorners[0].x;
        float right = catchCorners[2].x;
        float top = catchCorners[1].y;
        float bottom = catchCorners[0].y;

        // 👇 gamitin CENTER ng word (mas accurate)
        Vector3 wordCenter = rt.position;

        bool insideX = wordCenter.x > left && wordCenter.x < right;
        bool insideY = wordCenter.y > bottom && wordCenter.y < top;

        return insideX && insideY;
    }

    bool IsBelowScreen()
    {
        Vector3[] corners = new Vector3[4];
        gameArea.GetWorldCorners(corners);

        float bottomY = corners[0].y;

        Vector3[] wordCorners = new Vector3[4];
        rt.GetWorldCorners(wordCorners);

        float wordBottom = wordCorners[0].y;

        return wordBottom < bottomY;
    }

    void Answer(bool correct)
    {
        if (answered) return;

        answered = true;

        gameManager.Answer(correct);

        // 🔥 ALWAYS destroy (para clean)
        Destroy(gameObject);
    }
}