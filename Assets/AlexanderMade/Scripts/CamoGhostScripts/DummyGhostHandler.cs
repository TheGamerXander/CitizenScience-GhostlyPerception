using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyGhostHandler : MonoBehaviour
{
   
    [SerializeField] List<GameObject> camoPrefabs;

    int listPosition = 0;
    private GameObject currentCamo;
    private GameObject camoChiled;

    private void Start()
    {
        RandomizeOrder(camoPrefabs);
        gameObject.GetComponent<Renderer>().enabled = true;
    }


    public void ChangeCamo()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        SpawnNewCamo();
    }

    private void RandomizeOrder<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private void SpawnNewCamo()
    {
        if (camoChiled != null)
        {
            Destroy(camoChiled);
        }

        if (listPosition < camoPrefabs.Count)
        {
            currentCamo = camoPrefabs[listPosition];
            Transform parentTransform = transform.parent;
            GameObject newCamo = Instantiate(GetChildCamo(currentCamo), transform.position, transform.rotation, parentTransform);
            newCamo.transform.localScale = transform.localScale;
            camoChiled = newCamo;
            int ghostLayer = LayerMask.NameToLayer("Ghost");
            camoChiled.layer = ghostLayer;

            Renderer renderer = camoChiled.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Camo " + listPosition + " / " + camoPrefabs.Count + " " + currentCamo.name);
            listPosition++;
        }
        else
        {
            Debug.Log("List Ended");
        } 
    }

    private GameObject GetChildCamo(GameObject parent)
    {
       // Get the first child of the parent GameObject
       Transform firstChild = parent.transform.GetChild(0);
       return firstChild.gameObject;
    }

    public GameObject GetCurrentCamo()
    {
        return currentCamo;
    }

    public List<GameObject> GetCamoList()
    {
        return camoPrefabs;
    }
}
