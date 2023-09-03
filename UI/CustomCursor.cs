using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D normalCursor; // 未点击时的光标纹理
    public Texture2D clickedCursor; // 点击时的光标纹理
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        // 设置默认光标
        Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
    }

    void Update()
    {
        // 如果按下鼠标，切换到点击光标
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(clickedCursor, hotSpot, cursorMode);
        }

        // 如果释放鼠标，恢复到正常光标
        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
        }
    }
}
