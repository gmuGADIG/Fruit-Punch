using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("No longer needed; see ColorTweaker.cs")]
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
        auraSprite.color = newAura switch
        {
            AuraType.Strike => strikeColor,
            AuraType.Throw => throwColor,
            AuraType.JumpAtk => jumpColor,
            AuraType.Pearry => peariesColor,
            _ => noAuraColor
        };
    }

}
