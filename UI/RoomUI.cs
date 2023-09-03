using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    public InputField nameInputField;

    public void OnNameInput()
    {
        string name = nameInputField.text;
        PlayerPrefs.SetString("PlayerName", name);
    }

}
