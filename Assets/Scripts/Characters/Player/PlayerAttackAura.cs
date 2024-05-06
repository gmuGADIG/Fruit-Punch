using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class PlayerAttackAura : MonoBehaviour
{
    [Header("Colors")]
    public Color strikeColor;
    public Color throwColor;
    public Color jumpColor;
    public Color peariesColor;
    public Color noAuraColor;

    ParticleSystem particles;
    
    Player player;

    void Start()
    {
        this.GetComponentOrError(out particles);
        player = GetComponentInParent<Player>();
        SetAuraColor(PlayerState.Normal);
        player.OnPlayerStateChange += SetAuraColor;
    }

    /// <summary>
    /// Switches sprite color base on PlayerState
    /// </summary>
    void SetAuraColor(PlayerState newState)
    {
        Color newColor = newState switch
        {
            PlayerState.Strike1 => strikeColor,
            PlayerState.Strike2 => strikeColor,
            PlayerState.Strike3 => strikeColor,
            PlayerState.Grabbing => throwColor,
            PlayerState.JumpStrike => jumpColor,
            PlayerState.Pearry => peariesColor,
            PlayerState.Normal => noAuraColor,
            _ => noAuraColor
        };

        particles.startColor = newColor;
        particles.Play();
    }
}
