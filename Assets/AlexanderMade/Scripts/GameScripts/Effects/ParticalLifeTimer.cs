using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticalLifeTimer : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5.0f;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
