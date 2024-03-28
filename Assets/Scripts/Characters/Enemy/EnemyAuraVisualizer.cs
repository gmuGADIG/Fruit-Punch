using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAuraVisualizer : MonoBehaviour
{
    public AuraType currentAura;

    [Header ("Colors")]
    public Color strikeColor;
    public Color throwColor;
    public Color jumpColor;
    public Color peariesColor;
    public Color invincibleColor;

    Color noAuraColor = Color.gray;
    SpriteRenderer auraSprite;

    // Start is called before the first frame update
    void Start()
    {
        auraSprite = GetComponent<SpriteRenderer>();
        //code to get current Aura from whatever script decides it.
        setAuraColor(currentAura);
    }

    public void setAuraColor( AuraType newAura)
    {
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
