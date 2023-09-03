using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Benchmark;
using UnityEngine.InputSystem;
public class flip : NetworkBehaviour
{

    protected Vector2 mousePos;
   
    protected float flipY;
    protected Vector2 direction;
    void Start()
    {
        
        flipY = transform.localScale.y;

    }
    public void Update()
    {

        if (isLocalPlayer)
        {


            if (mousePos.x < transform.position.x)
                transform.localScale = new Vector3(flipY, -flipY, 1);
            else
                transform.localScale = new Vector3(flipY, flipY, 1);
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
            transform.right = direction;


        }

    }
}
