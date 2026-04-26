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
    public Image progressBarFill;

    public BlendDropZone choiceAZone;
    public BlendDropZone choiceBZone;

    public BlendDraggable draggableWord;
    public Transform wordOriginalParent;



    private Vector2 originalPos;

    private List<Question> questions = new List<Question>();
    private int currentIndex = 0;

    private int tutorialStep = 0;

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
        ShowQuestion();

        StartCoroutine(TutorialIntro());
    }

    void SetupQuestions()
    {
        questions.Clear();

        questions.Add(new Question("bl", "black", "tabl", "black"));
        questions.Add(new Question("tr", "tree", "cartr", "tree"));
        questions.Add(new Question("pl", "play", "appl", "play"));
    }

    IEnumerator TutorialIntro()
    {
        tutorialStep = 0;

        instructionText.text = "Welcome! Let's learn about beginning sounds!";
        yield return new WaitForSeconds(2f);

        instructionText.text = "Look at the word in the middle.";
        yield return new WaitForSeconds(2f);

        instructionText.text = "Choose the word that starts the same sound.";
        yield return new WaitForSeconds(2f);

        instructionText.text = "Drag it to your answer!";
    }

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            EndTutorial();
            return;
        }

        Question q = questions[currentIndex];

        // 🧠 MAS TEACHING STYLE
        if (currentIndex == 0)
            instructionText.text = "Listen: 'bl' sound. Which word starts with 'bl'?";
        else if (currentIndex == 1)
            instructionText.text = "Now try 'tr' sound. Find the correct word!";
        else
            instructionText.text = "Last one! Look carefully at the starting sound.";

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

        // progress
        progressText.text = $"Tutorial {currentIndex + 1} / {questions.Count}";
        progressBarFill.fillAmount = (float)currentIndex / questions.Count;
    }

    public void CheckAnswer(string answer)
    {
        if (currentIndex >= questions.Count) return;

        string correct = questions[currentIndex].correctAnswer;

        if (answer == correct)
        {
            instructionText.text = "Great job! 🎉 That word starts with the correct sound!";
            Invoke("NextQuestion", 1.2f);
        }
        else
        {
            instructionText.text = "Good try! 😊 Let's look at the first sound again.";
            Invoke("RestartTutorial", 1.5f);
        }
    }

    void RestartTutorial()
    {
        currentIndex = 0;

        draggableWord.ResetPosition(wordOriginalParent, originalPos);

        ShowQuestion();
    }

    void NextQuestion()
    {
        currentIndex++;

        draggableWord.ResetPosition(wordOriginalParent, originalPos);

        ShowQuestion();
    }

    void EndTutorial()
    {
        instructionText.text = "Awesome! 🎉 You're ready to play!";
        Invoke("GoToGame", 2f);
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
}