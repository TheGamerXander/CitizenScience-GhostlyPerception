using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapEnding : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private GameObject lastGhostEncounter;
   
    private bool trigger = false;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!trigger)
        {
            if (lastGhostEncounter == null)
            {
                triggerTrap();
            }      
        }
    }

    private void triggerTrap()
    {
        trigger = true;
        gameManager.PausePlayer();
        gameManager.StartDialogue("VampireTrap");
    }

    public void ReturnToMainMenu()
    {
        gameManager.ReturnToMainMenu();
    }
}
