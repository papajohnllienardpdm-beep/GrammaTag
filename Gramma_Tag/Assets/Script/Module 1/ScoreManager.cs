// ScoreManager.cs — attach to the GameManager object
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private int coins = 0;

    public void Reset() { score = 0; coins = 0; }
    public void AddPoint() { score++; }
    public int GetScore() => score;
    public int GetCoins() => coins;
    public bool HasPassed()
    {
        return (float)score / 10 >= 0.6f;
    }
    public void CalculateCoins() { coins = HasPassed() ? 100 : 20; }
}