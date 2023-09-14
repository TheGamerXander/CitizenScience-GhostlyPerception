using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    private GhostGameManager gameManager;
    private Camera playerCamera;
    private GhostMovementHandler ghostMovementHandler;
    private bool setUp = false;
    private bool isVisible = false;
    private string currentCamo;
    private string firstTimeOnScreen;
    private int numTimesOnScreen = 0;

    private float totalScreenTimeInSeconds = 0.0f;

    public void RegisterSighting()
    {
       
        if (numTimesOnScreen == 0)
        {
            firstTimeOnScreen = gameManager.GetDataManager().GetCurrentTIme();
        }
        numTimesOnScreen++;
    }

    public string GetFirstTimeOnScreen()
    {
        return firstTimeOnScreen;
    }

    public string GetNumTimesOnScreen()
    {
        return numTimesOnScreen.ToString();
    }

    public string GetTotalScreenTime()
    {
        return totalScreenTimeInSeconds.ToString();
    }


    public void Setup(GhostGameManager newManager, Camera newCamera, string camo, GhostMovementHandler movementHandler)
    {
        gameManager = newManager;
        playerCamera = newCamera;
        currentCamo = camo;
        ghostMovementHandler = movementHandler;
        setUp = true;
    }

    public GhostMovementHandler GetMovmentHandler()
    {
        return ghostMovementHandler;
    }


    public string GetCurrentCamo()
    {
        return currentCamo;
    }

    private void FixedUpdate()
    {
        if (setUp)
        {
            if (playerCamera == null)
            {
                Debug.LogWarning("Player camera reference missing.");
                return;
            }

            Renderer renderer = GetComponent<Renderer>();

            if (renderer != null && renderer.isVisible)
            {


                if (!isVisible)
                {
                    Debug.Log(gameObject.name + " is visible!");
                    gameManager.SetCurrentGhost(gameObject, gameObject.GetComponent<VisibilityChecker>());
                    isVisible = true;
                }

            }
            else
            {
                if (isVisible)
                {
                    Debug.Log(gameObject.name + " not visible");
                    gameManager.RemoveGhostFromCurrent(gameObject);
                    isVisible = false;
                }
            }

            if (isVisible)
            {
                totalScreenTimeInSeconds += Time.deltaTime;
            }
        }
    }

    public string GetScale()
    {    
        // Get the world scale of the object
        Vector3 worldScale = transform.lossyScale;
        return worldScale.ToString();  
    }
}
