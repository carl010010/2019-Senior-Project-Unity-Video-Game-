using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

[CreateAssetMenu(fileName = "DoorSounds", menuName = "ScriptableObject/DoorSounds", order = 1)]
public class DoorSounds : ScriptableObject
{
    public Sound DoorUnlock;
    public Sound KeyInsert;
    public Sound KeyRemove;
    public Sound LockPicking;
    public Sound DoorClose;
    public Sound DoorLocked;
}
