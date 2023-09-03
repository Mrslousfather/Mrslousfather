using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScore : NetworkBehaviour
{
    // 玩家的积分
    [SyncVar(hook = nameof(OnScoreChanged))]
    public int score = 0;

    // 用于在玩家积分发生变化时进行处理
    private void OnScoreChanged(int oldScore, int newScore)
    {
        Debug.Log($"Player's score changed: {oldScore} -> {newScore}");
        // 在这里可以做一些处理，比如更新UI显示等
    }

    // 增加积分的方法
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
}








