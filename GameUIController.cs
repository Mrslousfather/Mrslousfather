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

    
    // ����ĸ��µ��ı�������ʾ����
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
    private bool gameHasEnded = false; // ��Ӵ˱�����ȷ����Ϸ�����߼�ִֻ��һ��
    private bool canChangerState;
    private float playAnTime = 3f;

    public GameObject menuPanel;
    bool isMenuShowed = false;
    private void Start()
    {
        bgmController = GetComponent<musicbgm>();
        // ����Ϸ��ʼʱ����ʼ����ʱ
        timerIsRunning = false;
        endingPanel.SetActive(false);
        gameHasEnded = false;
        canChangerState = true; ;
        // ��ʼ����ʱЭ��
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
            // ����ʽ��ʼʱ����ʼ����ʱ
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
        txtCountDown.gameObject.SetActive(true);  // ȷ������ʱ�ı��ǿɼ���

        // ��3��ʼ����ʱ
        for (int i = 3; i > 0; i--)
        {
            txtCountDown.text = i.ToString();
            yield return new WaitForSeconds(1);  // �ȴ�1��
        }

        // ����ʱ��������ʾ"FIGHT!"
        txtCountDown.text = "FIGHT!";
        canBegin = true;  // ������Ϸ��ʼ

        yield return new WaitForSeconds(1f);  // �ٵȴ�1��

        txtCountDown.gameObject.SetActive(false);  // ���ص���ʱ�ı�
    }
    void DisplayIntTime(float timeToDisplay)
    {
        timeText.text = string.Format(Mathf.Round(timeToDisplay).ToString() + "S");

    }
    void DisplayFloatTime(float timeToDisplay)
    {
        timeText.text = string.Format("{0:F3}S", timeRemaining);
    }

    public IEnumerator ShowPanel()//����ҳ��
    {
        yield return new WaitForSeconds(playAnTime);
        endingPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        CountKillNum();//�����ɱ��
        yield return new WaitForSeconds(5f);
        //��ʾSortingPanel,����endingPanel
        endingPanel.SetActive(false);
        sortingPanel.SetActive(true);
        StartCoroutine(BackToRoom());//�ص�����
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
            // ������ٶ�����Ϊ0
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // ֹͣ���ж���
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
            // ��ֹ����ǹ֧����
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
            // ֹͣ���ж���
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
            }
            // ��ֹ����ǹ֧����
            foreach (Gun gun in player.GetComponentsInChildren<Gun>())
            {
                gun.canFire = true;
            }
        }
    }
    void CountKillNum()
    {
        // �ҳ��÷���ߵ����
        PlayerMovement topPlayer = null;
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            if (topPlayer == null || player.score > topPlayer.score)
            {
                topPlayer = player;
            }
        }

        // ����endTitle���ı�
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            if (player == topPlayer)
            {
                player.endTitle.text = "��ϲ���Ϊ�����գ�";
            }
            else
            {
                player.endTitle.text = "����ʳ��...";
            }
        }
        foreach (PlayerMovement player in PlayerMovement.allPlayers)
        {
            player.HideNonCorrespondingEndTitle();
        }

        if (int.Parse(nowScore.text) > 20)
        {
            killNumber.text = "������" + nowScore.text + " ������";
        }
        else if (int.Parse(nowScore.text) > 10)
        {
            killNumber.text = "����" + nowScore.text + " ����";
        }
        else if (int.Parse(nowScore.text) > 5)
        {
            killNumber.text = "��" + nowScore.text + " ��";
        }
        else
        {
            killNumber.text = nowScore.text;
        }

        
    }
    void Sorting()
    {
        // ����һ���б�洢������Ҳ�����
        List<PlayerMovement> playerScoreList = new List<PlayerMovement>(PlayerMovement.allPlayers);
        playerScoreList.Sort((p1, p2) => p2.score.CompareTo(p1.score));

        // ��������������ֺͻ�ɱ�����µ� UI

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
