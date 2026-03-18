using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_Dropdown ageDropdown;

    private string playerSex = "";

    public void SelectGirl()
    {
        playerSex = "Girl";
    }

    public void SelectBoy()
    {
        playerSex = "Boy";
    }

    public void StartGame()
    {
        if (firstNameInput.text == "" || lastNameInput.text == "" || playerSex == "")
        {
            Debug.Log("Complete your profile first!");
            return;
        }

        int age = int.Parse(ageDropdown.options[ageDropdown.value].text);

        DatabaseManager.Instance.InsertUser(
            firstNameInput.text,
            lastNameInput.text,
            age,
            playerSex
        );

        SceneManager.LoadScene("MainMenu");
    }
}