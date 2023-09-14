using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollectionSignler : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private GhostGameManager.GameItems item;

    public void SignalItemColection()
    {
        gameManager.SignalAcquisition(item);
    }
}
