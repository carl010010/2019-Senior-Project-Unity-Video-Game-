using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelRequirments", menuName = "ScriptableObject/LevelRequirments", order = 1)]
public class LevelRequirments : ScriptableObject
{
    [Range(0,1)]
    public float GoldPercentage;
}
