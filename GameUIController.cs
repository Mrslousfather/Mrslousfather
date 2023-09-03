using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public musicbgm bgmController;

    public float timeRemaining;
    public bool timerIsRunning = false;
    public Text timeText;
    public GameObject endingPanel;
    public GameObject sortingPanel;
    public GameObject animationPanel;
    public Text nowScore;
    public Text killNumber;

    
    // 添加四个新的文本用于显示排名
    public Text firstPlace;
    public Text secondPlace;
    public Text thirdPlace;
    public Text fourthPlace;

    public Text firstKillPlace;
    public Text secondKillPlace;
    public Text thirdKillPlace;
    public Text fourthKillPlace;

    public Text firstRank;
    public Text secondRank;
    public Text thirdRank;
    public Text fourthRank;
    public bool canBegin=false;
    public Text txtCountDown;
    private bool gameHasEnded = false; // 添加此变量以确保游戏结束逻辑只执行一次
    private bool canChangerState;
    private float playAnTime = 3f;

    public GameObject menuPanel;
    bool isMenuShowed = false;
    private void Start()
    {
        bgmController = GetComponent<musicbgm>();
        // 在游戏开始时，开始倒计时
        timerIsRunning = false;
        endingPanel.SetActive(false);
        gameHasEnded = false;
        canChangerState = true; ;
        // 开始倒计时协程
        StartCoroutine(CountdownCoroutine());
    }

    private void Update()
    {
        if (!isMenuShowed && Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(true);
            isMenuShowed = true;
        }
        else if (isMenuShowed && Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(false);
            isMenuShowed = false;
        }

       
        if (canBegin)
        {
            StartPlayer();
            // 在正式开始时，开始倒计时
            timerIsRunning = true;
            bgmController.PlayMusic();
        }
        else
        {          
            StopPlayer();          
            return;
        }

        
        if (timerIsRunning)
        {
            if (timeRemaining > 10)
            {
                timeRemaining -= Time.deltaTime;
                DisplayIntTime(timeRemaining);
            }
            else if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayFloatTime(timeRemaining);
            }
            else
            {
                bgmController.StopMusic();
                StopPlayer();
                if (canChangerState)
                {               
                    gameHasEnded = true;
                    canChangerState = false;
                }
                if(gameHasEnded)
                {
                    timeRemaining = 0;
                    timerIsRunning = false;
                    timeText.gameObject.SetActive(false);
                    StartCoroutine(playAnimation());
                    StartCoroutine(ShowPanel());
                }      
            }
        }
    }
    IEnumerator playAnimation()
    {
        gameHasEnded = false; 
        animationPanel.SetActive(true);
        yield return new WaitForSeconds(playAnTime);
        animationPanel.SetActive(false);
    }
    IEnumerator CountdownCoroutine()
    {
        txtCountDown.gameObject.SetActive(true);  // 确保倒计时文本是可见的

        // 从3开始倒计时
        for (int i = 3; i > 0; i--)
        {
            txtCountDown.text = i.ToString();
            yield return new WaitForSeconds(1);  // 等待1秒
        }

        // 倒计时结束，显示"FIGHT!"
        txtCountDown.text = "FIGHT!";
        canBegin = true;  // 允许游戏开始

        yield return new WaitForSeconds(1f);  // 再等待1秒

        txtCountDown.gameObject.SetActive(false);  // 隐藏倒计时文本
    }
    void DisplayIntTime(float timeToDisplay)
    {
        timeText.text = string.Format(Mathf.Round(timeToDisplay).ToString() + "S");

    }
    void DisplayFloatTime(float timeToDisplay)
    {
        timeText.text = string.Format("{0:F3}S", timeRemaining);
    }

    public IEnumerator ShowPanel()//结束页面
    {
        yield return new WaitForSeconds(playAnTime);
        endingPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        CountKillNum();//计算击杀数
        yield return new WaitForSeconds(5f);
        //显示SortingPanel,隐藏endingPanel
        endingPanel.SetActive(false);
        sortingPanel.SetActive(true);
        StartCoroutine(BackToRoom());//回到房间
        Sorting();
    }
    IEnumerator BackToRoom()
    {
        DestroyAllRoomPlayers();
        yield return new WaitForSeconds(6f);
        NetworkRoomManagerExt.singleton.ServerChangeScene("RoomScene");


    }
    void DestroyAllRoomPlayers()
    {
        GameObject[] roomPlayers = GameObject.FindGameObjectsWithTag("RoomPlayer");
        foreach (GameObject roomPlayer in roomPlayers)
        {
            Destroy(roomPlayer);
        }
    }
    void StopPlayer()
    {
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            player.canMove = false;
            // 将玩家速度设置为0
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // 停止所有动画
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
            // 禁止所有枪支开火
            foreach (Gun gun in player.GetComponentsInChildren<Gun>())
            {
                gun.canFire = false;
            }
        }
    }
    void StartPlayer()
    {
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            player.canMove = true;
            // 停止所有动画
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
            }
            // 禁止所有枪支开火
            foreach (Gun gun in player.GetComponentsInChildren<Gun>())
            {
                gun.canFire = true;
            }
        }
    }
    void CountKillNum()
    {
        // 找出得分最高的玩家
        PlayerMovement topPlayer = null;
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            if (topPlayer == null || player.score > topPlayer.score)
            {
                topPlayer = player;
            }
        }

        // 设置endTitle的文本
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            if (player == topPlayer)
            {
                player.endTitle.text = "恭喜你成为了宰粽！";
            }
            else
            {
                player.endTitle.text = "败者食尘...";
            }
        }
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            player.HideNonCorrespondingEndTitle();
        }

        if (int.Parse(nowScore.text) > 20)
        {
            killNumber.text = "！！！" + nowScore.text + " ！！！";
        }
        else if (int.Parse(nowScore.text) > 10)
        {
            killNumber.text = "！！" + nowScore.text + " ！！";
        }
        else if (int.Parse(nowScore.text) > 5)
        {
            killNumber.text = "！" + nowScore.text + " ！";
        }
        else
        {
            killNumber.text = nowScore.text;
        }

        
    }
    void Sorting()
    {
        // 创建一个列表存储所有玩家并排序
        List<PlayerMovement> playerScoreList = new List<PlayerMovement>(PlayerMovement.allPlayers);
        playerScoreList.Sort((p1, p2) => p2.score.CompareTo(p1.score));

        // 将排序后的玩家名字和击杀数更新到 UI

        if (playerScoreList.Count > 0)
        {
            firstPlace.text = playerScoreList[0].playerName;
            firstKillPlace.text = (playerScoreList[0].score).ToString();
            firstRank.gameObject.SetActive(true);
            if (playerScoreList.Count > 1)
            {
                secondPlace.text =   playerScoreList[1].playerName ;
                secondKillPlace.text = (playerScoreList[1].score).ToString();
                secondRank.gameObject.SetActive(true);
                if (playerScoreList.Count > 2)
                {
                    thirdPlace.text = playerScoreList[2].playerName;
                    thirdKillPlace.text = (playerScoreList[2].score).ToString();
                    thirdRank.gameObject.SetActive(true);
                    if (playerScoreList.Count > 3)
                    {
                        fourthPlace.text =  playerScoreList[3].playerName ;
                        fourthKillPlace.text = (playerScoreList[3].score).ToString();
                        fourthRank.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
