using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GhostAttackOrb : MonoBehaviour
{
    private float movementSpeed1 = 2.5f;
    private float movementSpeed2 = 5.0f;
    private Vector3 targetDirection;

    [SerializeField] private GameObject explosion;

    private Animator animator;

    private GameObject targetLauncher;
    private GameObject target;

    private bool lauching = false;
    private bool loading = false;
    private GhostAttackHandler attackHandler;

    // countdown timer
    private float currentTime;
    private float lifeTime = 8.0f;

    private void Start()
    {
        currentTime = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (loading)
        {
            MoveToLauncher();
        }
        else if (lauching)
        {
            MoveToTargetPos();

            if (currentTime <= 0)
            {
               DestoryOrb();
            }
            else
            {
                currentTime -= Time.deltaTime;
            }
        }  
    }


    public void SetUpOrb(GhostAttackHandler newAttackHandler, GameObject launcher, GameObject player)
    {

        animator = GetComponent<Animator>();
        animator.SetTrigger("Spawning");
        targetLauncher = launcher;
        target = player;
        attackHandler = newAttackHandler;
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        targetDirection = (targetPos - transform.position).normalized;
    }


    public void LaunchOrb()
    {
        loading = false;
        lauching = true;
        Vector3 playerPos = target.transform.position;
        SetTargetPosition(playerPos);
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }


    public void SignalLoadOrb()
    {
        loading = true;
    }


    private void MoveToLauncher()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetLauncher.transform.position, movementSpeed1 * Time.deltaTime);

        float distanceToWaypoint = Vector3.Distance(transform.position, targetLauncher.transform.position);
        if (distanceToWaypoint < 0.01f)
        {
            LaunchOrb();
        }
    }

    private void MoveToTargetPos()
    {
        // Move Ghost       
        transform.position += targetDirection * ((movementSpeed2 + 2) * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "Player")
        {
            Debug.Log("player hit!");
            PlayerManager playerManager = other.gameObject.GetComponent<PlayerManager>();
            playerManager.TakeDamge();

            DestoryOrb();
        }
        else if (other.gameObject.tag != "Ghost" && other.gameObject.tag != "Player")
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player") && other.gameObject.layer != LayerMask.NameToLayer("Ghost"))
            {
                DestoryOrb();
            }           
        }
    }

    public void DestoryOrb()
    {
        Debug.Log("ExplodingOrb");
        if (attackHandler != null)
        {
            attackHandler.RemoveOrbFromList(gameObject);
        }
        else
        {
            Debug.LogWarning("No Handler!");
        }
        
        GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
