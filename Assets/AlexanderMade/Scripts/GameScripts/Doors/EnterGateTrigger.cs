using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterGateTrigger : MonoBehaviour
{
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private GhostGameManager gameManager;
    private bool playerDetected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && !playerDetected)
        {
            gameManager.ShowInteractIcon();
            playerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player" && playerDetected)
        {
            gameManager.HideInteractIcon();
            playerDetected = false;
        }
    }

    private void Update()
    {
        if (playerDetected)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!gameManager.GetPlayerItemsAcquired())
                {
                    gameManager.HideInteractIcon();
                    gameManager.StartDialogue("LockGetItems");
                }
                else
                {
                    gameManager.HideInteractIcon();
                    gateAnimator.SetTrigger("OpenGateIn");
                    Destroy(gameObject);
                }            
            }
        }
    }
}
