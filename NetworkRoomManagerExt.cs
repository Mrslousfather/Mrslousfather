using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class NetworkRoomManagerExt : CustomNetworkRoomManager
{
    public static Dictionary<NetworkConnection, int> playerCharacterIndexes = new Dictionary<NetworkConnection, int>();
    public RuntimeAnimatorController[] characterControllers;
    bool showStartButton;

    private Animator animator;  // 当前 Animator 组件
    public override void OnRoomServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        #if UNITY_SERVER
        base.OnRoomServerPlayersReady();
        #else
        showStartButton = true;
        #endif
    }
    private bool isButtonPressed = false;

    IEnumerator HandleStartGameButton()
    {
        yield return new WaitForSeconds(0.3f);
        showStartButton = false;
        ServerChangeScene(GameplayScene);
    }

    public override void OnGUI()
    {
        base.OnGUI();

        // Load the images from the Resources folder
        Texture2D buttonNormalImage = Resources.Load<Texture2D>("ART/UI/Button/B_Start_b1");
        Texture2D buttonPressedImage = Resources.Load<Texture2D>("ART/UI/Button/B_Start_b2");

        // Create a GUIStyle for each button state
        GUIStyle buttonNormalStyle = new GUIStyle();
        buttonNormalStyle.normal.background = buttonNormalImage;
        GUIStyle buttonPressedStyle = new GUIStyle();
        buttonPressedStyle.normal.background = buttonPressedImage;

        if (allPlayersReady && showStartButton)
        {
            // Create a Rect for the button
            Rect buttonRect = new Rect(750, 800, 250, 250);

            // Check for mouse events
            if (Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition))
            {
                isButtonPressed = true;
                StartCoroutine(HandleStartGameButton());
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isButtonPressed = false;
            }

            // Draw the button
            GUI.Button(buttonRect, " ", isButtonPressed ? buttonPressedStyle : buttonNormalStyle);
        }
       
    }

    


}



