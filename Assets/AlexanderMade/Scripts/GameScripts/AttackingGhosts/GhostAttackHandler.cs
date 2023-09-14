using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GhostAttackHandler : MonoBehaviour
{

    [SerializeField] private AttackingGhostHandler attackingGhost;

    [SerializeField] private GameObject attackOrbPrefab;
    [SerializeField] private List<GameObject> launchers;
    public int currentOrbIndex = 0;
    private List<int> launcherOrder = new List<int>();
    public List<GameObject> activeOrbs = new List<GameObject>();
    public List<GameObject> orbsToLaunch = new List<GameObject>();

    public enum AttackTypes { One, Two, Three, Four};
    private AttackTypes currentAttack = AttackTypes.One;

    public List<AttackTypes> attackOrder = new List<AttackTypes>();

    private GameObject player;

    private bool attackTriggered = false;
    private bool attacking = false;

    // countdown timer
    public float countDownTime = 1.5f; 
    private float currentTime;

    bool windingDown = false;
    bool returning = false;

    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject attackGhost;
    [SerializeField] private Animator ghostShieldAnimator;

    int attackIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

        foreach (var launcher in launchers)
        {
            launcher.GetComponent<Renderer>().enabled = false;
        }

        attackOrder.Add(AttackTypes.One);
        attackOrder.Add(AttackTypes.Two);
        attackOrder.Add(AttackTypes.Three);
        attackOrder.Add(AttackTypes.Four);
        attackOrder.Add(AttackTypes.Four);
        attackOrder.Add(AttackTypes.Four);


        int n = attackOrder.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            AttackTypes temp = attackOrder[i];
            attackOrder[i] = attackOrder[j];
            attackOrder[j] = temp;
        }
    }

    private void Update()
    {
        if (attackTriggered && !attacking)
        {
            if (activeOrbs.Count <= 0)
            {
                WindDown();
            }
            else if (orbsToLaunch.Count > 0)
            {
                if (currentTime <= 0)
                {
                    switch (currentAttack)
                    {
                        case AttackTypes.One:
                            currentTime = 0.8f;
                            break;

                        case AttackTypes.Two:
                            currentTime = 1.8f;
                            break;

                        case AttackTypes.Three:
                            currentTime = 1.4f;
                            break;

                        default:
                            currentTime = countDownTime;
                            break;
                    }


                    SetUpOrb();
                }
                else
                {
                    currentTime -= Time.deltaTime;
                }
            }
        }
        else if (windingDown)
        {
            if (currentTime <= 0)
            {               
                SingnalReturn();
            }
            else
            {
                currentTime -= Time.deltaTime;
               
            }    
        }
        else if (returning)
        {
            if (currentTime <= 0)
            {
                EndAttack();
            }
            else
            {
                currentTime -= Time.deltaTime;

            }       
        }
    }

    private void SetUpOrb()
    {
        GameObject currentProjectile = orbsToLaunch[0];
        if (currentProjectile != null)
        {
            GhostAttackOrb attackOrb = currentProjectile.GetComponent<GhostAttackOrb>();

            if (attackOrb != null)
            {
                GhostAttackHandler ghostAttackHandler = GetComponent<GhostAttackHandler>();

                if (ghostAttackHandler != null)
                {
                    attackOrb.SetUpOrb(ghostAttackHandler, launchers[launcherOrder[currentOrbIndex]], player);
                }               
            }
        }     
        currentOrbIndex++;
        orbsToLaunch.RemoveAt(0);
    }

    private void EndAttack()
    {
        // Clear the lists
        activeOrbs.Clear();
        orbsToLaunch.Clear();
        returning = false;
        attackGhost.GetComponent<Renderer>().enabled = true;
        attackingGhost.EndAttack();
        attackIndex++;
    }

    private void WindDown()
    {
        attackTriggered = false;
        attacking = false;
        windingDown = true;
        countDownTime = 0.3f;
        currentTime = countDownTime;
    }

    private void SingnalReturn()
    {
        windingDown = false;
        returning = true;
        countDownTime = 1.5f;
        currentTime = countDownTime;

        Instantiate(explosion, transform.position, transform.rotation);
        HideAttackGhost();
        
    }


    public void CreateAttack(GameObject playerTarget)
    {      
        player = playerTarget;
        attackTriggered = true;
        attacking = true;
        currentOrbIndex = 0;
        countDownTime = 1.0f;

        if (attackIndex >= attackOrder.Count)
        {
            attackIndex = 0;
        }

        currentAttack = attackOrder[attackIndex];


        switch (currentAttack)
        {
            case AttackTypes.One:
                SpawnAttackOne();
                break;

            case AttackTypes.Two:
                SpawnAttackTwo();
                break;
            
            case AttackTypes.Three:
                SpawnAttackThree();
                     break;

            case AttackTypes.Four:
                SpawnAttackFour();
                break;

            default:
                print("Unexpected attack type");
                break;
        }     
    }


    private void SpawnAttackOne()
    {
        int[] launcherToAdd = { 0, 1, 2, 3, 4};
        launcherOrder.AddRange(launcherToAdd);
 

        for (int i = 0; i < 5; i++)
        {
            // Instantiate the projectile at the weapon's position
            GameObject newProjectile = Instantiate(attackOrbPrefab, transform.position, transform.rotation);
            activeOrbs.Add(newProjectile);
        }

        foreach (GameObject Orb in activeOrbs)
        {
            orbsToLaunch.Add(Orb);
        }

        attacking = false;
    }

    private void SpawnAttackTwo()
    {
        int[] launcherToAdd = { 2, 0, 4};
        launcherOrder.AddRange(launcherToAdd);

        for (int i = 0; i < 3; i++)
        {
            // Instantiate the projectile at the weapon's position
            GameObject newProjectile = Instantiate(attackOrbPrefab, transform.position, transform.rotation);
            activeOrbs.Add(newProjectile);
        }

        foreach (GameObject Orb in activeOrbs)
        {
            orbsToLaunch.Add(Orb);
        }

        attacking = false;
    }

    private void SpawnAttackThree()
    {
        int[] launcherToAdd = { 2, 2, 2};
        launcherOrder.AddRange(launcherToAdd);

        for (int i = 0; i < 3; i++)
        {
            // Instantiate the projectile at the weapon's position
            GameObject newProjectile = Instantiate(attackOrbPrefab, transform.position, transform.rotation);
            activeOrbs.Add(newProjectile);
        }

        foreach (GameObject Orb in activeOrbs)
        {
            orbsToLaunch.Add(Orb);
        }

        attacking = false;
    }

    private void SpawnAttackFour()
    {
        int[] launcherToAdd = {2};
        launcherOrder.AddRange(launcherToAdd);

        for (int i = 0; i < 1; i++)
        {
            // Instantiate the projectile at the weapon's position
            GameObject newProjectile = Instantiate(attackOrbPrefab, transform.position, transform.rotation);
            activeOrbs.Add(newProjectile);
        }

        foreach (GameObject Orb in activeOrbs)
        {
            orbsToLaunch.Add(Orb);
        }

        attacking = false;
    }

    public void RemoveOrbFromList(GameObject orb)
    {
        activeOrbs.Remove(orb);
    }

    private void HideAttackGhost()
    {
        attackGhost.GetComponent<Renderer>().enabled = false;
        ghostShieldAnimator.SetTrigger("DeSpawn");
    }
}
