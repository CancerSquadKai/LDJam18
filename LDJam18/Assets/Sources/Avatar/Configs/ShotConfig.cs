using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotConfig", menuName = "!!!JAM!!!/ShotConfig")]
public class ShotConfig : ScriptableObject
{
    public GameObject prefab;
    public float cooldown = 0.0625f;
}
