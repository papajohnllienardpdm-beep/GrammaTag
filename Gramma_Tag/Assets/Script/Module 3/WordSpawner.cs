using UnityEngine;
using TMPro;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform spawnPoint;
    public GameManager_Module3 gameManager;
    public RectTransform gameArea;
    public RectTransform catchPoint;

    public string[] words = { "ship", "chair", "shoe", "chicken", "shark", "cheese", "shop", "chalk", "shell", "chop" };
    public string[] answers = { "SH", "CH", "SH", "CH", "SH", "CH", "SH", "CH", "SH", "CH" };

    public void Spawn(int index)
    {
        GameObject obj = Instantiate(wordPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);

        FallingWord fw = obj.GetComponent<FallingWord>();
        fw.wordTMP.text = words[index];

       

        fw.wordText = words[index];
        fw.gameManager = gameManager;

        // 👇 ADD THIS


        fw.gameArea = gameArea;

        fw.catchPoint = catchPoint;
    }
}