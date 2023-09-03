using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFlow : MonoBehaviour
{
    public float speed = 5f;
    public float height = 0.01f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 v = startPos;
        v.y += height * Mathf.Sin(Time.time * speed);
        transform.position = v;
    }
}
