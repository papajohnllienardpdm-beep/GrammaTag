using UnityEngine;
using TMPro;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform spawnPoint;
    public GameManager_Module3 gameManager;
    public RectTransform gameArea;
    public RectTransform catchPoint;

    public void SpawnWord(string word, int moduleID, int quizID, string choiceKey)
    {
        GameObject obj = Instantiate(wordPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);

        var fw = obj.GetComponent<FallingWord>();

        if (fw == null)
        {
            Debug.LogError("FallingWord script missing on prefab!");
            return;
        }

        fw.wordTMP.text = word;
        fw.wordText = word;
        fw.gameManager = gameManager;

        fw.gameArea = gameArea;
        fw.catchPoint = catchPoint;

        fw.SetupChoiceVisual(moduleID, quizID, choiceKey);
    }
}