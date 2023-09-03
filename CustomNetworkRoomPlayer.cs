using UnityEngine;
using Mirror;
using System.Collections;

namespace Mirror
{
    /// <summary>
    /// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
    /// <para>The RoomPrefab object of the NetworkRoomManager must have this component on it. This component holds basic room player data required for the room to function. Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Room Player")]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-room-player")]
    public class CustomNetworkRoomPlayer : NetworkBehaviour
    {
        /// <summary>
        /// This flag controls whether the default UI is shown for the room player.
        /// <para>As this UI is rendered using the old GUI system, it is only recommended for testing purposes.</para>
        /// </summary>
        [Tooltip("This flag controls whether the default UI is shown for the room player")]
        public bool showRoomGUI = true;

        [Header("Diagnostics")]

        /// <summary>
        /// Diagnostic flag indicating whether this player is ready for the game to begin.
        /// <para>Invoke CmdChangeReadyState method on the client to set this flag.</para>
        /// <para>When all players are ready to begin, the game will start. This should not be set directly, CmdChangeReadyState should be called on the client to set it on the server.</para>
        /// </summary>
        [Tooltip("Diagnostic flag indicating whether this player is ready for the game to begin")]
        [SyncVar(hook = nameof(ReadyStateChanged))]
        public bool readyToBegin;

        /// <summary>
        /// Diagnostic index of the player, e.g. Player1, Player2, etc.
        /// </summary>
        [Tooltip("Diagnostic index of the player, e.g. Player1, Player2, etc.")]
        [SyncVar(hook = nameof(IndexChanged))]
        public int index;

        #region Unity Callbacks

        /// <summary>
        /// Do not use Start - Override OnStartHost / OnStartClient instead!
        /// </summary>
        public virtual void Start()
        {
            Debug.Log(index);
            if (NetworkManager.singleton is CustomNetworkRoomManager room)
            {
                // NetworkRoomPlayer object must be set to DontDestroyOnLoad along with NetworkRoomManager
                // in server and all clients, otherwise it will be respawned in the game scene which would
                // have undesirable effects.
                if (room.dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);

                room.roomSlots.Add(this);

                if (NetworkServer.active)
                    room.RecalculateRoomPlayerIndices();

                if (NetworkClient.active)
                    room.CallOnClientEnterRoom();
            }
            else Debug.LogError("RoomPlayer could not find a NetworkRoomManager. The RoomPlayer requires a NetworkRoomManager object to function. Make sure that there is one in the scene.");
            // 设置 playerName 为 "Player1"，"Player2" 等
            //Debug.Log("我在" + index + "之后");
            //PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            //if (playerMovement != null)
            //{
            //    playerMovement.playerName = "Player" + (index + 1);
            //    Debug.Log("我在" + index + "之后,不为空");
            //}
            //else
            //{
            //    Debug.Log("我在" + index + "之后,为空");
            //}
        }

        public virtual void OnDisable()
        {
            if (NetworkClient.active && NetworkManager.singleton is CustomNetworkRoomManager room)
            {
                // only need to call this on client as server removes it before object is destroyed
                room.roomSlots.Remove(this);

                room.CallOnClientExitRoom();
            }
        }

        #endregion

        #region Commands

        [Command]
        public void CmdChangeReadyState(bool readyState)
        {
            readyToBegin = readyState;
            CustomNetworkRoomManager room = NetworkManager.singleton as CustomNetworkRoomManager;
            if (room != null)
            {
                room.ReadyStatusChanged();
            }
        }

        #endregion

        #region SyncVar Hooks

        /// <summary>
        /// This is a hook that is invoked on clients when the index changes.
        /// </summary>
        /// <param name="oldIndex">The old index value</param>
        /// <param name="newIndex">The new index value</param>
        public virtual void IndexChanged(int oldIndex, int newIndex) { }

        /// <summary>
        /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
        /// <para>This function is called when the a client player calls CmdChangeReadyState.</para>
        /// </summary>
        /// <param name="newReadyState">New Ready State</param>
        public virtual void ReadyStateChanged(bool oldReadyState, bool newReadyState) { }

        #endregion

        #region Room Client Virtuals

        /// <summary>
        /// This is a hook that is invoked on clients for all room player objects when entering the room.
        /// <para>Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.</para>
        /// </summary>
        public virtual void OnClientEnterRoom() { }

        /// <summary>
        /// This is a hook that is invoked on clients for all room player objects when exiting the room.
        /// </summary>
        public virtual void OnClientExitRoom() { }

        #endregion

        #region Optional UI

        /// <summary>
        /// Render a UI for the room. Override to provide your own UI
        /// </summary>
        public virtual void OnGUI()
        {
            if (!showRoomGUI)
                return;

            CustomNetworkRoomManager room = NetworkManager.singleton as CustomNetworkRoomManager;
            if (room)
            {
                if (!room.showRoomGUI)
                    return;

                if (!Utils.IsSceneActive(room.RoomScene))
                    return;

                DrawPlayerReadyState();
                DrawPlayerReadyButton();
            }
        }

        bool isButtonPressed = false;
        Texture2D buttonNormalImage;
        Texture2D buttonPressedImage;


        void DrawPlayerReadyState()
        {

            // Create a GUIStyle for each button state
            GUIStyle buttonNormalStyle = new GUIStyle();
            buttonNormalStyle.normal.background = buttonNormalImage;
            GUIStyle buttonPressedStyle = new GUIStyle();
            buttonPressedStyle.normal.background = buttonPressedImage;
            
            float playerAreaWidth = 900f; // 设置每个玩家绘制区域的宽度
            float playerAreaHeight = 100f; // 设置每个玩家绘制区域的高度
            float verticalSpacing = 45f; // 设置玩家之间的垂直间距

            // 计算玩家绘制区域的位置
            float playerAreaX = 160f ;
            float playerAreaY = 220f + (index * (playerAreaHeight + verticalSpacing));

            GUILayout.BeginArea(new Rect(playerAreaX, playerAreaY, playerAreaWidth, playerAreaHeight));

            GUILayout.BeginHorizontal(); // 开始水平布局
            //GUILayout.BeginArea(new Rect(158f + (index * 118), 220f, 1200f, 1200f));
            //GUILayout.BeginHorizontal(); // 开始水平布局

            // 玩家编号和Ready/Not Ready标签
            GUIStyle myStyle = new GUIStyle();
            myStyle.normal.textColor = Color.black;
            myStyle.fontSize = 80;

            GUILayout.Label($"Player {index + 1}", myStyle);

            // Add a gap of 30 pixels
            GUILayout.Space(300);

            if (readyToBegin)
            {
                GUILayout.Label("√", myStyle);
            }
            else
            {
                GUILayout.Label("×", myStyle);
            }

            GUILayout.EndHorizontal(); // 结束水平布局

            // Add a gap of 3 pixels
            GUILayout.Space(30);

            GUILayout.EndArea();
        }



        IEnumerator HandleButtonClick()
        {
            isButtonPressed = true;
            yield return new WaitForSeconds(0.3f);
            isButtonPressed = false;
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }



        IEnumerator HandleReadyButton()
        {
            yield return new WaitForSeconds(0.3f);
            CmdChangeReadyState(!readyToBegin);
        }

        void DrawPlayerReadyButton()
        {
            if (NetworkClient.active && isLocalPlayer)
            {
                // Load the images from the Resources folder
                Texture2D buttonReadyNormalImage = Resources.Load<Texture2D>("ART/UI/Button/B_Ready1");
                Texture2D buttonReadyPressedImage = Resources.Load<Texture2D>("ART/UI/Button/B_Ready2");
                Texture2D buttonCancelNormalImage = Resources.Load<Texture2D>("ART/UI/Button/B_Cancel1");
                Texture2D buttonCancelPressedImage = Resources.Load<Texture2D>("ART/UI/Button/B_Cancel2");

                // Create a GUIStyle for each button state
                GUIStyle buttonReadyNormalStyle = new GUIStyle();
                buttonReadyNormalStyle.normal.background = buttonReadyNormalImage;
                GUIStyle buttonReadyPressedStyle = new GUIStyle();
                buttonReadyPressedStyle.normal.background = buttonReadyPressedImage;
                GUIStyle buttonCancelNormalStyle = new GUIStyle();
                buttonCancelNormalStyle.normal.background = buttonCancelNormalImage;
                GUIStyle buttonCancelPressedStyle = new GUIStyle();
                buttonCancelPressedStyle.normal.background = buttonCancelPressedImage;

                GUILayout.BeginArea(new Rect(100f, 800f, 250, 250f));

                float buttonSize = 250f;  // Set a fixed size for the button

                // Create a Rect for the button
                Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(readyToBegin ? "Cancel" : "Ready"), isButtonPressed ? (readyToBegin ? buttonCancelPressedStyle : buttonReadyPressedStyle) : (readyToBegin ? buttonCancelNormalStyle : buttonReadyNormalStyle), GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));

                // Check for mouse events
                if (Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition))
                {
                    isButtonPressed = true;
                    StartCoroutine(HandleReadyButton());
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    isButtonPressed = false;
                }

                // Draw the button
                GUI.Button(buttonRect, readyToBegin ? " " : " ", isButtonPressed ? (readyToBegin ? buttonCancelPressedStyle : buttonReadyPressedStyle) : (readyToBegin ? buttonCancelNormalStyle : buttonReadyNormalStyle));

                GUILayout.EndArea();
            }
        }



        #endregion
    }
}
