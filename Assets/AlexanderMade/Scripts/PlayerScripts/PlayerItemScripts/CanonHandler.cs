using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonHandler : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private GameObject playerCamera; 
    [SerializeField] private GameObject ghostBulletPrefab;
    public PlayerItemsManager itemsManager = null;
    [SerializeField] private GameObject bulletOrigin;
    [SerializeField] private LayerMask defaultAndGhostLayerMask;

    float raycastDistance = 80.0f;
    private GameObject beamTarget;
    private GameObject currentBeam;
    [SerializeField] private GameObject beamPrefab;
    private CaptureBeamHandler currentCaptureBeamHandler;
    public enum FiringModes { Bullets, Beam};
    public FiringModes currentMode = FiringModes.Bullets;

    [SerializeField] private GameObject canonBeamIndicator;
    [SerializeField] private GameObject canonBeamIndicatorParticals;

    bool hitGhost = false;
    Vector3 rayHitPoint;

    private Vector3 FireRaycast()
    {
        // Get the center of the camera's viewport (where the crosshair points)
        Vector3 rayOrigin = playerCamera.transform.position;

        // Shoot a ray from the camera's center forward
        Ray ray = new Ray(rayOrigin, playerCamera.transform.forward);

        // Create a RaycastHit variable to store information about the hit point
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, raycastDistance, defaultAndGhostLayerMask))
        {
            //if the raycast hit a GameObject on either the default or ghost layers return the point at which they where hit

            if (hit.collider.gameObject.name == "Ghost")
            {
                hitGhost = true;
            }

            rayHitPoint = hit.point;
            return rayHitPoint;
        }
        else
        {           
            // If the raycast doesn't hit anything return an end point that would be at the end of the raycasts reach
            Vector3 endPoint = ray.origin + ray.direction * raycastDistance;

            rayHitPoint = endPoint;

            return rayHitPoint;
        }

        
    }

    public void SetUpBeam(GameObject ghost)
    {
        currentMode = FiringModes.Beam;
        beamTarget = ghost;
        canonBeamIndicator.SetActive(true);
        canonBeamIndicatorParticals.SetActive(true);
    }

    public void FireGhostCanon()
    {
        if (currentMode == FiringModes.Bullets)
        {
            // Instantiate the projectile at the weapon's position
            GameObject newProjectile = Instantiate(ghostBulletPrefab, bulletOrigin.transform.position, bulletOrigin.transform.rotation);
            newProjectile.transform.localScale = bulletOrigin.transform.localScale;

            // set the trigectory of the ghost bullet using the hit pos of the raycast
            newProjectile.GetComponent<GhostBullet>().SetTargetPosition(FireRaycast());

            if (gameManager.GetCurrentGhost() != null)
            {
                PlayerShotDataRecorder dataRecorder = newProjectile.GetComponent<PlayerShotDataRecorder>();             
                dataRecorder.RecordFiringData(playerCamera, gameManager, rayHitPoint, hitGhost);
            }
            hitGhost = false;
        }
    }


    public void FireGhostBeam()
    {
        if (currentMode == FiringModes.Beam)
        {
            if (beamTarget != null)
            {
                // Instantiate the beam at the ghost's position
                currentBeam = Instantiate(beamPrefab, beamTarget.transform.position, beamTarget.transform.rotation);
                currentCaptureBeamHandler = currentBeam.GetComponent<CaptureBeamHandler>();
                currentCaptureBeamHandler.SetUpBeam(beamTarget, bulletOrigin, gameObject.GetComponent<CanonHandler>());
            }          
        }
    }

    public void EndGhostBeam()
    {
        if (currentMode == FiringModes.Beam)
        {
            if(currentBeam != null)
            {             
                currentCaptureBeamHandler.EndBeam();
                currentCaptureBeamHandler = null;
                Destroy(currentBeam);
                currentBeam = null;
            }           
        }
    }

    public void CompleteGhostBeam()
    {
        if (currentMode == FiringModes.Beam)
        {
            if (currentBeam != null)
            {
                canonBeamIndicator.SetActive(false);
                canonBeamIndicatorParticals.SetActive(false);
                currentCaptureBeamHandler.EndBeam();
                currentCaptureBeamHandler = null;
                Destroy(currentBeam);
                currentBeam = null;

                currentMode = FiringModes.Bullets;
                beamTarget = null;
            }
        }
    }
}
