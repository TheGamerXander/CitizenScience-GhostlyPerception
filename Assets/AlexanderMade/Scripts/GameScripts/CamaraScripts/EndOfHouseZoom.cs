using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfHouseZoom : MonoBehaviour
{
    [SerializeField] private CamChanger camChnager;
    [SerializeField] private GameObject introAnimationCar;
    [SerializeField] private GameObject interactiveCar;


    public void EndZoom()
    {       
        introAnimationCar.SetActive(false);
        interactiveCar.SetActive(true);
        camChnager.ActivatePlayerCam();
    }
}
