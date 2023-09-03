using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class randomname : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField text;
    public string[] name1;
    private int index;
    private void Start()
    {
        text = GetComponent<InputField>();
        RandomName();


    }
    public void RandomName()
    {
        index = Random.Range(0, name1.Length);
        text.text = name1[index];
        string name = text.text;
        PlayerPrefs.SetString("PlayerName", name);
    }

}
