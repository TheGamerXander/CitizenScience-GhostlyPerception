using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDeactivator : MonoBehaviour
{
    public float totalTime = 5.0f;
    private float currentTime = 5.0f;

    private void Start()
    {
        currentTime = totalTime;       
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;          
        }
        else
        {
            currentTime = totalTime;
            gameObject.SetActive(false);    
        }
    }
}
