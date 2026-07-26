using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SettingsUIManager : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField ageInput;
    public TMP_InputField genderInput;

    [Header("Button")]
    public Button editButton;
    public Button logoutButton;

    [Header("Character Image")]
    public Image charImage;
    public Sprite boySprite;
    public Sprite girlSprite;

    [Header("Validation")]
    public GameObject validationPanel;
    public TMP_Text validationText;

    [Header("Credits")]
    public Button creditsButton;
    public Button closeCreditsButton;

    public GameObject creditsPanel;

    [Header("Credits Animation")]
    public float creditsPopupDuration = 0.15f;

    private Coroutine creditsCoroutine;

    [Header("Exit Confirmation")]
    public GameObject exitPanel;

    public Button yesExitButton;
    public Button noExitButton;

    [Header("Exit Animation")]
    public float exitPopupDuration = 0.15f;

    private Coroutine exitCoroutine;


    private bool isEditing = false;


    bool ValidateInputs()
    {
        // FIRST NAME EMPTY
        if (string.IsNullOrWhiteSpace(firstNameInput.text))
        {
            ShowValidation("Please enter your first name.");
            return false;
        }

        // FIRST NAME LETTERS ONLY
        if (!Regex.IsMatch(firstNameInput.text.Trim(), @"^[a-zA-Z\s]+$"))
        {
            ShowValidation("First name should contain letters only.");
            return false;
        }

        // LAST NAME EMPTY
        if (string.IsNullOrWhiteSpace(lastNameInput.text))
        {
            ShowValidation("Please enter your last name.");
            return false;
        }

        // LAST NAME LETTERS ONLY
        if (!Regex.IsMatch(lastNameInput.text.Trim(), @"^[a-zA-Z\s]+$"))
        {
            ShowValidation("Last name should contain letters only.");
            return false;
        }

        // AGE EMPTY
        if (string.IsNullOrWhiteSpace(ageInput.text))
        {
            ShowValidation("Please enter your age.");
            return false;
        }

        // AGE NUMBERS ONLY
        if (!Regex.IsMatch(ageInput.text.Trim(), @"^\d+$"))
        {
            ShowValidation("Age should contain numbers only.");
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

            panelRect.localScale = Vector3.Lerp(
                startScale,
                endScale,
                Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }

        panelRect.localScale = Vector3.zero;

        validationPanel.SetActive(false);
    }


    IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseManager.Instance != null && DatabaseManager.Instance.IsDatabaseReady());

        LoadUserData();
        SetEditMode(false);

        editButton.onClick.AddListener(OnEditButtonClick);
        logoutButton.onClick.AddListener(OnLogoutButtonClick);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OpenCredits);

        if (closeCreditsButton != null)
            closeCreditsButton.onClick.AddListener(CloseCredits);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (yesExitButton != null)
            yesExitButton.onClick.AddListener(ConfirmLogout);

        if (noExitButton != null)
            noExitButton.onClick.AddListener(CloseExitPanel);

        if (exitPanel != null)
            exitPanel.SetActive(false);

    }

    void LoadUserData()
    {
        if (!DatabaseManager.Instance.HasUser())
            return;

        var user = DatabaseManager.Instance.GetUserData();

        if (user == null)
            return;

        // 🔥 LOAD DATABASE VALUES
        firstNameInput.text = user.FirstName;
        lastNameInput.text = user.LastName;
        genderInput.text = user.Gender;

        // 🔥 AGE DROPDOWN
        // 🔥 AGE INPUT
        ageInput.text = user.Age.ToString();

        // 🔥 UPDATE CHARACTER IMAGE
        SetCharacterImage(user.Gender);
    }

    void SetEditMode(bool enable)
    {
        firstNameInput.interactable = enable;
        lastNameInput.interactable = enable;
        ageInput.interactable = enable;

        // ❌ bawal i-edit gender
        genderInput.interactable = false;

        isEditing = enable;
    }

    void OnEditButtonClick()
    {
        if (!isEditing)
        {
            SetEditMode(true);
        }
        else
        {
            // Lock fields ONLY if save succeeded
            if (SaveUserData())
            {
                SetEditMode(false);
            }
        }
    }

    bool SaveUserData()
    {
        // VALIDATE FIRST
        if (!ValidateInputs())
            return false;

        string first = firstNameInput.text.Trim();
        string last = lastNameInput.text.Trim();

        int age;

        if (!int.TryParse(ageInput.text.Trim(), out age))
        {
            ShowValidation("Age should contain numbers only.");
            return false;
        }

        string gender = genderInput.text;

        DatabaseManager.Instance.InsertUser(first, last, age, gender);

        // UPDATE CHARACTER IMAGE
        SetCharacterImage(gender);

        Debug.Log("User updated!");

        // REFRESH MAIN MENU
        MainMenuUIManager menu = FindObjectOfType<MainMenuUIManager>();

        if (menu != null)
        {
            menu.LoadPlayerData();
        }

        return true;
    }

    void SetCharacterImage(string gender)
    {
        if (gender.ToLower() == "boy")
        {
            charImage.sprite = boySprite;
        }
        else
        {
            charImage.sprite = girlSprite;
        }
    }


    void OnLogoutButtonClick()
    {
        if (exitPanel == null)
            return;

        exitPanel.SetActive(true);

        if (exitCoroutine != null)
            StopCoroutine(exitCoroutine);

        exitCoroutine =
            StartCoroutine(OpenExitAnimation());
    }

    public void OpenCredits()
    {
        Debug.Log("Open Credits Clicked");

        if (creditsPanel == null)
            return;

        creditsPanel.SetActive(true);

        if (creditsCoroutine != null)
            StopCoroutine(creditsCoroutine);

        creditsCoroutine = StartCoroutine(OpenCreditsAnimation());
    }

    public void CloseCredits()
    {
        if (creditsPanel == null)
            return;

        if (creditsCoroutine != null)
            StopCoroutine(creditsCoroutine);

        creditsCoroutine = StartCoroutine(CloseCreditsAnimation());
    }

    IEnumerator OpenCreditsAnimation()
    {
        RectTransform panelRect =
            creditsPanel.GetComponent<RectTransform>();

        panelRect.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < creditsPopupDuration)
        {
            timer += Time.deltaTime;

            float scale =
                Mathf.SmoothStep(0f, 1f, timer / creditsPopupDuration);

            panelRect.localScale =
                new Vector3(scale, scale, scale);

            yield return null;
        }

        panelRect.localScale = Vector3.one;
    }

    IEnumerator CloseCreditsAnimation()
    {
        RectTransform panelRect =
            creditsPanel.GetComponent<RectTransform>();

        float timer = 0f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < creditsPopupDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.SmoothStep(0f, 1f, timer / creditsPopupDuration);

            panelRect.localScale =
                Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        panelRect.localScale = Vector3.zero;

        creditsPanel.SetActive(false);
    }

    public void ConfirmLogout()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void CloseExitPanel()
    {
        if (exitCoroutine != null)
            StopCoroutine(exitCoroutine);

        exitCoroutine =
            StartCoroutine(CloseExitAnimation());
    }

    IEnumerator OpenExitAnimation()
    {
        RectTransform panelRect =
            exitPanel.GetComponent<RectTransform>();

        panelRect.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < exitPopupDuration)
        {
            timer += Time.deltaTime;

            float scale =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    timer / exitPopupDuration);

            panelRect.localScale =
                new Vector3(scale, scale, scale);

            yield return null;
        }

        panelRect.localScale = Vector3.one;
    }

    IEnumerator CloseExitAnimation()
    {
        RectTransform panelRect =
            exitPanel.GetComponent<RectTransform>();

        float timer = 0f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < exitPopupDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    timer / exitPopupDuration);

            panelRect.localScale =
                Vector3.Lerp(
                    startScale,
                    endScale,
                    t);

            yield return null;
        }

        panelRect.localScale = Vector3.zero;

        exitPanel.SetActive(false);
    }

}
