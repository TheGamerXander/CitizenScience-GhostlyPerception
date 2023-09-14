using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetMansionDoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator mazeAnimator;
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private GameObject insideGhostEncounters;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            insideGhostEncounters.SetActive(true);
            gameManager.DisableBaitBecon();
            animator.SetTrigger("Reset");
            mazeAnimator.SetTrigger("Reset");
            Destroy(gameObject);
        }
    }
}
