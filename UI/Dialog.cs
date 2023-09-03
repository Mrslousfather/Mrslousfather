using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text dialogText;
    public float delayBetweenCharacters = 0.05f;
    public float delayBetweenDialogs = 1.0f; // 两句对话之间的延迟
    private bool isTyping = false;
    public string[] dialogues; // 存储所有的对话
    private int currentDialogIndex = 0; // 当前正在显示的对话的索引

    // 开始时自动开始对话序列
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
        dialogText.text = ""; // 清空文本
        foreach (char c in text)
        {
            dialogText.text += c; // 逐个添加字符
            yield return new WaitForSeconds(delayBetweenCharacters);
        }
        isTyping = false;
        
        // 检查是否还有更多对话
        if (currentDialogIndex < dialogues.Length - 1)
        {
            currentDialogIndex++; // 增加索引以准备下一句对话
            yield return new WaitForSeconds(delayBetweenDialogs); // 等待两句对话之间的延迟
            DisplayText(dialogues[currentDialogIndex]); // 显示下一句对话
        }
    }
}
