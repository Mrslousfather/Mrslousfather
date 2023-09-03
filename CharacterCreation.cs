using Mirror;
using UnityEngine;
using System.Collections.Generic;
public class CharacterCreation : NetworkBehaviour
{

    private Animator animator;  // 当前 Animator 组件
    public GameObject[] characterPrefabs;
    private GameObject[] characterGameObjects;
    private int selectedIndex = 0;
    private int length;
    private PlayerMovement player;  // 存储连接到当前客户端的玩家对象
  
   
    private void Start()
    {
        // 找到连接到当前客户端的 Player 对象
        player = NetworkClient.connection.identity.GetComponent<PlayerMovement>();
       
    }
    public override void OnStartClient()
    {
        length = characterPrefabs.Length;
        characterGameObjects = new GameObject[length];
        for (int i = 0; i < length; i++)
        {
            characterGameObjects[i] = Instantiate(characterPrefabs[i], transform.position, transform.rotation);
        }
        UpdateCharacterShow();
    }

    void UpdateCharacterShow()
    {
        // 确保 selectedIndex 不会超出数组范围
        if (selectedIndex >= 0 && selectedIndex < length)
        {
            characterGameObjects[selectedIndex].SetActive(true);
            for (int i = 0; i < length; i++)
            {
                if (i != selectedIndex)
                {
                    characterGameObjects[i].SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogWarning("selectedIndex is out of range");
        }
    }


    public void OnNextButtonClick()
    {
        selectedIndex++;
        selectedIndex %= length;
        UpdateCharacterShow();
        
        ChooseCharacter(selectedIndex);
        Debug.Log(selectedIndex);
    }

    public void OnPrevButtonClick()
    {
        selectedIndex--;
        if (selectedIndex == -1)
        {
            selectedIndex = length - 1;
        }
        UpdateCharacterShow();
       
        ChooseCharacter(selectedIndex);
        Debug.Log(selectedIndex);
    }
    public void ChooseCharacter(int index)
    {

        NetworkRoomManagerExt.playerCharacterIndexes[NetworkClient.connection] = index;
    }


   
 }          
