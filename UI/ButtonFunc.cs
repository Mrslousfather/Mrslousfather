using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunc : MonoBehaviour
{

    public GameObject black;
    public Button buttonQuit;
    public Button buttonHelp;
    public GameObject helpPanel;
    public Button quitHelpPanel;
    private void Start()
    {
        // 初始化时，设置图片为不可见
  
        helpPanel.gameObject.SetActive(false);
        // 添加按钮点击事件
        buttonQuit.onClick.AddListener(QuitGame);
        buttonHelp.onClick.AddListener(ShowHelpPanel);
        quitHelpPanel.onClick.AddListener(CloseHelpPanel);
        StartCoroutine(blackflip());
    }
    IEnumerator blackflip()
    {
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(black);

    }
    // 切换图片的可见性

    private void QuitGame()
    {
        Application.Quit();
    }
    private void ShowHelpPanel()
    {
        helpPanel.gameObject.SetActive(true);
    }
    private void CloseHelpPanel()
    {
        helpPanel.gameObject.SetActive(false);
    }
}
