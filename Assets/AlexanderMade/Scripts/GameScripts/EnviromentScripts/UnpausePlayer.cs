using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpausePlayer : MonoBehaviour
{

    [SerializeField] private PlayerControls playerControls;

    public void UnFreezePlayer()
    {
        playerControls.UnpausePlayer();     
    }
}
