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
        // ��ʼ��ʱ������ͼƬΪ���ɼ�
  
        helpPanel.gameObject.SetActive(false);
        // ��Ӱ�ť����¼�
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
    // �л�ͼƬ�Ŀɼ���

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
