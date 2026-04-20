using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // ADD THIS

public class LoginManager : MonoBehaviour
{
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_Dropdown ageDropdown;
    public Button startButton;

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

    public void StartGame()
    {
        if (isSaving) return; // 🔥 PREVENT DOUBLE CALL
        isSaving = true;

        if (startButton != null)
            startButton.interactable = false;

        if (firstNameInput.text == "" || lastNameInput.text == "" || playerSex == "")
        {
            Debug.Log("Complete your profile first!");

            if (startButton != null)
                startButton.interactable = true;

            isSaving = false;
            return;
        }

        int age = int.Parse(ageDropdown.options[ageDropdown.value].text);

        DatabaseManager.Instance.InsertUser
        (
            firstNameInput.text,
            lastNameInput.text,
            age,
            playerSex
        );

        // 👉 SAVE GENDER TEMP (for cutscene)
        PlayerPrefs.SetString("SelectedGender", playerSex);
        PlayerPrefs.Save();

        StartCoroutine(LoadSceneDelayed());
    }

    IEnumerator LoadSceneDelayed()
    {
        yield return new WaitForSeconds(0.3f);

        PlayerPrefs.SetString("SelectedGender", playerSex); // 👈 ADD THIS
        PlayerPrefs.Save();

        SceneManager.LoadScene("CutScene1");
    }
}