using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlendTutorialManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI questionText;

    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;

    public TextMeshProUGUI progressText;
    public Slider progressBar;

    public BlendDropZone choiceAZone;
    public BlendDropZone choiceBZone;

    public BlendDraggable draggableWord;
    public Transform wordOriginalParent;

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private Vector2 originalPos;

    private List<Question> questions = new List<Question>();
    private int currentIndex = 0;

    private int correctAnswers = 0;
    private int requiredCorrectAnswers = 3;

    private bool hasTouchedWord = false;

    void Awake()
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
            Destroy(gm.gameObject);
    }

    void Start()
    {
        originalPos = draggableWord.GetComponent<RectTransform>().anchoredPosition;

        SetupQuestions();
        ShuffleQuestions();


        // ✅ set max once
        progressBar.maxValue = requiredCorrectAnswers;
        progressBar.value = 0;

        ShowQuestion();
    }

    void SetupQuestions()
    {
        questions.Clear();

        questions.Add(new Question("bl", "black", "table", "black"));
        questions.Add(new Question("tr", "tree", "car", "tree"));
        questions.Add(new Question("pl", "play", "apple", "play"));
        questions.Add(new Question("cl", "clock", "duck", "clock"));
        questions.Add(new Question("gr", "green", "banana", "green"));
        questions.Add(new Question("sm", "smile", "house", "smile"));
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            Question temp = questions[i];

            int randomIndex = Random.Range(i, questions.Count);

            questions[i] = questions[randomIndex];
            questions[randomIndex] = temp;
        }
    }

    void ShowQuestion()
    {
        // ✅ if tapos na lahat
        if (currentIndex >= questions.Count)
        {
            // 🔥 force full progress
            progressBar.value = requiredCorrectAnswers;
            progressText.text = $"Progress: {requiredCorrectAnswers} / {requiredCorrectAnswers}";

            EndTutorial();
            return;
        }

        Question q = questions[currentIndex];

        // instruction reset
        hasTouchedWord = false;
        if (!hasTouchedWord)
        {
            instructionText.text = "Tap and hold the word first.";
        }

        // random swap
        if (Random.value > 0.5f)
        {
            choiceAText.text = q.choiceA;
            choiceBText.text = q.choiceB;

            choiceAZone.answerText = q.choiceA;
            choiceBZone.answerText = q.choiceB;
        }
        else
        {
            choiceAText.text = q.choiceB;
            choiceBText.text = q.choiceA;

            choiceAZone.answerText = q.choiceB;
            choiceBZone.answerText = q.choiceA;
        }

        // ✅ progress (starts at 0)
        progressBar.value = correctAnswers;
        progressText.text = $"Progress: {correctAnswers} / {requiredCorrectAnswers}";
    }

    public void CheckAnswer(string answer)
    {
        if (currentIndex >= questions.Count) return;

        string correct = questions[currentIndex].correctAnswer;

        if (answer == correct)
        {
            // ✅ dagdag correct count
            correctAnswers++;

            instructionText.text = "Great job! That word starts with the correct sound!";

            // 🔊 PLAY CORRECT SOUND
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSFX);

            // ✅ kapag naka 3 correct na
            if (correctAnswers >= requiredCorrectAnswers)
            {
                // ✅ force full progress UI
                progressBar.value = requiredCorrectAnswers;
                progressText.text = $"Progress: {requiredCorrectAnswers} / {requiredCorrectAnswers}";

                EndTutorial();
                return;
            }

            Invoke("NextQuestion", 1.2f);
        }
        else
        {
            instructionText.text = "Good try! Let's look at the first sound again.";

            // 🔊 PLAY WRONG SOUND
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSFX);

            Invoke("RestartTutorial", 1.5f);
        }
    }

    void NextQuestion()
    {
        currentIndex++;

        draggableWord.ResetPosition(wordOriginalParent, originalPos);

        ShowQuestion();

        hasTouchedWord = false;
    }

    void RestartTutorial()
    {
        currentIndex = 0;
        ShuffleQuestions();

        // ✅ reset correct answers din
        correctAnswers = 0;

        draggableWord.ResetPosition(wordOriginalParent, originalPos);

        // reset progress
        progressBar.value = 0;

        ShowQuestion();

        hasTouchedWord = false;
    }

    void EndTutorial()
    {
        instructionText.text = "Awesome! You're ready to play!";
        Invoke("GoToGame", 0.7f);
    }

    void GoToGame()
    {
        SceneManager.LoadScene("Module2_GameScene");
    }

    public Vector2 GetOriginalPos()
    {
        return originalPos;
    }

    public Transform GetOriginalParent()
    {
        return wordOriginalParent;
    }

    public void OnWordTouched()
    {
        if (!hasTouchedWord)
        {
            hasTouchedWord = true;
            instructionText.text = "Good! Now drag it to the correct answer.";
        }
    }
}
