using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenerTrigger : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string animationName;
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
        if (playerDetected )
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                gameManager.HideInteractIcon();
                doorAnimator.SetTrigger(animationName);
                Destroy(gameObject);
            }
        }
    }

}
