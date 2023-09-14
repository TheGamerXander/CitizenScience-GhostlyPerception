using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerShotDataRecorder : MonoBehaviour
{
    private GhostGameManager gameManager;
    private DataManager dataManager;
    private GameObject playerCamera;
    private GameObject activeGhost;
    private VisibilityChecker ghostInfo;

    private string timeOfShot;
    private string timeOfCurrentSighting;

    private string timeOfCollision;

    private string playerPosAtSpawnTime;
    private string playerRotAtSpawnTime;

    private string playerPosAtCollisionTime;
    private string playerRotAtCollisionTime;

    private string ghostName;
    private string ghostCamo;

    private string firstTimeOnScreen;
    private string numTimesOnScreen;
    private string totalScreenTimeInSeconds;

    private string ghostPosAtSpawnTime;
    private string ghostRotAtSpawnTime;

    private string ghostMovementSpeed;
    private string ghostRotationSpeed;

    private string ghostRotatingAtShotTime;
    private string ghostRotatingAtCollisionTime;

    private string ghostScale;

    private string rayHitPoint;
    private string ghostHitByRay;

    private string ghostPosAtCollisionTime;
    private string ghostRotAtCollisionTime;

    private string ghostHitByShot;
    private string collisionHitPoint;

    public bool setUp = false;

    public void RecordFiringData(GameObject newPlayerCamera, GhostGameManager newGameManager, Vector3 hitPointOfRay, bool raycastHitGhost)
    {
        playerCamera = newPlayerCamera;
        gameManager = newGameManager;
        dataManager = gameManager.GetDataManager();
        activeGhost = gameManager.GetCurrentGhost();

        if (activeGhost != null)
        {
            ghostInfo = activeGhost.GetComponent<VisibilityChecker>();

            if (ghostInfo != null)
            {
                timeOfShot = dataManager.GetCurrentTIme();

                timeOfCurrentSighting = gameManager.GetTimeOfCurrentSighting();

                playerPosAtSpawnTime = dataManager.ReplaceCommasWithSemicolon(playerCamera.transform.position.ToString());
                playerRotAtSpawnTime = dataManager.ReplaceCommasWithSemicolon(playerCamera.transform.rotation.ToString());

                rayHitPoint = dataManager.ReplaceCommasWithSemicolon(hitPointOfRay.ToString());

                ghostName = dataManager.ReplaceCommasWithSemicolon(activeGhost.name);
                ghostCamo = dataManager.ReplaceCommasWithSemicolon(ghostInfo.GetCurrentCamo());

                firstTimeOnScreen = dataManager.ReplaceCommasWithSemicolon(ghostInfo.GetFirstTimeOnScreen());
                numTimesOnScreen = dataManager.ReplaceCommasWithSemicolon(ghostInfo.GetNumTimesOnScreen());
                totalScreenTimeInSeconds = dataManager.ReplaceCommasWithSemicolon(ghostInfo.GetTotalScreenTime());

                ghostHitByRay = dataManager.ReplaceCommasWithSemicolon(raycastHitGhost.ToString());

                ghostMovementSpeed = ghostInfo.GetMovmentHandler().GetCurrentSpeed();
                ghostRotationSpeed = ghostInfo.GetMovmentHandler().GetCurrentRotationSpeed();
                ghostRotatingAtShotTime = ghostInfo.GetMovmentHandler().GetIsRotating();

                ghostScale = dataManager.ReplaceCommasWithSemicolon(ghostInfo.GetScale());

                ghostPosAtSpawnTime = dataManager.ReplaceCommasWithSemicolon(activeGhost.transform.position.ToString());
                ghostRotAtSpawnTime = dataManager.ReplaceCommasWithSemicolon(activeGhost.transform.rotation.ToString());

                setUp = true;
            }
        }
    }

    public void RecordCollisionData(bool hitByShot)
    {
        if (setUp)
        {
            timeOfCollision = dataManager.GetCurrentTIme();

            playerPosAtCollisionTime = dataManager.ReplaceCommasWithSemicolon(playerCamera.transform.position.ToString());
            playerRotAtCollisionTime = dataManager.ReplaceCommasWithSemicolon(playerCamera.transform.rotation.ToString());

            ghostHitByShot = hitByShot.ToString();
            collisionHitPoint = dataManager.ReplaceCommasWithSemicolon(transform.position.ToString());

            ghostRotAtCollisionTime = ghostInfo.GetMovmentHandler().GetIsRotating();
            ghostPosAtCollisionTime = dataManager.ReplaceCommasWithSemicolon(activeGhost.transform.position.ToString());
            ghostRotAtCollisionTime = dataManager.ReplaceCommasWithSemicolon(activeGhost.transform.rotation.ToString());
            ghostRotatingAtCollisionTime = ghostInfo.GetMovmentHandler().GetIsRotating();

            List<string[]> newDataRows = new List<string[]>
            {
                new string[] {
                    "AtSpawn", timeOfShot,
                    ghostName, ghostCamo,
                    firstTimeOnScreen, numTimesOnScreen, totalScreenTimeInSeconds, timeOfCurrentSighting,
                    ghostMovementSpeed, ghostRotationSpeed, ghostRotatingAtShotTime, ghostScale,
                    ghostHitByRay, rayHitPoint,
                    ghostPosAtSpawnTime, ghostRotAtSpawnTime,
                    playerPosAtSpawnTime, playerRotAtSpawnTime,
                    gameManager.GetDataManager().GetGameStartTIme()
                },

                new string[] {
                    "AtCollision", timeOfCollision,
                    ghostName, ghostCamo,
                    firstTimeOnScreen, numTimesOnScreen, totalScreenTimeInSeconds, timeOfCurrentSighting,
                    ghostMovementSpeed, ghostRotationSpeed, ghostRotatingAtCollisionTime, ghostScale,
                    ghostHitByShot, collisionHitPoint,
                    ghostPosAtCollisionTime, ghostRotAtCollisionTime,
                    playerPosAtCollisionTime, playerRotAtCollisionTime,
                    gameManager.GetDataManager().GetGameStartTIme()
                }
            };

            dataManager.WriteToCSV(newDataRows);
        }

        Destroy(gameObject);
    }
}
