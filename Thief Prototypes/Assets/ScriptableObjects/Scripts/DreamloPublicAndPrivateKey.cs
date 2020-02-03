using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoorSounds", menuName = "ScriptableObject/DreamloPublicAndPrivateKey", order = 1)]
public class DreamloPublicAndPrivateKey : ScriptableObject {

    public string privateCode = "";
    public string publicCode = "";
}
