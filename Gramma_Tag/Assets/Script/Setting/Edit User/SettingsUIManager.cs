using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_Dropdown ageDropdown;
    public TMP_InputField genderInput;

    [Header("Button")]
    public Button editButton;
    public Button logoutButton;

    [Header("Character Image")]
    public Image charImage;
    public Sprite boySprite;
    public Sprite girlSprite;

    private bool isEditing = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseManager.Instance != null && DatabaseManager.Instance.IsDatabaseReady());

        LoadUserData();
        SetEditMode(false);

        editButton.onClick.AddListener(OnEditButtonClick);
        logoutButton.onClick.AddListener(OnLogoutButtonClick);
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
        for (int i = 0; i < ageDropdown.options.Count; i++)
        {
            if (ageDropdown.options[i].text == user.Age.ToString())
            {
                ageDropdown.value = i;
                break;
            }
        }

        // 🔥 UPDATE CHARACTER IMAGE
        SetCharacterImage(user.Gender);
    }

    void SetEditMode(bool enable)
    {
        firstNameInput.interactable = enable;
        lastNameInput.interactable = enable;
        ageDropdown.interactable = enable;

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
            SaveUserData();
            SetEditMode(false);
        }
    }

    void SaveUserData()
    {
        string first = firstNameInput.text;
        string last = lastNameInput.text;
        int age = int.Parse(ageDropdown.options[ageDropdown.value].text);
        string gender = genderInput.text;

        DatabaseManager.Instance.InsertUser(first, last, age, gender);

        // 🔥 update character image after save
        SetCharacterImage(gender);

        Debug.Log("User updated!");

        // ✅ REFRESH MAIN MENU NAME
        MainMenuUIManager menu = FindObjectOfType<MainMenuUIManager>();

        if (menu != null)
        {
            menu.LoadPlayerData();
        }

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
        Debug.Log("Logout button clicked");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
