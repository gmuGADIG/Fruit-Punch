using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

/// <summary>
/// Controls a ColorTweaker shader for both aura effects and damage flashes. <br/>
/// Damage flashes are handled automatically by attaching to the onHurt event in a parent Health component. <br/>
/// Aura effects are not done automatically, as they're handled differently by enemies and players.
/// </summary>
public class ColorTweaker : MonoBehaviour
{
    public Renderer spriteRenderer;

    [Header("Aura Colors")]
    public Color strikeColor;
    public Color throwColor;
    public Color jumpColor;
    public Color peariesColor;
    public Color invulnerableColor;
    public Color noAuraColor;

    [Header("Damage Colors")]
    public Color damageColor;
    public Color noDamageColor;
    
    void Start()
    {
        // attach damage flash to parent Health component
        if (transform.parent.TryGetComponent<Health>(out var health))
        {
            health.onHurt += _ => DamageFlash();
            health.onAuraChange += SetAuraColor;
            SetAuraColor(health.vulnerableTypes);
        }

    }

    /// <summary>
    /// Switches outline color according to an attack's or invulnerability's aura. <br/>
    /// For disabling auras, it's recommended you use RemoveAuraColor() 
    /// </summary>
    public void SetAuraColor(AuraType aura)
    {
        Color newColor = aura switch
        {
            AuraType.Strike => strikeColor,
            AuraType.Throw => throwColor,
            AuraType.JumpAtk => jumpColor,
            AuraType.Pearry => peariesColor,
            AuraType.Everything => invulnerableColor,
            _ => noAuraColor
        };

        spriteRenderer.material.SetColor("_OutlineColor", newColor);
    }

    public void RemoveAuraColor()
    {
        spriteRenderer.material.SetColor("_OutlineColor", noAuraColor);
    }
    
    void DamageFlash()
    {
        StartCoroutine(Anim());

        IEnumerator Anim()
        {
            spriteRenderer.material.SetColor("_OverwriteColor", damageColor);
            yield return new WaitForSeconds(.15f);
            spriteRenderer.material.SetColor("_OverwriteColor", noDamageColor);
        }
    }
}
