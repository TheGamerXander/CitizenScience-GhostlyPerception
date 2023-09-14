using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamChanger : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] cameras;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private GameObject letterCanvas;
    private int camQueued = 1;
    private int activeCam = 6;

    public void ActivatePlayerCam()
    {       
        SignalCamChange(0);
    }
    public void ActivateCam1()
    {
        SignalCamChange(1);
    }
    public void ActivateCam2()
    {
        SignalCamChange(2);
    }
    public void ActivateCam3()
    {
        SignalCamChange(3);
    }
    public void ActivateCam4()
    {
        SignalCamChange(4);
    }
    public void ActivateCam5()
    {
        SignalCamChange(5);
    }
    public void ActivateMainMenu()
    {
        SignalCamChange(6);
    }
    public void ActivateEnding()
    {
        SignalCamChange(7);
    }

    public void SignalCamChange(int nextCam)
    {
        camQueued = nextCam;
        Debug.Log("nextCam = " + nextCam);
        FadeOut();
    }

    public void ChangeCam()
    {       
        cameras[camQueued].SetActive(true); // reminder the new camera is activated first so there is not a point were no camera is active, Dont know if this matters
        cameras[activeCam].SetActive(false);       
        activeCam = camQueued;
        FadeIn();
    }

    private void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    private void FadeIn()
    {
        animator.SetTrigger("FadeIn");

        if (activeCam == 0)
        {
            player.SetActive(true);
            playerControls.PausePlayer();
            letterCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else if(activeCam == 7)
        {
            gameManager.EndPlayer();
        }
        else if (activeCam == 6)
        {
            gameManager.ShowEndMenu();
        }
    }
}
