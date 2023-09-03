using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text dialogText;
    public float delayBetweenCharacters = 0.05f;
    public float delayBetweenDialogs = 1.0f; // ����Ի�֮����ӳ�
    private bool isTyping = false;
    public string[] dialogues; // �洢���еĶԻ�
    private int currentDialogIndex = 0; // ��ǰ������ʾ�ĶԻ�������

    // ��ʼʱ�Զ���ʼ�Ի�����
    private void Start()
    {
        if (dialogues.Length > 0)
        {
            DisplayText(dialogues[currentDialogIndex]);
        }
    }

    public void DisplayText(string text)
    {
        if (!isTyping)
        {
            StartCoroutine(TypeText(text));
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogText.text = ""; // ����ı�
        foreach (char c in text)
        {
            dialogText.text += c; // �������ַ�
            yield return new WaitForSeconds(delayBetweenCharacters);
        }
        isTyping = false;
        
        // ����Ƿ��и���Ի�
        if (currentDialogIndex < dialogues.Length - 1)
        {
            currentDialogIndex++; // ����������׼����һ��Ի�
            yield return new WaitForSeconds(delayBetweenDialogs); // �ȴ�����Ի�֮����ӳ�
            DisplayText(dialogues[currentDialogIndex]); // ��ʾ��һ��Ի�
        }
    }
}
