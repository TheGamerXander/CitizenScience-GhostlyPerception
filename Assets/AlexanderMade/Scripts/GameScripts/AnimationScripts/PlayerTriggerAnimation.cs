using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerAnimation : MonoBehaviour
{
    private bool playerDetected = false;
   [SerializeField] private Animator animator;
   [SerializeField] private string animationName;
   [SerializeField] private GhostGameManager gameManager;

    private void Update()
    {
        if (playerDetected)
        {
            if (Input.GetKeyDown(KeyCode.E) && !gameManager.PlayerPause())
            {
                animator.SetTrigger(animationName);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            playerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            playerDetected = false;
        }
    }
}
