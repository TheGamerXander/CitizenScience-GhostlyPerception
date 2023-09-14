using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject playerLookTarget;
    [SerializeField] private GameObject playerPosTarget;

    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject blockingObject;

    private bool playerDetected = false;
    private bool triggered = false;
    private float movementSpeed = 1.0f;
    private float rotationSpeed = 1.5f;

    [SerializeField] private string blockingDialogue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && !playerDetected)
        {
            playerDetected = true;
            gameManager.ShowInteractIcon();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player" && playerDetected)
        {
            playerDetected = false;
            gameManager.HideInteractIcon(); 
        }
    }

    private void Update()
    {
        if (triggered)
        {
            MoveToPosAndRotateToLook();
        }
        else
        {
            if (playerDetected)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if(blockingObject == null)
                    {
                        triggered = true;
                        playerControls = player.GetComponent<PlayerControls>();
                        gameManager.HideInteractIcon();
                        playerControls.PausePlayer();
                    }
                    else
                    {
                        gameManager.HideInteractIcon();
                        gameManager.StartDialogue(blockingDialogue);
                    }            
                }
            }
        }
    }

    private void PlayAnimation()
    {
        animator.SetTrigger("Play");
        Destroy(gameObject);
    }

    private void MoveToPosAndRotateToLook()
    {
        bool destinationReached = false;
        bool lookingAtTarget = false;

        float distanceToTarget = Vector3.Distance(player.transform.position, playerPosTarget.transform.position);
        if (distanceToTarget < 0.01f)
        {
            destinationReached = true;
        }
        else
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerPosTarget.transform.position, movementSpeed * Time.deltaTime);
        }

        Vector3 directionToWaypoint = playerLookTarget.transform.position - player.transform.position;
        //directionToWaypoint.y = 0f; // Ensure the object does not tilt up or down.

        if (directionToWaypoint != Vector3.zero)
        {
            //  Check if the object is facing the waypoint within the specified tolerance.
            float angleDifference = Vector3.Angle(player.transform.forward, directionToWaypoint);
            if (angleDifference <= 0.01f)
            {
                lookingAtTarget = true;
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (destinationReached && lookingAtTarget)
        {  
            PlayAnimation();
        }
    }
}
