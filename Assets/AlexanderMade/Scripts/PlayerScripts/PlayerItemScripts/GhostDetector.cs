using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GhostDetector : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;
    [SerializeField] private GameObject needle;
    private enum ThreatLevels { Green, YellowLow, YellowHigh, AmberLow, AmberHigh, RedLow, RedHigh, None };

    private ThreatLevels currentThreatLevel = ThreatLevels.None;
    private ThreatLevels newThreatLevel = ThreatLevels.Green;


    bool isMoving = false;
    bool isRotatingLeft = true;
    float maxLeft = 0;
    float maxRight = 0;
    private float currentAngle = 0;
    private float rotationSpeed = 50.0f;

    // Update is called once per frame
    void Update()
    {     
        MoveNeedle();
    }

    void  CalculateThreatLevel()
    { 
        int nearestThreatDistance = Mathf.CeilToInt(gameManager.GetNearestBeaconDistance());

        switch (nearestThreatDistance)
        {
            case int n when n >= 0 && n <= 2:

                newThreatLevel = ThreatLevels.RedHigh;

                break;

            case int n when n >= 2 && n <= 4:

                newThreatLevel = ThreatLevels.RedLow;

                break;

            case int n when n >= 5 && n <= 8:

                newThreatLevel = ThreatLevels.AmberHigh;

                break;

            case int n when n >= 9 && n <= 12:
                newThreatLevel = ThreatLevels.AmberLow;
                break;


            case int n when n >= 13 && n <= 15:
                newThreatLevel = ThreatLevels.YellowHigh;
                break;

            case int n when n >= 16 && n <= 200:
                newThreatLevel = ThreatLevels.YellowLow;
                break;

            default:
                newThreatLevel = ThreatLevels.Green;               
                break;
        }      
    }

    void MoveNeedle()
    {
        if (isMoving)
        {
            float targetAngle = isRotatingLeft ? maxLeft : maxRight;

            // Rotate the object towards the target angle
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            needle.transform.eulerAngles = new Vector3(needle.transform.eulerAngles.x, needle.transform.eulerAngles.y, currentAngle);

            // Check if we reached the target angle, then change the rotation direction
            if (Mathf.Abs(currentAngle - targetAngle) < 0.001f)
            {
                isRotatingLeft = !isRotatingLeft;
                isMoving = false;
            }
        }
        else
        {
            CalculateThreatLevel();

            
            if (currentThreatLevel != newThreatLevel)
            {             
                // Debug.Log(newThreatLevel + " " + gameManager.GetNearestBeacon().name);

                float newMaxLeft = 0;
                float newMaxRight = 0;

                switch (newThreatLevel)
                {
                    case ThreatLevels.RedHigh:
                        newMaxRight = 6f;
                        newMaxLeft = 3;
                        break;

                    case ThreatLevels.RedLow:
                        newMaxRight = -6;
                        newMaxLeft = -9;
                        break;

                    case ThreatLevels.AmberHigh:
                        newMaxRight = -14;
                        newMaxLeft = -18;
                        break;

                    case ThreatLevels.AmberLow:
                        newMaxRight = -28;
                        newMaxLeft = -32;
                        break;


                    case ThreatLevels.YellowHigh:
                        newMaxRight = -28;
                        newMaxLeft = -32;
                        break;

                    case ThreatLevels.YellowLow:
                        newMaxRight = -40;
                        newMaxLeft = -46;
                        break;

                    case ThreatLevels.Green:
                        newMaxRight = -63;
                        newMaxLeft = -63;
                        break;
                }

                maxLeft = newMaxLeft; // Correct assignment
                maxRight = newMaxRight; // Correct assignment

                // Check the rotation direction based on the new values
                isRotatingLeft = newMaxRight < maxLeft;


                currentThreatLevel = newThreatLevel;
            }
            isMoving = true;
        }
    }

}
