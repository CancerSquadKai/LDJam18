using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlashConfig", menuName = "!!!JAM!!!/SlashConfig")]
public class SlashConfig : ScriptableObject
{
    public float range                 = 1.0f;
    public float angle                 = 120;
    public float anticipation_duration = 1.0f;
    public float recovery_duration     = 1.0f;
    public float bump_distance         = 1.0f;
    public float bump_duration         = 1.0f;
    public int   damage                = 8;
    public float input_bufer_duration  = 0.25f;
}
