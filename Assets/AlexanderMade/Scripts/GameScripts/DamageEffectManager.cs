using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffectManager : MonoBehaviour
{
    [SerializeField] private Material damgeEffect;

    private float currentIntensity = 0.0f;
    private float setIntensity = 0.0f;
    private float lFiveDamageIntensity = 0.88f;
    private float lFourDamageIntensity = 0.85f;
    private float lThreeDamageIntensity = 0.8f;
    private float lTwoDamageIntensity = 0.75f;
    private float lOneDamageIntensity = 0.7f;

    private float intensityIncreaseRate = 0.1f;
    private float intensityDecreaseRate = 0.08f;

    private int damageCounter = 0;

    private bool takingDamage = false;
    private bool damaged = false;
    private bool healing = false;

    private float timer = 0f;
    private float triggerTime = 20.0f; 

    private void Awake()
    {
        damgeEffect.SetFloat("_FullScreenIntensity", 0.0f);
    }

    private void Update()
    {
        if (takingDamage)
        {
            currentIntensity += intensityIncreaseRate * Time.deltaTime;
            damgeEffect.SetFloat("_FullScreenIntensity", currentIntensity);
            if (currentIntensity >= setIntensity)
            {
                currentIntensity = setIntensity;
                takingDamage = false;
                damaged = true;
            }
        }
        else if (damaged)
        {
            currentIntensity -= intensityDecreaseRate * Time.deltaTime;
            damgeEffect.SetFloat("_FullScreenIntensity", currentIntensity);

            if (currentIntensity <= 0.0f)
            {
                currentIntensity = 0.0f;
                damaged = false;
                healing = true;
            }
        }
        else if (healing)
        {
            timer += Time.deltaTime;

            if (timer >= triggerTime)
            {
                damageCounter--;

                if (damageCounter <= 0)
                {
                    damageCounter = 0;
                    healing = false;
                }
            }
        }     
    }

    public void RegisterDamage()
    {
        switch (damageCounter)
        {
            case 4:
                setIntensity = lFiveDamageIntensity;
                break;
            case 3:
                setIntensity = lFourDamageIntensity;
                break;
            case 2:
                setIntensity = lThreeDamageIntensity;
                break;
            case 1:
                setIntensity = lTwoDamageIntensity;
                break;
            case 0:
                setIntensity = lOneDamageIntensity;
                break;
            default:
                setIntensity = lFiveDamageIntensity;
                break;
        }

        damageCounter++;

        currentIntensity = setIntensity;
        takingDamage = true;
        damaged = false;
        healing = false;
    }


}
