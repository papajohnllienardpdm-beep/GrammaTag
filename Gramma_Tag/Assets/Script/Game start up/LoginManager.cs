using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // ADD THIS
using System.Text.RegularExpressions;


public class LoginManager : MonoBehaviour
{
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField ageInput;
    public Button startButton;

    [Header("Gender Buttons")]
    public Image girlButtonImage;
    public Image boyButtonImage;

    [Header("Girl Sprites")]
    public Sprite girlNormalSprite;
    public Sprite girlSelectedSprite;

    [Header("Boy Sprites")]
    public Sprite boyNormalSprite;
    public Sprite boySelectedSprite;

    public GameObject validationPanel;
    public TMP_Text validationText;

    private string playerSex = "";

    public void SelectGirl()
    {
        playerSex = "Girl";

        // 🔥 GIRL SELECTED
        if (girlButtonImage != null)
            girlButtonImage.sprite = girlSelectedSprite;

        // 🔥 BOY BACK TO NORMAL
        if (boyButtonImage != null)
            boyButtonImage.sprite = boyNormalSprite;
    }

    public void SelectBoy()
    {
        playerSex = "Boy";

        // 🔥 BOY SELECTED
        if (boyButtonImage != null)
            boyButtonImage.sprite = boySelectedSprite;

        // 🔥 GIRL BACK TO NORMAL
        if (girlButtonImage != null)
            girlButtonImage.sprite = girlNormalSprite;
    }

    private bool isSaving = false;

    void Start()
    {
        // 🔥 DEFAULT BUTTON LOOK
        if (girlButtonImage != null)
            girlButtonImage.sprite = girlNormalSprite;

        if (boyButtonImage != null)
            boyButtonImage.sprite = boyNormalSprite;
    }

    bool ValidateInputs()
    {
        // 🔥 FIRST NAME EMPTY
        if (string.IsNullOrWhiteSpace(firstNameInput.text))
        {
            ShowValidation("Please enter your first name.");
            return false;
        }

        // 🔥 FIRST NAME LETTERS ONLY
        if (!Regex.IsMatch(firstNameInput.text, @"^[a-zA-Z\s]+$"))
        {
            ShowValidation("First name should contain letters only.");
            return false;
        }

        // 🔥 LAST NAME EMPTY
        if (string.IsNullOrWhiteSpace(lastNameInput.text))
        {
            ShowValidation("Please enter your last name.");
            return false;
        }

        // 🔥 LAST NAME LETTERS ONLY
        if (!Regex.IsMatch(lastNameInput.text, @"^[a-zA-Z\s]+$"))
        {
            ShowValidation("Last name should contain letters only.");
            return false;
        }

        // 🔥 AGE EMPTY
        if (string.IsNullOrWhiteSpace(ageInput.text))
        {
            ShowValidation("Please enter your age.");
            return false;
        }

        // 🔥 AGE NUMBERS ONLY
        if (!Regex.IsMatch(ageInput.text, @"^\d+$"))
        {
            ShowValidation("Age should contain numbers only.");
            return false;
        }

        // 🔥 GENDER CHECK
        if (string.IsNullOrEmpty(playerSex))
        {
            ShowValidation("Please select your gender.");
            return false;
        }

        return true;
    }

    void ShowValidation(string message)
    {
        if (validationPanel != null)
        {
            validationPanel.SetActive(true);

            StopCoroutine("PopupAnimation");
            StartCoroutine("PopupAnimation");
        }

        if (validationText != null)
            validationText.text = message;
    }

    IEnumerator PopupAnimation()
    {
        RectTransform panelRect = validationPanel.GetComponent<RectTransform>();

        panelRect.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float scale = Mathf.SmoothStep(0f, 1f, timer / duration);

            panelRect.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        panelRect.localScale = Vector3.one;
    }

    public void CloseValidation()
    {
        if (validationPanel != null)
        {
            StopCoroutine("PopupAnimation");
            StartCoroutine("ClosePopupAnimation");
        }
    }

    IEnumerator ClosePopupAnimation()
    {
        RectTransform panelRect = validationPanel.GetComponent<RectTransform>();

        float timer = 0f;
        float duration = 0.12f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            panelRect.localScale = Vector3.Lerp(startScale, endScale, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }

        panelRect.localScale = Vector3.zero;

        validationPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (isSaving) return;

        isSaving = true;

        if (startButton != null)
            startButton.interactable = false;

        if (!ValidateInputs())
        {
            Debug.Log("Complete your profile first!");

            if (startButton != null)
                startButton.interactable = true;

            isSaving = false;
            return;
        }

        int age;

        if (!int.TryParse(ageInput.text.Trim(), out age))
        {
            ShowValidation("Invalid age entered.");

            if (startButton != null)
                startButton.interactable = true;

            isSaving = false;
            return;
        }

        StartCoroutine(WaitAndSaveUser(age));
    }

    IEnumerator WaitAndSaveUser(int age)
    {
        yield return new WaitUntil(() =>
            DatabaseManager.Instance != null &&
            DatabaseManager.Instance.IsDatabaseReady()
        );

        Debug.Log("✅ DB READY SA LOGIN");

        DatabaseManager.Instance.InsertUser(
            firstNameInput.text.Trim(),
            lastNameInput.text.Trim(),
            age,
            playerSex
        );

        Debug.Log("✅ USER SAVED");

        SceneManager.LoadScene("CutScene1");
    }


}