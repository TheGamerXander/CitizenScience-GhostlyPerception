using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class UnpackedBoxHandler : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    bool boxActive = false;
    [SerializeField] private string uniqueFunctionName;
    [SerializeField] private string dialogueToTrigger;
    private DialogueRunner dialogueRunner;
    [SerializeField] private GameObject trigger;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        uniqueFunctionName = uniqueFunctionName + "CleanUP";
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler(uniqueFunctionName, CleanUP);
    }


    public void RegisterBoxActive()
    {
        boxActive = true;
        Destroy(trigger);
        gameManager.StartDialogue(dialogueToTrigger);
    }

    public void CleanUP()
    {
        if (boxActive)
        {
            animator.SetTrigger("Finish");
            boxActive = false;
        }
    }

    public void UnpausePlayer()
    {
        gameManager.UnpausePlayer();
    }
}
