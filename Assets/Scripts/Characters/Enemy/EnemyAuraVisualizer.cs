using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAuraVisualizer : MonoBehaviour
{
/// <summary>
///  Take the aura types from health script and makes the sprite a color
/// </summary>
  

    [Header ("Colors")]
    public Color strikeColor;
    public Color throwColor;
    public Color jumpColor;
    public Color peariesColor;

    Color noAuraColor;
    SpriteRenderer auraSprite;
    Health parentHealth;

    public static event Action auraChange; //Not sure if aura changes but this should allow another script to change it if needed
    
    private void OnEnable()
    {
        auraChange += setAuraColor;
    }
    // Start is called before the first frame update
    void Start()
    {
         
        parentHealth = GetComponentInParent<Health>();
        auraSprite = GetComponent<SpriteRenderer>();
        noAuraColor = auraSprite.color;

        setAuraColor();
    }

    void setAuraColor()
    {
        AuraType newAura = parentHealth.vulnerableTypes;
        switch (newAura)
        {
            case AuraType.Strike:
                auraSprite.color = strikeColor;
                break;
            case AuraType.Throw:
                auraSprite.color = throwColor;
                break;
            case AuraType.JumpAtk:
                auraSprite.color = jumpColor;
                break;
            case AuraType.Pearry:
                auraSprite.color = peariesColor;
                break;
            default:
                auraSprite.color = noAuraColor;
                break;
        }

    }

}
