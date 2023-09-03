//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Mirror;
//public class CameraFollow : NetworkBehaviour
//{
//    public Transform target;
//    public float smoothing;
//    public Vector2 minPostion;
//    public Vector2 maxPostion;
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void LateUpdate()
//    {
//        if (!isLocalPlayer)
//            return;
//        if (target !=null)
//        {

//            GameObject player = GameObject.FindGameObjectWithTag("Player");
//            if (player != null)
//                target = player.transform;
//            return;
//            if (transform.position != target.position)
//            {
//                Vector3 targetpos = target.position;
//                targetpos.x = Mathf.Clamp(targetpos.x, minPostion.x, maxPostion.x);
//                targetpos.y = Mathf.Clamp(targetpos.y, minPostion.y, maxPostion.y);
//                transform.position = Vector3.Lerp(transform.position, targetpos, smoothing);
//            }
//        }
//    }
//     public void SetCamPosLimit(Vector2 minpos,Vector2 maxPos)
//    {
//        minPostion = minpos;
//        maxPostion = maxPos;
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class CameraFollow : NetworkBehaviour
{
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Vector2 minPosition;
    public Vector2 maxPosition;
    private Transform target;
    private bool isMouseVisible = false;
   
    private void Awake()
    {
       
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    void LateUpdate()
    {
        if (target == null)
        {
            if (NetworkClient.connection != null && NetworkClient.connection.identity != null)
                target = NetworkClient.connection.identity.transform;
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x); 
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosition.y, maxPosition.y);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
   
}
