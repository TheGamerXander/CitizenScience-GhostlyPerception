using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GhostBullet : MonoBehaviour
{
    private bool targetAsigned = false;
    private float movementSpeed = 10.5f;
    private Vector3 direction;

    [SerializeField] private GameObject missExplosion;
    [SerializeField] private GameObject hitExplosion;
    [SerializeField] private GameObject gBarrierExplosion;
    [SerializeField] private PlayerShotDataRecorder dataRecorder;

    enum ExplosionTypes { Spawn, Miss, Hit, BarrierHit};
    bool hitGhost = false;

    private void Start()
    {      
        SpawnExplosion(ExplosionTypes.Spawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetAsigned)
        {
            MoveToTargetPos();
        }     
    }

    private void SpawnExplosion(ExplosionTypes type)
    {
        // Instantiate the projectile at the weapon's position
        GameObject newExplosion;
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;

        switch (type)
        {
            case ExplosionTypes.Spawn:
                newExplosion = Instantiate(hitExplosion, pos, transform.rotation);
                newExplosion.transform.localScale = scale;
                break;
            case ExplosionTypes.Miss:
                newExplosion = Instantiate(missExplosion, pos, transform.rotation);
                newExplosion.transform.localScale = scale + new Vector3(0.35f, 0.35f, 0.35f);      
                break;
            case ExplosionTypes.Hit:
                newExplosion = Instantiate(hitExplosion, pos, transform.rotation);
                newExplosion.transform.localScale = scale + new Vector3(0.55f, 0.35f, 0.35f);
                break;
            case ExplosionTypes.BarrierHit:
                newExplosion = Instantiate(gBarrierExplosion, pos, transform.rotation);
                newExplosion.transform.localScale = scale + new Vector3(0.35f, 0.35f, 0.35f);
                break;

            default:
               
                break;
        }
    }

   
    public void SetTargetPosition(Vector3 targetPos)
    {
        direction = (targetPos - transform.position).normalized;
        targetAsigned = true;
    }

    private void MoveToTargetPos()
    {
        // Move Ghost       
        transform.position += direction * movementSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {     
            if(other.gameObject.name != "Player")
            {
                if (other.gameObject.tag == "Ghost")
                {

                   
                    if(other.gameObject.name == "Ghost")
                    {
                        hitGhost = true;
                    }


                    if (other.gameObject.name == "GhostAttackOrb")
                    {
                        SpawnExplosion(ExplosionTypes.Hit);

                        GhostAttackOrb orbControls = other.GetComponent<GhostAttackOrb>();

                        if (orbControls != null)
                        {
                            orbControls.DestoryOrb();
                        }
                    }
                    else
                    {
                        GameObject newHitGhost = other.gameObject;
                        GhostMovementLinker linker = newHitGhost.GetComponent<GhostMovementLinker>();

                        if (linker != null)
                        {
                            GhostMovementHandler ghostMovementHandler = linker.GetMovementHandler();
                            SpawnExplosion(ExplosionTypes.Hit);
                            ghostMovementHandler.RegisterGhostHit();
                        }
                    }
                }
                else if (other.gameObject.name == "GhostOrb")
                {
                    SpawnExplosion(ExplosionTypes.BarrierHit);
                }
                else
                {
                    SpawnExplosion(ExplosionTypes.Miss);
                }

                if (other.gameObject.tag == "Dummy")
                {
                    if (other.gameObject.name == "DummyGhost")
                    {
                        other.gameObject.GetComponent<DummyGhostHandler>().ChangeCamo();
                    }
                }

                RecoredData();
            }           
        }        
    }

    private void RecoredData()
    {
        dataRecorder.RecordCollisionData(hitGhost);
    }
}
 