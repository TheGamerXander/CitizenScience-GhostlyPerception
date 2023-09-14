using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimationController : MonoBehaviour
{
    [SerializeField] private CamChanger camChnager;
    [SerializeField] private GhostGameManager gameManager;

    public void ActivateCam1()
    {
        camChnager.ActivateCam1 ();
    }

    public void ActivateCam2()
    {
        camChnager.ActivateCam2();
    }

    public void ActivateCam3()
    {
        camChnager.ActivateCam3();
    }

    public void ActivateCam4()
    {
        camChnager.ActivateCam4();
    }
}
