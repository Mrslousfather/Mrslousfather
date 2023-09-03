using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fox : MonoBehaviour
{
    public Animator Animator;
    void Start()
    {
        Animator = GetComponent<Animator>();
        
    }

   IEnumerator fox1()
    {
        yield return new WaitForSeconds(2f); 
        Animator.SetBool("fox", true);
    }
    void Update()
    {
        StartCoroutine(fox1());
    }
}
