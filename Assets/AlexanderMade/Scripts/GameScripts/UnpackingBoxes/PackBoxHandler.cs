using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackBoxHandler : MonoBehaviour
{
    [SerializeField] InteractiveTruckHandler truckHandler;
    [SerializeField] GameObject unpackedBox;

    public void MarkBoxAsUnpacked()
    {
        truckHandler.SetBoxUnpacked(GetComponent<Animator>(), gameObject);
        unpackedBox.SetActive(true);
        Destroy(gameObject);
    }
}
