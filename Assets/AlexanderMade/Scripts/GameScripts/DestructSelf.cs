using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructSelf : MonoBehaviour
{
    
    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
