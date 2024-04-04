using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAura : MonoBehaviour
{
    public enum PlayerAura
    {
        Strike,
        Throw,
        JumpAttack,
        Peary,
        None
    }

    [Header("Colors")]
    public Color strikeColor;
    public Color throwColor;
    public Color jumpColor;
    public Color peariesColor;
    public Color noAuraColor;


    public PlayerAura pAura;
    SpriteRenderer auraSprite;
    
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        auraSprite = GetComponent<SpriteRenderer>();
        setAuraColor(pAura);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setAuraColor(PlayerAura pAura)
    {
        Color newColor = noAuraColor;
        switch (pAura)
        {
            case PlayerAura.Strike:
                newColor = strikeColor;
                break;
            case PlayerAura.Throw:
                newColor = throwColor;  
                break;
            case PlayerAura.JumpAttack:
                newColor = jumpColor;
                break;
            case PlayerAura.Peary:
                newColor = peariesColor;
                break;
            case PlayerAura.None:
                newColor = noAuraColor;
                break;
        }

        auraSprite.color = newColor;

    }
}
