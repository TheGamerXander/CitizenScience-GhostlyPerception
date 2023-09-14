using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovementLinker : MonoBehaviour
{
    [SerializeField] private GhostMovementHandler movementHandler;

    public GhostMovementHandler GetMovementHandler()
    {
        return movementHandler;
    }
}
