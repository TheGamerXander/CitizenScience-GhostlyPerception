using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Pair<T1, T2>
{
    public T1 StartPoint { get; set; }
    public T2 EndPoint { get; set; }

    public Pair(T1 first, T2 second)
    {
        StartPoint = first;
        EndPoint = second;
    }
}

public class ScreenShotMaker : MonoBehaviour
{
   [SerializeField] private GhostCamoHandler camoHandler;
   [SerializeField] private GameObject ghost;
   [SerializeField] private GameObject ghostCenterPoint;
   [SerializeField] private GameObject ghostBody;
   [SerializeField] private GameObject dummyPlayerBody;


   [SerializeField] private GameObject sphereOne;
   [SerializeField] private GameObject sphereTwo;
   [SerializeField] private GameObject sphereThree;
   [SerializeField] private GameObject sphereFour;
   [SerializeField] private GameObject sphereFive;
   [SerializeField] private GameObject background;

   [SerializeField] private float ghostMovespeed = 0.2f;

    private bool photosStarted = false;
    private bool pathFinished = false;
    private int pathCount = 0;
    private Pair<GameObject, GameObject>[] locationPairs = new Pair<GameObject, GameObject>[20];

    private bool PausedForScreenShot = false;
    private bool ghostShotAquired = false;
    private bool roomShotAquired = false;
    private float pauseTimer = 1.3f;
    private float targetPauseTime = 0.0f;
    private string currentStart = "";
    private string currentEnd = "";
    private int GhostShots = 0;
    private GameObject currentGhostCamo;

    Vector3 halfwayPoint;
    bool halfwayTriggered = false;

    private void Start()
    {
        sphereOne.GetComponent<Renderer>().enabled = false;
        sphereTwo.GetComponent<Renderer>().enabled = false;
        sphereThree.GetComponent<Renderer>().enabled = false;
        sphereFour.GetComponent<Renderer>().enabled = false;
        sphereFive.GetComponent<Renderer>().enabled = false;
        background.GetComponent<Renderer>().enabled = false;

        // Pair and load locations      
        locationPairs[0] = new Pair<GameObject, GameObject>(sphereOne, sphereTwo); //Path 1to2
        locationPairs[1] = new Pair<GameObject, GameObject>(sphereTwo, sphereOne); //Path 2to1
        locationPairs[2] = new Pair<GameObject, GameObject>(sphereOne, sphereThree); //Path 1to3
        locationPairs[3] = new Pair<GameObject, GameObject>(sphereThree, sphereOne); //Path 3to1
        locationPairs[4] = new Pair<GameObject, GameObject>(sphereOne, sphereFour); //Path 1to4
        locationPairs[5] = new Pair<GameObject, GameObject>(sphereFour, sphereOne); //Path 4to1
        locationPairs[6] = new Pair<GameObject, GameObject>(sphereOne, sphereFive); //Path 1to5
        locationPairs[7] = new Pair<GameObject, GameObject>(sphereFive, sphereOne); //Path 5to1
        locationPairs[8] = new Pair<GameObject, GameObject>(sphereTwo, sphereThree); //Path 2to3
        locationPairs[9] = new Pair<GameObject, GameObject>(sphereThree, sphereTwo); //Path 3to2
        locationPairs[10] = new Pair<GameObject, GameObject>(sphereTwo, sphereFour); //Path 2to4
        locationPairs[11] = new Pair<GameObject, GameObject>(sphereFour, sphereTwo); //Path 4to2
        locationPairs[12] = new Pair<GameObject, GameObject>(sphereTwo, sphereFive); //Path 2to5
        locationPairs[13] = new Pair<GameObject, GameObject>(sphereFive, sphereTwo); //Path 5to2
        locationPairs[14] = new Pair<GameObject, GameObject>(sphereThree, sphereFour); //Path 3to4
        locationPairs[15] = new Pair<GameObject, GameObject>(sphereFour, sphereThree); //Path 4to3
        locationPairs[16] = new Pair<GameObject, GameObject>(sphereThree, sphereFive); //Path 3to5
        locationPairs[17] = new Pair<GameObject, GameObject>(sphereFive, sphereThree); //Path 5to3
        locationPairs[18] = new Pair<GameObject, GameObject>(sphereFour, sphereFive); //Path 4to5
        locationPairs[19] = new Pair<GameObject, GameObject>(sphereFive, sphereFour); //Path 5to4


        ghost.transform.position = locationPairs[pathCount].StartPoint.transform.position;
        SetGhostLookAtToDestination();
        LookAtGhost();
        camoHandler.SetUpCamoHandlerForPhotos();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !photosStarted)
        {
            Debug.Log("Photos Started!");
            GhostShots = 0;
            photosStarted = true;
            pathFinished = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !photosStarted)
        {
            ghostBody.GetComponent<Renderer>().enabled = false;

            if (camoHandler.CamoListFinished())
            {                           
                camoHandler.ResetCamoListPosition();
                camoHandler.DestoryCurrentCamo();
                ghostBody.GetComponent<Renderer>().enabled = true;
                currentGhostCamo = null;
            }
            else
            {
                camoHandler.ChangeCamo();
                currentGhostCamo = camoHandler.GetCurrentCamoModle();           
            }



            GameObject GhostCamo = camoHandler.GetCurrentCamo();
            string currentCamoName = "Null";

            if (GhostCamo != null)
            {
                currentCamoName = GhostCamo.name;
            }

            Debug.Log("Current Camo = " + currentCamoName);         
        }

        if (!PausedForScreenShot && photosStarted)
        {
            if (pathFinished && pathCount < 20)
            {
                StartNewPath();
            }
            else if(!pathFinished && pathCount < 20)
            {
                MoveGhostToGoal();
            }
            if (pathCount == 20) 
            {
                Debug.Log("Photos Ended!");
                photosStarted = false;
            }
        }
        else if (PausedForScreenShot && photosStarted)
        {
            ScreenShotPauseTimer();
        }
    }

    private void ScreenShotPauseTimer()
    {
        if (!ghostShotAquired)
        {
            if (targetPauseTime == pauseTimer)
            {
                TakeGhostShot();
            }

            if (PauseTimerTick())             
            {
                ghostShotAquired = true;

                if (currentGhostCamo != null)
                {
                    currentGhostCamo.GetComponent<Renderer>().enabled = false;
                }
                else
                {
                    ghostBody.GetComponent<Renderer>().enabled = false;
                }

                   
                targetPauseTime = pauseTimer;
            }
        }
        else if (!roomShotAquired)
        {
            if (targetPauseTime == pauseTimer)
            {
                TakeRoomShot();
            }
            if (PauseTimerTick())
            {
                roomShotAquired = true;
                if (currentGhostCamo != null)
                {
                    currentGhostCamo.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    ghostBody.GetComponent<Renderer>().enabled = true;
                }
            
                targetPauseTime = pauseTimer;
            }
        }
        else
        {
            PausedForScreenShot = false;
            ghostShotAquired = false;
            roomShotAquired = false;
        }        
    }

    bool PauseTimerTick()
    {
        targetPauseTime -= Time.deltaTime;
        if (targetPauseTime <= 0.0f)
        {
            return true;
        }
        return false;
    }

    private void StartNewPath()
    {
        GhostShots = 0;
        pathFinished = false;
        ghost.transform.position = locationPairs[pathCount].StartPoint.transform.position;
        currentStart = locationPairs[pathCount].StartPoint.name;
        currentEnd = locationPairs[pathCount].EndPoint.name;

        // calculate half way point
        halfwayPoint = Vector3.Lerp(locationPairs[pathCount].StartPoint.transform.position, locationPairs[pathCount].EndPoint.transform.position, 0.5f);
        halfwayTriggered = false;

        SetGhostLookAtToDestination();
        LookAtGhost();
        TakeScreenShots();        
    }

    private void TakeScreenShots()
    {
        GhostShots++;
        targetPauseTime = pauseTimer;
        PausedForScreenShot = true;
    }

    private void TakeGhostShot()
    {
        ScreenCapture.CaptureScreenshot(ScreenShotNamer(true));            
    }

    private void TakeRoomShot()
    {       
        ScreenCapture.CaptureScreenshot(ScreenShotNamer(false));        
    }

    private string ScreenShotNamer(bool GhostVisible)
    {
        GameObject GhostCamo = camoHandler.GetCurrentCamo();
        string currentCamoName = "Null";

        if (GhostCamo != null)
        {
            currentCamoName = GhostCamo.name;
        }

        string newScreenShotName = 
           ghost.name.ToString() + " "
           + currentStart + "-" + currentEnd + " "
           + GhostShots.ToString() + " "
           + GhostVisible.ToString()
           + " Gp X" + ghost.transform.position.x.ToString() + " Y" + ghost.transform.position.y.ToString() + " Z" + ghost.transform.position.z.ToString()
           + " Gr X" + ghost.transform.rotation.x.ToString() + " Y" + ghost.transform.rotation.y.ToString() + " Z" + ghost.transform.rotation.z.ToString()
           + " Camo " + currentCamoName
           + ".png";

        return (newScreenShotName);
    }


    void MoveGhostToGoal()
    {
        // Move Ghost
        ghost.transform.position = Vector3.MoveTowards(ghost.transform.position, locationPairs[pathCount].EndPoint.transform.position, ghostMovespeed * Time.deltaTime);
        SetGhostLookAtToDestination();
        LookAtGhost();
              
        if (ghost.transform.position == locationPairs[pathCount].EndPoint.transform.position)
        {
            // take screen shot of ghost at path end
            TakeScreenShots();
            pathFinished = true;
            pathCount++;
        }
        else if (!halfwayTriggered)
        {
            if (Vector3.Distance(ghost.transform.position, halfwayPoint) < 0.01f)
            {
                // Take screen shot when ghost is halfway
                TakeScreenShots();
                halfwayTriggered = true;
            }
        }
    }

    void LookAtGhost()
    {
        transform.LookAt(ghostCenterPoint.transform);            
    }

    void SetGhostLookAtToDestination()
    {
        //ghost.transform.LookAt(locationPairs[pathCount].EndPoint.transform);

        Vector3 directionToWaypoint = locationPairs[pathCount].EndPoint.transform.position - ghost.transform.position;
        directionToWaypoint.y = 0f;

        if (directionToWaypoint != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
            ghost.transform.rotation = targetRotation;         
        }
    }
}
