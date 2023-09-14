using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private GameObject playerObject;
    private bool playerAsinged = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Player")
        {
            playerObject = other.gameObject;
            playerAsinged = true;
        }
    }

    public GameObject GetDetectedPlayer()
    {
        return playerObject;
    }

    public bool IsPlayerAsinged()
    {
        return playerAsinged;
    }
}
