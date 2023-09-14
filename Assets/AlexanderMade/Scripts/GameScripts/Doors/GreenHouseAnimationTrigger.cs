using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenHouseAnimationTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GhostGameManager gameManager;
    private bool playerDetected = false;
    private bool opening = false;
    private bool triggered = false;

    [SerializeField] private GameObject playerLookTarget;
    [SerializeField] private GameObject playerPosTarget;

    private PlayerControls playerControls;
    private GameObject player;
    private float movementSpeed = 0.6f;
    private float rotationSpeed = 1f;

    [SerializeField] private GameObject LockBarrier;
    [SerializeField] private GameObject barrier;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && !playerDetected)
        {
            gameManager.ShowInteractIcon();
            playerDetected = true;
            player = other.gameObject;
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
        if (!opening)
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
                        if(LockBarrier != null)
                        {
                            gameManager.HideInteractIcon();
                            barrier.SetActive(true);
                            gameManager.StartDialogue("EnergyBlockingLock");
                        }
                        else
                        {
                            if (gameManager.GetAxeAcquired())
                            {
                                gameManager.HideInteractIcon();
                                triggered = true;
                                playerControls = player.GetComponent<PlayerControls>();
                                playerControls.PausePlayer();
                            }
                            else
                            {
                                gameManager.HideInteractIcon();
                                gameManager.StartDialogue("GetAxe");                           
                            }               
                        }           
                    }
                }
            }
           
        }
    }

    private void OpenDoor()
    {
        animator.SetTrigger("OpenGreenHouse");
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
            if (angleDifference <= 0.50)
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
            opening = true;
            OpenDoor();
        }
    }
}
