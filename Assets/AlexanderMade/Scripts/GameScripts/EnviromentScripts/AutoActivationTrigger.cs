using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoActivationTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private GameObject activatetionObject;

    private void FixedUpdate()
    {
        if (activatetionObject == null)
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);

            }
            Destroy(gameObject);
        }      
    }
}
