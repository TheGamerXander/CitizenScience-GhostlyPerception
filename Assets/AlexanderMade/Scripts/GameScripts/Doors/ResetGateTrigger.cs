using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGateTrigger : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private Animator hedgeAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            hedgeAnimator.SetTrigger("Activate");
            gateAnimator.SetTrigger("ResetGate");
            gameManager.StartDialogue("Maze");
            Destroy(gameObject);
        }
    }
}
