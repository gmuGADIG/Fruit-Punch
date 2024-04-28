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


    SpriteRenderer auraSprite;
    
    Player player;
    //PlayerState currState;
    // Start is called before the first frame update
    void Start()
    {
        auraSprite = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<Player>();
        SetAuraColor(PlayerState.Normal);
        player.OnPlayerStateChange += SetAuraColor;
    }

    /// <summary>
    /// Switches sprite color base on PlayerState
    /// </summary>
    /// <param name="pAura"></param>
    void SetAuraColor(PlayerState pAura)
    {
        Color newColor = noAuraColor;
        switch (pAura)
        {
            case PlayerState.Strike1:
                newColor = strikeColor;
                break;
            case PlayerState.Strike2:
                newColor = strikeColor;
                break;
            case PlayerState.Strike3:
                newColor = strikeColor;
                break;
            case PlayerState.Grabbing:
                newColor = throwColor;  
                break;
            case PlayerState.JumpStrike:
                newColor = jumpColor;
                break;
            case PlayerState.Pearry:
                newColor = peariesColor;
                break;
            case PlayerState.Normal:
                newColor = noAuraColor;
                break;
            
        }

        auraSprite.color = newColor;

    }
}
