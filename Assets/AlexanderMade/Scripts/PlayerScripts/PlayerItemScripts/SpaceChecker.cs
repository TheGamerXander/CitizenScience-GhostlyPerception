using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceChecker : MonoBehaviour
{
    public bool spaceFree = true;

    private void OnTriggerEnter(Collider other)
    {

        spaceFree = false;
    }

    private void OnTriggerExit(Collider other)
    {

        spaceFree = true;
    }
}
