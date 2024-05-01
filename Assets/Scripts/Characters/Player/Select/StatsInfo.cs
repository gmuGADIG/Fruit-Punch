using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatValue
{
    Low= 1,
    Mid=2,
    High=3
}

[CreateAssetMenu(fileName = "Stat", menuName = "Stat Info" )]
public class StatsInfo : ScriptableObject
{
    public StatValue health; 
    public StatValue damage; 
    public StatValue speed; 
}
