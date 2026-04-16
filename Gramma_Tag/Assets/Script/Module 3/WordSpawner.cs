using UnityEngine;
using TMPro;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform spawnPoint;
    public GameManager_Module3 gameManager;
    public RectTransform gameArea;

    public string[] words = { "ship", "chair", "shoe", "chicken", "shark", "cheese", "shop", "chalk", "shell", "chop" };
    public string[] answers = { "SH", "CH", "SH", "CH", "SH", "CH", "SH", "CH", "SH", "CH" };

    public void Spawn(int index)
    {
        GameObject obj = Instantiate(wordPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);

        obj.GetComponentInChildren<TextMeshProUGUI>().text = words[index];

        FallingWord fw = obj.GetComponent<FallingWord>();

        fw.correctAnswer = answers[index];
        fw.gameManager = gameManager;

        // 👇 ADD THIS
        fw.basketCH = GameObject.Find("Basket_CH").GetComponent<RectTransform>();
        fw.basketSH = GameObject.Find("Basket_SH").GetComponent<RectTransform>();

        fw.gameArea = gameArea;

        fw.catchPointCH = GameObject.Find("Basket_CH/CatchPoint").GetComponent<RectTransform>();
        fw.catchPointSH = GameObject.Find("Basket_SH/CatchPoint").GetComponent<RectTransform>();
    }
}