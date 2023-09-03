using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class disactive : MonoBehaviour
{
    
    void Start()
    {
       StartCoroutine(Startactive());
   
}

    private IEnumerator Startactive()
    {
        yield return new WaitForSeconds(18);
        SceneManager.LoadScene("MainScene");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
         {
            SceneManager.LoadScene("MainScene");
        }
    }
}
