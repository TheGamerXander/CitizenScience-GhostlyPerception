using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterMansionDoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
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

                if (gameManager.GetMansionKeyAcquired())
                {
                    gameManager.HideInteractIcon();
                    animator.SetTrigger("OpenDoorIn");
                    Destroy(gameObject);
                }
                else
                {
                    gameManager.HideInteractIcon();
                    gameManager.StartDialogue("MansionBlocked");
                }            
            }          
        }
    }
}
