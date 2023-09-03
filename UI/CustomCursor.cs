using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D normalCursor; // δ���ʱ�Ĺ������
    public Texture2D clickedCursor; // ���ʱ�Ĺ������
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        // ����Ĭ�Ϲ��
        Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
    }

    void Update()
    {
        // ���������꣬�л���������
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(clickedCursor, hotSpot, cursorMode);
        }

        // ����ͷ���꣬�ָ����������
        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
        }
    }
}
