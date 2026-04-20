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
    }

    void LoadUserData()
    {
        if (!DatabaseManager.Instance.HasUser()) return;

        var db = DatabaseManager.Instance;

        string gender = db.GetUserGender();

        firstNameInput.text = db.GetPlayerName();
        lastNameInput.text = "";
        genderInput.text = gender;
        ageDropdown.value = 0;

        SetCharacterImage(gender);
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
}
