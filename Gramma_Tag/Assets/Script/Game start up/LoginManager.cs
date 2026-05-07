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
    public TMP_Dropdown ageDropdown;
    public Button startButton;

    public GameObject validationPanel;
    public TMP_Text validationText;

    private string playerSex = "";

    public void SelectGirl()
    {
        playerSex = "Girl";
    }

    public void SelectBoy()
    {
        playerSex = "Boy";
    }

    private bool isSaving = false;

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

        // 🔥 AGE CHECK
        if (ageDropdown.value == 0)
        {
            ShowValidation("Please select your age.");
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
            validationPanel.SetActive(true);

        if (validationText != null)
            validationText.text = message;
    }

    public void CloseValidation()
    {
        if (validationPanel != null)
            validationPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (isSaving) return; // 🔥 PREVENT DOUBLE CALL
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

        int age = int.Parse(ageDropdown.options[ageDropdown.value].text);

        StartCoroutine(WaitAndSaveUser(age));
    }

    IEnumerator WaitAndSaveUser(int age)
    {
        // 🔥 WAIT UNTIL DATABASE IS READY
        yield return new WaitUntil(() =>
            DatabaseManager.Instance != null &&
            DatabaseManager.Instance.IsDatabaseReady()
        );

        Debug.Log("✅ DB READY SA LOGIN");

        DatabaseManager.Instance.InsertUser(
            firstNameInput.text,
            lastNameInput.text,
            age,
            playerSex
        );

        PlayerPrefs.SetString("SelectedGender", playerSex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("CutScene1");
    }

    
}