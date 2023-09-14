using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CaptureBeamHandler : MonoBehaviour
{
    [SerializeField] private GameObject electricArc;
    [SerializeField] private GameObject beamStart;
    [SerializeField] private GameObject beamEnd;
    private GameObject beamTarget;
    private GameObject ghostCanon;
    bool beamSetUp = false;
    float captureSpeed = 5.0f;
    private CanonHandler canonHandler;
    [SerializeField] private GameObject explosionPrefab;
    private AttackingGhostHandler ghostHandler;

    private float shrinkDuration = 1.6f; // Duration over which to shrink the object
    private Vector3 targetScale = new Vector3(0.18f, 0.18f, 0.18f); // Target scale (shrink size) of the object
    private Vector3 originalScale; 


    public GameObject GetBeamStart()
    {
        return beamStart;
    }

    public void SetUpBeam(GameObject newBeamTarget, GameObject newGhostCanon, CanonHandler newCanonHandler)
    {
        beamTarget = newBeamTarget;
        ghostHandler = beamTarget.GetComponent<AttackingGhostHandler>();
        originalScale = beamTarget.transform.localScale; // Store the original scale of the object
        StartCoroutine(ShrinkCoroutine());



        ghostCanon = newGhostCanon;

        canonHandler = newCanonHandler;
       
        beamSetUp = true;
    }

    public void EndBeam()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (beamSetUp)
        {
            MaintainPositions();
            RealInBeam();
        }
    }


    private void MaintainPositions()
    {
        beamStart.transform.position = beamTarget.transform.position;
        beamEnd.transform.position = ghostCanon.transform.position;
    }


    private void RealInBeam()
    {
        // Move Ghost
        beamTarget.transform.position = Vector3.MoveTowards(beamTarget.transform.position, ghostCanon.transform.position, captureSpeed * Time.deltaTime);

        // Check if the ghost has reached the Canon
        float distanceToWaypoint = Vector3.Distance(beamTarget.transform.position, ghostCanon.transform.position);

        if (distanceToWaypoint < 0.02f) // You can adjust this tolerance based on your needs
        {
            ghostHandler.SignalCapture();
            canonHandler.CompleteGhostBeam();

            Instantiate(explosionPrefab, ghostCanon.transform.position, ghostCanon.transform.rotation);
        }
    }



    private IEnumerator ShrinkCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            beamTarget.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        beamTarget.transform.localScale = targetScale;
    }
}

