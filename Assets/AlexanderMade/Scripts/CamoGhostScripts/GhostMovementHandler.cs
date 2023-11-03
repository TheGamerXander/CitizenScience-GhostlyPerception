using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovementHandler : MonoBehaviour
{
    [SerializeField] private GameObject photoMaker;
    [SerializeField] private GameObject backgroud;
    [SerializeField] private GameObject camoGhost;
    [SerializeField] private List<GameObject> waypoints;
    [SerializeField] private GameObject destinationWaypoint;
    [SerializeField] private GameObject currentWaypoint;

    private float slowMaxMovementSpeed = 0.8f;
    private float fastMaxMovementSpeed = 1.0f;

    private float movementSpeedCurrent = 0.8f;
    private float fearFactor = 0.0f; // fear factor is used to mesure how scared the ghost that the player is looking at the and 
    // is used in caculating how fast the ghost should move
    private float rotationSpeed = 3.5f;

    private float rotationTolerance = 4.0f;
    private bool isFacingWaypoint = false;

    private bool paused = false;

    [SerializeField] private GameObject attackingGhost;
    [SerializeField] private GhostCamoHandler camoHandler;
    GhostGameManager gameManager;

    Vector3 playerToEnemy;
    Vector3 crosshairDirection;
    float dotProduct;
    float angleInDegrees;

    //Start is called before the first frame update
    void Start()
    {
        if (!photoMaker.activeSelf)
        {
            foreach (var waypoint in waypoints)
            {
                waypoint.GetComponent<Renderer>().enabled = false;
            }
            currentWaypoint.GetComponent<Renderer>().enabled = false;
            backgroud.GetComponent<Renderer>().enabled = false;

            RandomisePosition();

            camoHandler.SetUpCamoHandler(); 
            gameObject.SetActive(false);

            gameManager = camoHandler.GetGameManager();
        }     
    }

    // Update is called once per frame
    void Update()
    {
        if (!photoMaker.activeSelf)
        {           
            if (!paused)
            {

                DetermineFearLevel();

                if (isFacingWaypoint)
                {
                    MoveToWaypoint();
                }
                else
                {
                    FaceWaypoint();
                }
            }         
        }
    }


    void DetermineFearLevel()
    {
        playerToEnemy = camoGhost.transform.position - gameManager.GetPlayer().transform.position;
        crosshairDirection = gameManager.GetPlayerCamera().transform.forward;

        playerToEnemy.Normalize();
        crosshairDirection.Normalize();

        dotProduct = Vector3.Dot(playerToEnemy, crosshairDirection);

        angleInDegrees = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log(angleInDegrees);
        }

    }



    public void DetectionCheck()
    {
        Vector3 playerToEnemy = transform.position - gameManager.GetPlayer().transform.position;
        Vector3 crosshairDirection = gameManager.GetPlayerCamera().transform.forward;

        playerToEnemy.Normalize();
        crosshairDirection.Normalize();

        float dotProduct = Vector3.Dot(playerToEnemy, crosshairDirection);
        float angleInDegrees = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
    }



    public string GetIsRotating()
    {
        bool isRotating = true;

        // if the ghost is not faceing the waypoint then the ghost is rotating
        if (isFacingWaypoint)
        {
            isRotating = false;
        }

        return isRotating.ToString();
    }



    public void RegisterGhostHit()
    {
        if (!paused)
        {
            PauseGhost();
        }      
    }

    public void PauseGhost()
    {
        gameManager.RemoveGhostFromCurrent(camoHandler.GetCurrentCamoModle());

        attackingGhost.transform.position = transform.position;
        attackingGhost.transform.rotation = transform.rotation;
        attackingGhost.SetActive(true);

        AttackingGhostHandler attackHandler = attackingGhost.GetComponent<AttackingGhostHandler>();

        if (attackHandler != null)
        {
            attackHandler.StartGhostAttack();
        }
        else
        {
            Debug.LogError("attackHandler Missing!");
        }
       

        paused = true;
        camoGhost.SetActive(false);
    }

    public void UnpauseGhost()
    {
        paused = false;
        RandomisePosition();
        camoHandler.ChangeCamo();
        camoGhost.SetActive(true);
    }

    private void RandomisePosition()
    {
        //add currentWaypoint back in the list so that all waypoints are in the list 
        waypoints.Add(currentWaypoint);

        // set the current wapoint to a random one and set the ghosts position to that waypoints
        currentWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        transform.position = currentWaypoint.transform.position;

        waypoints.Remove(currentWaypoint);

        // sent the destinationWaypoint to be = to the currentWaypoint so that it can be added back to the list once a new waypoint is asigned 
        destinationWaypoint = currentWaypoint;
        isFacingWaypoint = true;
    }

    private void MoveToWaypoint()
    {
        // Move Ghost
        transform.position = Vector3.MoveTowards(transform.position, destinationWaypoint.transform.position, (fearFactor * movementSpeedCurrent) * Time.deltaTime);
        // Check if the ghost has reached the destination waypoint
        float distanceToWaypoint = Vector3.Distance(transform.position, destinationWaypoint.transform.position);
        if (distanceToWaypoint < 0.01f) // You can adjust this tolerance based on your needs
        {
            DeterminNextWaypoint();
        }
    }

    private void DeterminNextWaypoint()
    {
        // Add the waypoint the Ghost came from back into the list
        waypoints.Add(currentWaypoint);

        // Change the current waypoint to the destination as that destination has been reached, and this is the new current waypoint.
        currentWaypoint = destinationWaypoint;

        // Remove the new current waypoint as the ghost can't go to where it is.
        waypoints.Remove(currentWaypoint);

        // Select a new waypoint to be the next destination
        if (waypoints.Count > 0)
        {
            destinationWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        }

        isFacingWaypoint = false; // Reset isFacingWaypoint to false
    }

    private void FaceWaypoint()
    {
        Vector3 directionToWaypoint = destinationWaypoint.transform.position - transform.position;
        directionToWaypoint.y = 0f; // Ensure the object does not tilt up or down.

        if (directionToWaypoint != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (fearFactor * rotationSpeed) * Time.deltaTime);

            // Check if the object is facing the waypoint within the specified tolerance.
            float angleDifference = Vector3.Angle(transform.forward, directionToWaypoint);
            if (angleDifference <= rotationTolerance)
            {
                isFacingWaypoint = true;
                // Object is facing the waypoint.
            }
        }
    }

    public string GetCurrentSpeed()
    {
        return movementSpeedCurrent.ToString();
    }

    public string GetCurrentRotationSpeed()
    {
        return rotationSpeed.ToString();
    }

    public void SetGhostToFast()
    {
        movementSpeedCurrent = fastMaxMovementSpeed;
    }

    public void SetGhostToSlow()
    {
        movementSpeedCurrent = slowMaxMovementSpeed;
    }
}