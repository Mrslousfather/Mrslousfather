using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class KillFeed : NetworkBehaviour
{
    public Image TextImage;
    public TMP_Text killFeedText;  // ������ʾ��ɱ֪ͨ���ı����
    public float displayDuration = 3.0f;  // ֪ͨ��ʾ�ĳ���ʱ��

    [ClientRpc]
    public void RpcNotifyKill(string killer, string victim, int killStreak)
    {
        if (killStreak == 1)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n�ɵ���\n" + "<i>" + victim + "</i>";
        }
        else if(killStreak == 2)
        {
            killFeedText.text ="<i>"+killer+"</i>" + "\n���˫ɱ";
        }
        else if(killStreak == 3)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n�����ɵ�������Ү";
        }
        else if(killStreak == 4)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n�ĸ����ĸ��ˣ����度������";
        }
        else if(killStreak == 5)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n����ǿ�ߣ��ֲ���˹��";
        }
        else if(killStreak == 6)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\nɱ����ɱ���˰�������������������";
        }
        else if(killStreak == 7)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\nɱ������Ҳ�������ˣ�������˰ɣ�����";
        }
        else if(killStreak == 8)
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n����������濪�ˣ���������";
        }
        else
        {
            killFeedText.text = "<i>" + killer + "</i>" + "\n������£�����ƭ����ͷ�����۸����ϵ�������ͬ־";
        }
        TextImage.gameObject.SetActive(true);  // ʹ�ı�����ɼ�
        StartCoroutine(HideAfterDelay());
    }
    [ClientRpc]
    public void RpcNotifyTowerKill(string victim)
    {
        killFeedText.text =  "���"+"<i>" + victim + "</i>"+"����ѷ�𣬾�Ȼ���������ɵ���~";
        TextImage.gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }
    [ClientRpc]
    public void RpcNotifySpikeKill(string victim)
    {
        killFeedText.text = "<i>" + victim + "</i>" + "\n���ش���©�ˣ���";
        TextImage.gameObject.SetActive(true);  // ʹ�ı�����ɼ�
        StartCoroutine(HideAfterDelay());
    }
    [ClientRpc]
    public void RpcNotifyFireKill(string victim)
    {
        killFeedText.text = "<i>" + victim + "</i>" + "\n�����������ˣ�";
        TextImage.gameObject.SetActive(true);  // ʹ�ı�����ɼ�
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        TextImage.gameObject.SetActive(false);  // �����ı����
    }
}
