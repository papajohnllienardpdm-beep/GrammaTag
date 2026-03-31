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

        SetCharacter("Boy"); // test muna
        //string gender = DatabaseManager.Instance.GetUserGender();
        //SetCharacter(gender);
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
