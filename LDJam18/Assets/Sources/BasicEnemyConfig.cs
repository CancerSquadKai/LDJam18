using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicEnemyConfig", menuName = "!!!JAM!!!/BasicEnemyConfig")]
public class BasicEnemyConfig : ScriptableObject
{
    [Header("Speeds")]
    [Tooltip("The speed of the enemy when moving.")]
    public float speed_movement = 1.0f;

    [Header("Attack")]
    [Tooltip("The distance to target required so enemy starts his attack.")]
    public float attack_agro_range        =  1.0f;
    public float attack_range             = 1.0f;
    public float attack_windup_duration   = 1.0f;
    public float attack_recovery_duration = 1.0f;
    public float attack_bump_distance     = 1.0f;
    public float attack_bump_duration     = 1.0f;

    [Header("Prefabs")]
    public WindupCircularView prefab_windup_circular_view;
}
