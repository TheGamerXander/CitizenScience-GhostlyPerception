using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float countDown;

    private void Start()
    {
        Destroy(gameObject, countDown);
    }
}
