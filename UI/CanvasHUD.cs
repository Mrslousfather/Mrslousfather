using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CanvasHUD : MonoBehaviour
{
    public Button buttonHost;
    public Button buttonClinet;

    public void Start()
    {
        buttonHost.onClick.AddListener(ButtonHost); //绑定函数
        buttonClinet.onClick.AddListener(ButtonClinet); //绑定函数
    }
    public void ButtonHost()
    {
        NetworkManager.singleton.StartHost();
    }
    public void ButtonClinet()
    {
        NetworkManager.singleton.StartClient();
    }

}