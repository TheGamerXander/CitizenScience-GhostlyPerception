using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTruckHandler : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private List<Animator> boxAnimators;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerPosTarget;

    [SerializeField] private List<GameObject> boxes;

    private bool unpacking = false;
    private bool movingPlayer = false;
    private bool playerDetected = false;
    private float movementSpeed = 1.0f;
    private float rotationSpeed = 2.5f;

    // Update is called once per frame
    void Update()
    {
        if (playerDetected)
        {
            if (Input.GetKeyDown(KeyCode.E) && !gameManager.PlayerPause())
            {
                if (boxAnimators.Count > 0)
                {
                    gameManager.HideInteractIcon();
                    gameManager.PausePlayer();
                    movingPlayer = true;
                }
            }
        }

        if (movingPlayer)
        {
            MoveToPosAndRotateToLook();
        }
        else if(unpacking)
        {
            player.transform.LookAt(boxes[0].transform);
        }
    }

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

    public void SetBoxUnpacked(Animator animator, GameObject box)
    {
        boxAnimators.Remove(animator);
        boxes.Remove(box);
        unpacking = false;
        gameManager.UnpausePlayer();

        if (boxAnimators.Count < 1)
        {
            Destroy(gameObject);
        }
        else
        {
            if (playerDetected)
            {
                gameManager.ShowInteractIcon(); 
            }
        }     
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

        Vector3 directionToWaypoint = boxes[0].transform.position - player.transform.position;
        //directionToWaypoint.y = 0f; // Ensure the object does not tilt up or down.

        if (directionToWaypoint != Vector3.zero)
        {
            //  Check if the object is facing the waypoint within the specified tolerance.
            float angleDifference = Vector3.Angle(player.transform.forward, directionToWaypoint);
            if (angleDifference <= 0.05)
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
            movingPlayer = false;
            unpacking = true;
            boxAnimators[0].SetTrigger("OffLoad");
        }
    }
}
