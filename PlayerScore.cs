using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScore : NetworkBehaviour
{
    // ��ҵĻ���
    [SyncVar(hook = nameof(OnScoreChanged))]
    public int score = 0;

    // ��������һ��ַ����仯ʱ���д���
    private void OnScoreChanged(int oldScore, int newScore)
    {
        Debug.Log($"Player's score changed: {oldScore} -> {newScore}");
        // �����������һЩ�����������UI��ʾ��
    }

    // ���ӻ��ֵķ���
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
}








