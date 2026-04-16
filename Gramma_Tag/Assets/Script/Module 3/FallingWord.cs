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

    public RectTransform catchPointCH;
    public RectTransform catchPointSH;


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

        if (!isProcessing && BasketDrag.activeBasket == "CH" && CheckCollision(catchPointCH))
        {
            CheckAnswer("CH");
            return;
        }
        else if (!isProcessing && BasketDrag.activeBasket == "SH" && CheckCollision(catchPointSH))
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

        RectTransform targetPoint = (basketTag == "CH") ? catchPointCH : catchPointSH;

        // 👇 SNAP AGAD (NO LERP / NO ANGAT)
        rt.position = targetPoint.position + new Vector3(0, -20f, 0);

        StartCoroutine(DestroyAndAnswer(basketTag == correctAnswer));

        GetComponent<CanvasGroup>().alpha = 0f;
    }



    bool CheckCollision(RectTransform catchPoint)
    {
        if (BasketDrag.activeBasket == "") return false;

        Vector2 wordPos = RectTransformUtility.WorldToScreenPoint(null, rt.position);
        Vector2 catchPos = RectTransformUtility.WorldToScreenPoint(null, catchPoint.position);

        float yDiff = Mathf.Abs(wordPos.y - catchPos.y);
        float xDiff = Mathf.Abs(wordPos.x - catchPos.x);

        // 🔥 dapat malapit sa butas vertically
        if (yDiff > 1f) return false;

        // 🔥 dapat nasa loob ng lapad ng basket
        if (xDiff > 100f) return false;

        return true;
    }

    IEnumerator DestroyAndAnswer(bool isCorrect)
    {
        yield return null;

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