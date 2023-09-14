using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MineHandler : MonoBehaviour
{
    [SerializeField] private Animator mineAnimator;
    public PlayerItemsManager itemsManager = null;
    public Animator mistAnimator;

    public void TriggerExplosion()
    {
        mineAnimator.SetTrigger("Explode");
    }

    public void RegisterExplosionComplete()
    {   
        DestroyMine();
    }

    public void FreePlayerItems()
    {
        if (itemsManager)
        {
            itemsManager.RegisterMineExplosion();
        }
    }

    public void DestroyMine()
    {
        if (mistAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartMist"))
        {
            mistAnimator.SetTrigger("EndMist");
        }
        Destroy(gameObject);
    }
}
