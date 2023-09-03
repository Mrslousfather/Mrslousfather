using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class KillFeed : NetworkBehaviour
{
    public Image TextImage;
    public TMP_Text killFeedText;  // 用于显示击杀通知的文本组件
    public float displayDuration = 3.0f;  // 通知显示的持续时间

    [ClientRpc]
    public void RpcNotifyKill(string killer, string victim, int killStreak)
    {
        if (killStreak == 1)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n干掉了\n" + "<i>" + victim + "</i>";
        }
        else if(killStreak == 2)
        {
            killFeedText.text ="<i>"+killer+"</i>" + "\n完成双杀";
        }
        else if(killStreak == 3)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n连续干掉了三个耶";
        }
        else if(killStreak == 4)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n四个！四个了！好腻害！！！";
        }
        else if(killStreak == 5)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n斗宗强者，恐怖如斯！";
        }
        else if(killStreak == 6)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n杀疯了杀疯了啊啊啊啊啊！！！！！";
        }
        else if(killStreak == 7)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n杀多少我也数不清了，这货开了吧？？？";
        }
        else if(killStreak == 8)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n还在输出？真开了？？？？？";
        }
        else
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n不讲武德，又来骗！来头吸！欺负场上的其他老同志";
        }
        TextImage.gameObject.SetActive(true);  // 使文本组件可见
        StartCoroutine(HideAfterDelay());
    }
    [ClientRpc]
    public void RpcNotifyTowerKill(string victim)
    {
        killFeedText.text =  "这个"+"<i>" + victim + "</i>"+"真是逊吼，竟然被防御塔干掉了~";
        TextImage.gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }
    [ClientRpc]
    public void RpcNotifySpikeKill(string victim)
    {
        killFeedText.text = "<i>" + victim + "</i>" + "\n被地刺扎漏了！！";
        TextImage.gameObject.SetActive(true);  // 使文本组件可见
        StartCoroutine(HideAfterDelay());
    }
    [ClientRpc]
    public void RpcNotifyFireKill(string victim)
    {
        killFeedText.text = "<i>" + victim + "</i>" + "\n被火焰烧死了！";
        TextImage.gameObject.SetActive(true);  // 使文本组件可见
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        TextImage.gameObject.SetActive(false);  // 隐藏文本组件
    }
}
