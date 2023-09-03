using Mirror;
using UnityEngine;
using System.Collections.Generic;
public class CharacterCreation : NetworkBehaviour
{

    private Animator animator;  // ��ǰ Animator ���
    public GameObject[] characterPrefabs;
    private GameObject[] characterGameObjects;
    private int selectedIndex = 0;
    private int length;
    private PlayerMovement player;  // �洢���ӵ���ǰ�ͻ��˵���Ҷ���
  
   
    private void Start()
    {
        // �ҵ����ӵ���ǰ�ͻ��˵� Player ����
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
        // ȷ�� selectedIndex ���ᳬ�����鷶Χ
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
