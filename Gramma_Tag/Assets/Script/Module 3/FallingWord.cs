using UnityEngine;
using TMPro;
using System.Collections;

public class FallingWord : MonoBehaviour
{
    public string correctAnswer;
    public GameManager_Module3 gameManager;

    public RectTransform basketCH;
    public RectTransform basketSH;

    private RectTransform rt;
    private bool answered = false;

    public float fallSpeed = 500f;

    private float spawnTime;
    public float catchDelay = 0f;

    private bool isProcessing = false;

    public RectTransform gameArea; // 🔥 ADD THIS

    // 🔥 DITO MO ILALAGAY




    void Start()
    {
        rt = GetComponent<RectTransform>();

        // 🔥 IMPORTANT: ilagay sa taas agad
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 400f);

        StartCoroutine(EnableCollisionDelay());
    }

    IEnumerator EnableCollisionDelay()
    {
        yield return new WaitForSeconds(0.2f);
        catchDelay = 1f;
    }

    void Update()
    {
        if (answered || !gameObject.activeInHierarchy) return;

        if (catchDelay == 0f) return;

        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;


        Vector3[] corners = new Vector3[4];
        gameArea.GetWorldCorners(corners);

        float bottomY = corners[0].y;
        Vector3[] wordCorners = new Vector3[4];
        rt.GetWorldCorners(wordCorners);

        float wordBottom = wordCorners[0].y;

        // 🔥 check kung buong word ay lampas na sa screen
        if (wordBottom < bottomY)
        {
            MissedWord();
            return;
        }



        // 🔥 prevent early catch
        if (rt.anchoredPosition.y > 100f) return;

        if (!isProcessing && BasketDrag.activeBasket == "CH" && CheckCollision(basketCH))
        {
            CheckAnswer("CH");
            return;
        }
        else if (!isProcessing && BasketDrag.activeBasket == "SH" && CheckCollision(basketSH))
        {
            CheckAnswer("SH");
            return;
        }
    }

    // ✅ NASA LABAS NA NG UPDATE
    void CheckAnswer(string basketTag)
    {
        if (answered || isProcessing) return;

        isProcessing = true;
        answered = true;

        enabled = false; // 🔥 IMPORTANT

        Debug.Log("HIT DETECTED: " + basketTag + " | Correct: " + correctAnswer);

        StartCoroutine(DestroyAndAnswer(basketTag == correctAnswer));
    }



    bool CheckCollision(RectTransform basket)
    {
        if (BasketDrag.activeBasket == "") return false;

        // 👉 kunin screen position ng word (center)
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rt.position);

        // 👉 check kung nasa loob ng basket
        return RectTransformUtility.RectangleContainsScreenPoint(
            basket,
            screenPoint,
            null
        );
    }

    IEnumerator DestroyAndAnswer(bool isCorrect)
    {
        yield return new WaitForSeconds(0.05f);

        if (gameManager != null)
        {
            gameManager.Answer(isCorrect);
        }

        Destroy(gameObject);
    }

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager_Module3>();

        if (basketCH == null)
            basketCH = GameObject.Find("Basket_CH").GetComponent<RectTransform>();

        if (basketSH == null)
            basketSH = GameObject.Find("Basket_SH").GetComponent<RectTransform>();
    }

    void MissedWord()
    {
        if (answered || isProcessing) return;

        isProcessing = true;
        answered = true;

        Debug.Log("MISSED WORD: " + correctAnswer);

        StartCoroutine(DestroyAndAnswer(false)); // ❌ mali
    }
}