using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public Image characterImage;

    public List<CharacterData> characterList;


    void Start()
    {
        StartCoroutine(LoadCharacter());
    }

    IEnumerator LoadCharacter()
    {
        // wait until DatabaseManager exists
        while (DatabaseManager.Instance == null)
            yield return null;

        // wait until DB ready
        while (!DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        string gender = "";

        try
        {
            gender = DatabaseManager.Instance.GetUserGender();
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetUserGender ERROR: " + e);
        }

        SetCharacter(gender);
    }

    public void SetCharacter(string genderFromDB)
    {
        foreach (CharacterData data in characterList)
        {
            if (data.gender == genderFromDB)
            {
                characterImage.sprite = data.characterSprite;
                return;
            }
        }

        Debug.LogWarning("No character found for gender: " + genderFromDB);
    }
}
