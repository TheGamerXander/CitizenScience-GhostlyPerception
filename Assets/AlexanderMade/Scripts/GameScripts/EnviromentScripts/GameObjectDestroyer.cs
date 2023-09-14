using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDestroyer : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToDestory;


    public void DestroyOtherGameObject()
    {
        if (gameObjectToDestory != null)
        {
            Destroy(gameObjectToDestory);
        }      
    }
}
