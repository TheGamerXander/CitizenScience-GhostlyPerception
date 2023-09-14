using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionChangeTrigger : MonoBehaviour
{
    bool beaconAsigned = false;
    private GhostBecon beconControls;
    private GameObject nearestBeacon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            GameObject animatorObject = other.gameObject.GetComponent<PlayerControls>().MistCanvas;
            Animator animator = animatorObject.GetComponent<Animator>();
            animator.SetTrigger("StartMist");
        }

        if (other.gameObject.name == "BeaconTrigger" && !beaconAsigned)
        {
           nearestBeacon = other.gameObject;
           GameObject beacon  = other.gameObject.transform.parent.gameObject;
           beconControls = beacon.GetComponent<GhostBecon>();

           beaconAsigned = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            GameObject animatorObject = other.gameObject.GetComponent<PlayerControls>().MistCanvas;
            Animator animator = animatorObject.GetComponent<Animator>();
            animator.SetTrigger("EndMist");
        }

        if (nearestBeacon != null)
        {
            if (other.gameObject.name == nearestBeacon.name)
            {
                beconControls.ActivateEncounter();
            }
        }
       
    }
}
