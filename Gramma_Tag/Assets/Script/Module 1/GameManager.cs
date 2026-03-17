// GameManager.cs — attach to the GameManager object
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private QuestionManager qm;
    private UIManager ui;
    private ScoreManager sm;

    private int matchTotal = 0;
    private int matchAnswered = 0;
    private bool matchError = false;
    private bool locked = false;

    public bool IsLocked()
    {
        return locked;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        qm = GetComponent<QuestionManager>();
        ui = GetComponent<UIManager>();
        sm = GetComponent<ScoreManager>();
    }

    void Start()
    {
        sm.Reset();
        Load();
    }

    void Load()
    {
        if (qm.IsFinished()) { EndGame(); return; }
        locked = false;
        var q = qm.GetCurrent();
        ui.UpdateProgress(qm.CurrentNumber(), qm.Total());
        ui.HideFeedback();
        if (q.questionType == QuestionType.DragDrop)
            ui.ShowDragDrop(q);
        else
        {
            matchTotal = q.matchPairs.Count;
            matchAnswered = 0;
            matchError = false;
            ui.ShowMatching(q);
        }
    }

    public void OnDragDropCorrect()
    {
        if (locked) return;
        locked = true;
        sm.AddPoint();
        ui.ShowFeedback(true, qm.GetCurrent().explanation);
    }

    public void OnDragDropWrong(string correct)
    {
        if (locked) return;
        locked = true;
        ui.ShowFeedback(false,
            "Correct answer: \"" + correct + "\". " +
            qm.GetCurrent().explanation);
    }

    public void OnMatchItemPlaced(bool correct)
    {
        if (!correct) matchError = true;
        matchAnswered++;
        if (matchAnswered >= matchTotal)
        {
            locked = true;
            bool allOk = !matchError;
            if (allOk) sm.AddPoint();
            ui.ShowFeedback(allOk, qm.GetCurrent().explanation);
        }
    }

    public string GetExpectedZone(string label)
    {
        var q = qm.GetCurrent();
        if (q == null) return "";
        foreach (var p in q.matchPairs)
            if (p.word == label) return p.correctLabel;
        return "";
    }

    public void OnNextPressed()
    {
        qm.Advance();
        Load();
    }

    void EndGame()
    {
        sm.CalculateCoins();
        PlayerPrefs.SetInt("FinalScore", sm.GetScore());
        PlayerPrefs.SetInt("FinalCoins", sm.GetCoins());
        PlayerPrefs.SetInt("TotalQ", qm.Total());
        PlayerPrefs.SetInt("Passed", sm.HasPassed() ? 1 : 0);
        SceneManager.LoadScene("ResultScene");

        Debug.Log("Quiz Finished!");
    }
}