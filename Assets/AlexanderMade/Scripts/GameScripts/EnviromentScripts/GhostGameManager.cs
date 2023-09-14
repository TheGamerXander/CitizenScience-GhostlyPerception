using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Yarn.Unity;

public class GhostGameManager : MonoBehaviour
{
    [SerializeField] private DataManager dataManager;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCamera;
    private PlayerControls playerControls;
    [SerializeField] private List<GameObject> encounterBeacons;
    private GameObject nearestBeacon;
    [SerializeField] private GameObject insideGhostEncounters;
    [SerializeField] private GameObject mansionBaitBeacon;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject letter;
    [SerializeField] private CamChanger camChanger;

    [SerializeField] private GameObject endMenu;

    private string timeCurrentSighting;
    private bool playerItemsAcquired = false;
    private bool ghostCannonAquired = false;
    private bool ghostDetectorAquired = false;
    private bool ghostMineAquired = false;
    private bool mansionKeyAcquired = false;
    private bool axeAcquired = false;

    public enum GameItems {Axe, MansionKey, Cannon, Detector, Mine};

    private DialogueRunner dialogueRunner;
    [SerializeField] private GameObject dialogueButton;
    [SerializeField] private bool testing = false;

    private GameObject currentGhost;

    [SerializeField] private Animator trapAnimator;
    [SerializeField] private GameObject playerUIMenu;
    [SerializeField] private GameObject interactIcon;
    
    private void Awake()
    {
        playerControls = player.GetComponent<PlayerControls>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler("UnpausePlayer", UnpausePlayer);
        dialogueRunner.AddCommandHandler("PausePlayer", PausePlayer);

        dialogueRunner.AddCommandHandler("StartEnding", StartEnding);

        if (testing)
        {
            ghostCannonAquired = true;
            ghostDetectorAquired = true;
            ghostMineAquired = true;
            playerItemsAcquired = true;
            camChanger.ActivatePlayerCam();
        }
    }

    public GameObject GetPlayerCamera()
    {
        return playerCamera;
    }


    public void SetCurrentGhost(GameObject potentialGhost, VisibilityChecker ghostControls)
    {
        if (currentGhost == null)
        {
            currentGhost = potentialGhost;
            timeCurrentSighting = dataManager.GetCurrentTIme();
            ghostControls.RegisterSighting();
        }
        else if (currentGhost != potentialGhost)
        {
            float currentGhostDistance = Vector3.Distance(player.transform.position, currentGhost.transform.position);
            float potentialGhostDistance = Vector3.Distance(player.transform.position, potentialGhost.transform.position);

            if (potentialGhostDistance < currentGhostDistance)
            {
                currentGhost = potentialGhost;
                timeCurrentSighting = dataManager.GetCurrentTIme();
                ghostControls.RegisterSighting();
            }
        }
    }

    public string GetTimeOfCurrentSighting()
    {
        return timeCurrentSighting;
    }

    public void RemoveGhostFromCurrent(GameObject potentialCurrentGhost)
    {
        if (currentGhost != null)
        {
            if (currentGhost == potentialCurrentGhost)
            {
                currentGhost = null;
                timeCurrentSighting = "";
            }
        }
    }

    public GameObject GetCurrentGhost()
    {
        return currentGhost;
    }

    public void UnpausePlayer()
    {
        playerControls.UnpausePlayer();
        playerControls.LockMouse();
    }
    public void PausePlayer()
    {
        playerControls.PausePlayer();
    }
    public bool PlayerPause()
    {
        return playerControls.PlayerPaused();
    }


    // dialogue controls
    public void SetDialogueButtonON()
    {
        dialogueButton.SetActive(true);
    }
    public void SetDialogueButtonOff()
    {
        dialogueButton.SetActive(false);
    }

    public void StartDialogue(string node)
    {
        PausePlayer();
        playerControls.UnlockMouse();
        dialogueRunner.StartDialogue(node);
    }
    //

    public bool GetCannonAquired()
    {
        return ghostCannonAquired;
    }
    public bool GetDetectorAquired()
    {
        return ghostDetectorAquired;
    }
    public bool GetMineAquired()
    {
        return ghostMineAquired;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public GameObject GetNearestBeacon()
    {
        return nearestBeacon;
    }

    public bool BeaconListEmpty()
    {
        if (encounterBeacons.Count > 0)
        {
            return false;
        }
        return true;
    }

    public float GetNearestBeaconDistance()
    {
        float nearestBeaconDistance = 300;

        if (!BeaconListEmpty())
        {
            float newDistance = Vector3.Distance(encounterBeacons[0].transform.position, player.transform.position);
            nearestBeaconDistance = newDistance;
            nearestBeacon = encounterBeacons[0];

            foreach (GameObject beacon in encounterBeacons)
            {
                newDistance = Vector3.Distance(beacon.transform.position, player.transform.position);

                if (newDistance < nearestBeaconDistance)
                {
                    nearestBeaconDistance = newDistance;
                    nearestBeacon = beacon;
                }
            }

            return nearestBeaconDistance;
        }
        else
        {
            return nearestBeaconDistance;
        }
    }

    public void AddBeaconToList(GameObject beaconToAdd)
    {
        encounterBeacons.Add(beaconToAdd);
    }

    public void RemoveBeaconToList(GameObject beaconToRemove)
    {
        encounterBeacons.Remove(beaconToRemove);
    }

    public void DisableBaitBecon()
    {
        RemoveBeaconToList(mansionBaitBeacon);
        Destroy(mansionBaitBeacon);
    }

    public bool GetPlayerItemsAcquired()
    {
        return playerItemsAcquired;
    }

    public bool GetMansionKeyAcquired()
    {
        return mansionKeyAcquired;
    }

    public bool GetAxeAcquired()
    {
        return axeAcquired;
    }

    public void SignalAcquisition(GameItems aquiredItem)
    {
        switch (aquiredItem)
        {
            case GameItems.Axe:
                axeAcquired = true;
                break;

            case GameItems.MansionKey:
                mansionKeyAcquired = true;
                break;

            case GameItems.Cannon:
                ghostCannonAquired = true;
                break;

            case GameItems.Detector:
                ghostDetectorAquired = true;
                break;

            case GameItems.Mine:
                ghostMineAquired = true;
                break;

            default:
                Debug.LogError("Item Not Recognised!");
                break;
        }

        if (ghostCannonAquired && ghostDetectorAquired && ghostMineAquired)
        {
            playerItemsAcquired = true;
        }
    }

    public void DestroyOtherGameObject(GameObject gameObjectToDestory)
    {
        Destroy(gameObjectToDestory);
    }

    public DataManager GetDataManager()
    {
        return dataManager;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quitting");
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        PausePlayer();
    }

    public void UnpauseGame()
    {
        pauseMenu.SetActive(false);
        letter.SetActive(false);
        UnpausePlayer();
    }

    public void StartEnding()
    {
        camChanger.ActivateEnding();      
    }

    public void EndPlayer()
    {
        player.SetActive(false);
        trapAnimator.SetTrigger("Play");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReturnToMainMenu()
    {
        camChanger.ActivateMainMenu();        
    }

    public void ShowEndMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        endMenu.SetActive(true);
    }

    public void ShowUIMenu()
    {
        playerUIMenu.SetActive(true);
    }

    public void HideUIMenu()
    {
        playerUIMenu.SetActive(false);
    }

    public void ShowInteractIcon()
    {
        interactIcon.SetActive(true);
    }

    public void HideInteractIcon()
    {
        interactIcon.SetActive(false);
    }
}