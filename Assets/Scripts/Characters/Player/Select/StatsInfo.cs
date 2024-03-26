using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatValue { Low= 1, Mid, High}
[CreateAssetMenu(fileName = "Stat", menuName = "Stat Info" )]
public class StatsInfo : ScriptableObject
{
    public Character character;
    public StatValue health; 
    public StatValue damage; 
    public StatValue speed; 
}
