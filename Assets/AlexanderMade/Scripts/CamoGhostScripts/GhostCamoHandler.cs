using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCamoHandler : MonoBehaviour
{
    private GhostGameManager gameManager;
    private GameObject playerCamera;

    [SerializeField] private GameObject camoDazzleDummy;
    private DummyGhostHandler dazzleDummyHandler;
    private GhostMovementHandler movementHandler;

    public List<GameObject> camoLowSigmaPrefabs;
    public List<GameObject> camoMediumSigmaPrefabs;
    public List<GameObject> camoHighSigmaPrefabs;

    public List<GameObject> camoPrefabsInUse;

    int camoListPosition = 0;

    private GameObject currentCamo;
    private GameObject camoModel;

    [SerializeField] private GameObject camoGhostBody;

    [System.Serializable]
    public class PreviousCamoSpeedData
    {
        public string name;
        public bool wasFast;
    }

    enum Speeds {Fast, Slow};
    private Speeds lastSpeed = Speeds.Fast;
  
    public List<PreviousCamoSpeedData> previousCamoSpeedDataList;


    private void Awake()
    {
        GameObject managerObject = GameObject.Find("GameManager");      
        gameManager = managerObject.GetComponent<GhostGameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Warning could not find gameManager script");
        }

        playerCamera = gameManager.GetPlayerCamera();     
        if (playerCamera == null)
        {
            Debug.LogError("Warning could not find playerCamera");
        }

        movementHandler = gameObject.GetComponent<GhostMovementHandler>();
        if (movementHandler == null)
        {
            Debug.LogError("Warning could not find movementHandler");
        }
    }

    public void SetUpCamoHandler()
    {
        camoGhostBody.GetComponent<Renderer>().enabled = false;

        //Add a random camo from the Low Sigma camo prefabs 
        int randomIndex = Random.Range(0, camoLowSigmaPrefabs.Count);  
        camoPrefabsInUse.Add(camoLowSigmaPrefabs[randomIndex]);

        //Add a random camo from the Medium Sigma camo prefabs 
        randomIndex = Random.Range(0, camoMediumSigmaPrefabs.Count);
        camoPrefabsInUse.Add(camoMediumSigmaPrefabs[randomIndex]);

        //Add a random camo from the High Sigma camo prefabs 
        randomIndex = Random.Range(0, camoHighSigmaPrefabs.Count);
        camoPrefabsInUse.Add(camoHighSigmaPrefabs[randomIndex]);

        // Get the dazzle camos
        dazzleDummyHandler = GetChildObject(camoDazzleDummy).GetComponent<DummyGhostHandler>();
        List<GameObject> camoDazzlePrefabs = dazzleDummyHandler.GetCamoList();
        //Add a random camo from the dazzle camo prefabs 
        randomIndex = Random.Range(0, camoDazzlePrefabs.Count);
        camoPrefabsInUse.Add(camoDazzlePrefabs[randomIndex]);



        // duplicate the camos
        int originalCount = camoPrefabsInUse.Count;
        for (int i = 0; i < originalCount; i++)
        {
            GameObject prefab = camoPrefabsInUse[i];
            camoPrefabsInUse.Add(prefab);
        }

        RandomizeOrder(camoPrefabsInUse);

        SpawnNewCamo();
    }


    public void SetUpCamoHandlerForPhotos()
    {     
        foreach (GameObject prefab in camoLowSigmaPrefabs)
        {
            camoPrefabsInUse.Add(prefab);
        }
        
        foreach (GameObject prefab in camoMediumSigmaPrefabs)
        {
            camoPrefabsInUse.Add(prefab);
        }

        foreach (GameObject prefab in camoHighSigmaPrefabs)
        {
            camoPrefabsInUse.Add(prefab);
        }

        dazzleDummyHandler = GetChildObject(camoDazzleDummy).GetComponent<DummyGhostHandler>();
        foreach (GameObject prefab in dazzleDummyHandler.GetCamoList())
        {
            camoPrefabsInUse.Add(prefab);
        }     
    }


    public void ChangeCamo()
    {    
        SpawnNewCamo();
    }

    private void RandomizeOrder<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }


    private void SpawnNewCamo()
    {     
        DestoryCurrentCamo();

        if (camoListPosition < camoPrefabsInUse.Count)
        {
            currentCamo = camoPrefabsInUse[camoListPosition];
            Transform parentTransform = camoGhostBody.transform.parent;
            GameObject newCamo = Instantiate(GetChildObject(currentCamo), camoGhostBody.transform.position, camoGhostBody.transform.rotation, parentTransform);

            camoModel = newCamo;
            camoModel.transform.localScale = camoGhostBody.transform.localScale;
            int ghostLayer = LayerMask.NameToLayer("Ghost");
            camoModel.layer = ghostLayer;
            Renderer renderer = camoModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            camoModel.name = gameObject.name;

            camoModel.AddComponent<VisibilityChecker>();
            VisibilityChecker camoChiledVisChecker = camoModel.GetComponent<VisibilityChecker>();

            if (camoChiledVisChecker != null)
            {
                camoChiledVisChecker.Setup(gameManager, playerCamera.GetComponent<Camera>(), currentCamo.name, movementHandler);
                assignGhostSpeed();
            }
            else
            {
                Debug.LogError("VisibilityChecker failed to attach");
            }

            camoListPosition++;
        }
        else
        {
            Debug.Log("List Ended");
        }
    }

    private GameObject GetChildObject(GameObject parent)
    {
        // Get the first child of the parent GameObject
        Transform firstChild = parent.transform.GetChild(0);
        return firstChild.gameObject;
    }

    public GameObject GetCurrentCamo()
    {
        return currentCamo;
    }

    public GameObject GetCurrentCamoModle()
    {
        return camoModel;
    }

    public bool CamoListFinished()
    {
        return camoListPosition >= camoPrefabsInUse.Count;
    }

    public void ResetCamoListPosition()
    {
        camoListPosition = 0;
        currentCamo =  null;
    }

    public void DestoryCurrentCamo()
    {
        if (camoModel != null)
        {
            Destroy(camoModel);
            camoModel = null;
        }
    }

    public GhostGameManager GetGameManager()
    {
        return gameManager;
    }

    private void assignGhostSpeed()
    {

        if (GhostAppeardBefore())
        {

            if (lastSpeed == Speeds.Fast)
            {
                movementHandler.SetGhostToSlow();
            }
            else
            {
                movementHandler.SetGhostToFast();
            }
        }
        else
        {
            // Generate a random integer (0 or 1)
            int randomInt = Random.Range(0, 2);

            // Convert the random integer to a bool value
            bool speedFast = randomInt == 1;

            if (speedFast)
            {
                movementHandler.SetGhostToFast();
            }
            else
            {
                movementHandler.SetGhostToSlow();
            }


            StoreCamoSpeedData(speedFast);
        }      
    }


    private void StoreCamoSpeedData(bool speedSetToFast)
    {
        previousCamoSpeedDataList.Add(new PreviousCamoSpeedData { name = camoModel.name, wasFast = speedSetToFast });          
    }

    private bool GhostAppeardBefore()
    {
        foreach (PreviousCamoSpeedData previousCamoVersion in previousCamoSpeedDataList)
        {          
            if (previousCamoVersion != null)
            {
                if (previousCamoVersion.name == currentCamo.name)
                {
                    if (previousCamoVersion.wasFast)
                    {
                        lastSpeed = Speeds.Fast;
                    }
                    else
                    {
                        lastSpeed = Speeds.Slow;
                    }

                   return true;
                }
            }
        }

        return false;
    }
}
