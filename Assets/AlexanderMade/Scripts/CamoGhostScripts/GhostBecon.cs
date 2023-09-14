using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBecon : MonoBehaviour
{
    private GhostGameManager gameManager;
    [SerializeField] private GameObject encounter;
    [SerializeField] private GameObject ghost;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject beaconTrigger;

    // Start is called before the first frame update
    void Start()
    {
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            string newName = parentTransform.name + " beacon";

           gameObject.name = newName;
        }
        StartCoroutine(FindGameManagerCoroutine());

        gameObject.GetComponent<Renderer>().enabled = false;
        beaconTrigger.GetComponent<Renderer>().enabled = false;
        background.SetActive(false);       
    }


    private IEnumerator FindGameManagerCoroutine()
    {
        yield return null; // Wait for one frame to ensure all objects are initialized

        GameObject gameManagerObject = GameObject.Find("GameManager");

        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GhostGameManager>();

            if (gameManager != null)
            {
                gameManager.AddBeaconToList(gameObject);
            }
            else
            {
                Debug.Log("GhostGameManager component not found on GameManager.");
            }
        }
        else
        {
            Debug.Log("GameManager not found in the scene.");
        }
    }


    public void SignalCapture()
    {
        gameManager.RemoveBeaconToList(gameObject);
        Destroy(encounter);
    }

    public void ActivateEncounter()
    {
        ghost.SetActive(true);
        background.SetActive(true);
    }
}
