using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashConfig", menuName = "!!!JAM!!!/DashConfig")]
public class DashConfig : ScriptableObject
{
    public float distance        = 8f;
    public float duration        = 0.125f;
    [Range(0,1)] 
    public float invulnerability = 0.5f;
    public float cooldown        = 1f;
}
