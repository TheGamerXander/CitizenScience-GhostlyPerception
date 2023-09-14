using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatedGhostChecker : MonoBehaviour
{
    [SerializeField] private List<GameObject> ghostsToDefeat;

    private void FixedUpdate()
    {
        bool ghostsDefeated = true;

        foreach (GameObject ghost in ghostsToDefeat)
        {
            if (ghost != null)
            {
                ghostsDefeated = false;
                break;
            }
        }

        if (ghostsDefeated)
        {
            Debug.LogWarning("Ghosts Defeated");
            Destroy(gameObject);
        }
    }
}
