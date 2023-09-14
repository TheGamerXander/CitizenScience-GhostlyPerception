using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private DamageEffectManager damageEffectManager;

    public void TakeDamge()
    {
        damageEffectManager.RegisterDamage();
    }

    public void PausePlayer()
    {
        playerControls.PausePlayer();
    }

    public void UnausePlayer()
    {
        playerControls.UnpausePlayer();
    }
}
