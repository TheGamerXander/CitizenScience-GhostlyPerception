using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingGhostHandler : MonoBehaviour
{
    [SerializeField] private GhostMovementHandler ghostMovementHandler;
    [SerializeField] private Animator animator;
    private GameObject player;
    private PlayerControls playerControls;
    private GhostGameManager gameManagerControls;
    private GameObject gameManager;

    private bool attacking = false;
    private bool playerAsigned = false;
    private float rotationSpeed = 2.5f;
    private float rotationTolerance = 35.0f;

    private bool beamTriggered = false;
    private bool ghostDying = false;

    [SerializeField] private GameObject captureBubblePrefab;
    [SerializeField] private GhostBecon ghostBeconControls;
    [SerializeField] private GhostCamoHandler camoHandler;
    [SerializeField] private GhostAttackHandler attackHandler;

    // Update is called once per frame
    void Update()
    {
        if (playerAsigned)
        {
            FacePlayer();

            if (playerAsigned)
            {
                if (gameObject.activeSelf && ghostDying && !beamTriggered)
                {
                    TriggerBeam();
                }
            }            
        }
    }

    public void StartGhostAttack()
    {
        attacking = true;
        if (!playerAsigned)
        {
            StartCoroutine(FindPlayer());
        }
        
        if (camoHandler.CamoListFinished() && !ghostDying)
        {
            ghostDying = true;
        }
    }

    public void EndAttack()
    {
        ghostMovementHandler.UnpauseGhost();
        gameObject.SetActive(false);
    }

    private void SpawnShild()
    {
        attacking = false;
        animator.SetTrigger("SpawnOrb");
    }

    public void AttackPlayer()
    {
        attackHandler.CreateAttack(player);
    }

    private void FacePlayer()
    {
        Vector3 directionToWaypoint = player.transform.position - transform.position;
        directionToWaypoint.y = 0f; // Ensure the object does not tilt up or down.

        if (directionToWaypoint != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (attacking && !camoHandler.CamoListFinished())
            {
                // Check if the object is facing the waypoint within the specified tolerance.
                float angleDifference = Vector3.Angle(transform.forward, directionToWaypoint);
                if (angleDifference <= rotationTolerance)
                {
                    SpawnShild();
                }
            }          
        }
    }

    public void TriggerBeam()
    {
        // Instantiate the projectile at the weapon's position
        GameObject newCaptureBall = Instantiate(captureBubblePrefab, transform.position, transform.rotation);
        newCaptureBall.transform.SetParent(gameObject.transform);
        newCaptureBall.transform.localScale = transform.localScale + new Vector3(80, 80, 80);

        beamTriggered = true;
        playerControls.TriggerCaptureBeam(gameObject);
    }

    IEnumerator FindPlayer()
    {
        gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            gameManagerControls = gameManager.GetComponent<GhostGameManager>();
            player = gameManagerControls.GetPlayer();
            playerControls = player.GetComponent<PlayerControls>();
            playerAsigned = true;
        }
       
        yield return null;    
    }

    public void SignalCapture()
    {
        ghostBeconControls.SignalCapture();
    }
}
