using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);             
            }
            Destroy(gameObject);
        }
    }
}
