using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemsManager : MonoBehaviour
{
    [SerializeField] private Animator itemAnimator;
    [SerializeField] private GameObject ghostCanon;
    [SerializeField] private GameObject ghostDetector;
    [SerializeField] private GameObject ghostMine;
    [SerializeField] private GameObject mineDetonator;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject redDetonatorLight;
    [SerializeField] private GameObject greenDetonatorLight;
    [SerializeField] private PlayerControls playerControls;

    [SerializeField] private GhostGameManager gameManager;

    bool minePlaced = false;
    bool itemsFree = true;

    enum Items { Canon, Detector, Mine, Detonator, none };
    Items currentItem = Items.none;
    Items nextItem = Items.none;

    private GameObject setMine = null;
    private CanonHandler canonHandler = null;

    [SerializeField] private bool camoTesting = false;

    private void Awake()
    {
        PlayerInput playerInput = new PlayerInput();
        playerInput.CharacterControls.Enable();
        playerInput.CharacterControls.SwitchToCanon.performed += SwitchToCanon;
        playerInput.CharacterControls.SwitchToDetector.performed += SwitchToDetector;
        playerInput.CharacterControls.SwitchToMine.performed += SwitchToMine;
        playerInput.CharacterControls.UseCurrentItem.performed += UseCurrentItem;
        playerInput.CharacterControls.UseBeam.started += UseBeam;
        playerInput.CharacterControls.UseBeam.canceled += EndBeam;

        canonHandler = ghostCanon.GetComponent<CanonHandler>();
        canonHandler.itemsManager = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        ghostDetector.SetActive(false);
        ghostCanon.SetActive(false);
        ghostMine.SetActive(false);
        mineDetonator.SetActive(false);
    }


    public void FreeHands()
    {


        nextItem = Items.none;
        itemAnimator.SetTrigger("ChangeItem");
        itemsFree = false;


    }


    private void SwitchToCanon(InputAction.CallbackContext context)
    {
        if (!playerControls.PlayerPaused())
        {
            if (itemsFree && gameManager.GetCannonAquired() && currentItem != Items.Canon)
            {
                nextItem = Items.Canon;
                itemAnimator.SetTrigger("ChangeItem");
                itemsFree = false;
            }
        }

    }

    private void SwitchToDetector(InputAction.CallbackContext context)
    {
        if (!playerControls.PlayerPaused())
        {
            if (itemsFree && gameManager.GetDetectorAquired() && currentItem != Items.Detector)
            {
                nextItem = Items.Detector;
                itemAnimator.SetTrigger("ChangeItem");
                itemsFree = false;
            }
        }
    }

    private void SwitchToMine(InputAction.CallbackContext context)
    {
        if (!playerControls.PlayerPaused())
        {
            if (itemsFree && gameManager.GetMineAquired())
            {
                if (!minePlaced)
                {
                    if (currentItem != Items.Mine)
                    {
                        nextItem = Items.Mine;
                        itemAnimator.SetTrigger("ChangeItem");
                        itemsFree = false;
                    }
                }
                else
                {
                    if (currentItem != Items.Detonator)
                    {
                        nextItem = Items.Detonator;
                        itemAnimator.SetTrigger("ChangeItem");
                        itemsFree = false;
                    }
                }
            }
        }
    }


    public void SwitchItems()
    {
        switch (currentItem)
        {
            case Items.Canon:
                ghostCanon.SetActive(false);
                break;

            case Items.Detector:
                ghostDetector.SetActive(false);
                break;

            case Items.Mine:
                ghostMine.SetActive(false);
                break;

            case Items.Detonator:
                mineDetonator.SetActive(false);
                break;

            default:

                break;
        }

        switch (nextItem)
        {
            case Items.Canon:
                ghostCanon.SetActive(true);
                itemAnimator.SetTrigger("CannonActive");
                break;
            case Items.Detector:
                ghostDetector.SetActive(true);
                break;
            case Items.Mine:
                ghostMine.SetActive(true);
                break;
            case Items.Detonator:
                redDetonatorLight.SetActive(true);
                greenDetonatorLight.SetActive(false);
                mineDetonator.SetActive(true);
                break;
            default:

                break;
        }

        currentItem = nextItem;
        nextItem = Items.none;
        FreeItemsForUse();
    }


    private void SpawnMine()
    {
        itemsFree = false;
        Vector3 spawnPosition = ghostMine.transform.position;
        Quaternion spawnRotation = ghostMine.transform.rotation;
        // Instantiate the mine prefab at the calculated spawn position with the player's rotation
        setMine = Instantiate(minePrefab, spawnPosition, spawnRotation);
        setMine.GetComponent<MineHandler>().itemsManager = this;
        setMine.GetComponent<MineHandler>().mistAnimator = playerControls.MistCanvas.gameObject.GetComponent<Animator>();

        // Optionally, you can apply an initial force to the mine to push it away from the player
        Rigidbody mineRigidbody = setMine.GetComponent<Rigidbody>();
        if (mineRigidbody != null)
        {
            mineRigidbody.AddForce(transform.forward * 600f); // You can adjust the force as needed
        }

        // Hide the handheld Mine
        currentItem = Items.none;
        ghostMine.SetActive(false);

        minePlaced = true;
        nextItem = Items.Detonator;
        itemAnimator.SetTrigger("ChangeItem");
    }

    private void TriggerMine()
    {
        if (setMine)
        {
            redDetonatorLight.SetActive(false);
            greenDetonatorLight.SetActive(true);

            setMine.GetComponent<MineHandler>().TriggerExplosion();
            setMine = null;
            itemsFree = false;
        }
    }

    public void RegisterMineExplosion()
    {
        minePlaced = false;
        nextItem = Items.none;
        itemAnimator.SetTrigger("ChangeItem");
    }

    private void TriggerFiringAnimation()
    {
        itemAnimator.SetTrigger("FireCannonAndRecharge");
    }


    public void FireCanon()
    {
        canonHandler.FireGhostCanon();
    }

    private void UseCurrentItem(InputAction.CallbackContext context)
    {
        if (itemsFree && !playerControls.PlayerPaused())
        {
            switch (currentItem)
            {
                case Items.Canon:

                    if (canonHandler.currentMode == CanonHandler.FiringModes.Bullets)
                    {
                        if (!camoTesting)
                        {
                            itemsFree = false;
                        }
                        else 
                        {
                            itemsFree = true;
                        }                 
                        TriggerFiringAnimation();
                    }

                    break;

                case Items.Mine:
                    SpawnMine();
                    break;

                case Items.Detonator:
                    TriggerMine();
                    break;

                default:

                    break;
            }
        }
    }

    public void FreeItemsForUse()
    {
        itemsFree = true;
    }


    public void TriggerCaptureBeam(GameObject ghost)
    {
        canonHandler.SetUpBeam(ghost);
    }


    private void UseBeam(InputAction.CallbackContext context)
    {
        if (!playerControls.PlayerPaused())
        {
            canonHandler.FireGhostBeam();
        }     
    }

    private void EndBeam(InputAction.CallbackContext context)
    {
        canonHandler.EndGhostBeam();
    }
}
