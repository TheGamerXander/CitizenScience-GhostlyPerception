using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private CamChanger camChanger;
    [SerializeField] private GameObject intro;    

   public void StartGame()
   {
        intro.SetActive(true);
        camChanger.ActivateCam1();
        Cursor.lockState = CursorLockMode.Locked;
        Destroy(gameObject, 1.4f);
   }

    public void QuitGame()
    {
        gameManager.QuitGame();
    }
}
