using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items")]
public class ItemScriptable : ScriptableObject
{
    [Tooltip("The prefab for an item.")]
    public GameObject itemPrefab;
    [Tooltip("The amount of damage that an item will do when it hits an enemy.")]
    public int itemDamageAmount;
    /* Item Durability has been turned off until noted that it is needed.
    [Tooltip("How many swings an item would have with the player before it breaks. Treated like a health value for the items.")]
    public int itemDurability;
    
    [Tooltip("The amount of time it will take in seconds before the item is able to attack again.")]
    public float rechargeTime;
    */

}
