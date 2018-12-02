using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarConfig", menuName = "!!!JAM!!!/AvatarConfig")]
public class AvatarConfig : ScriptableObject {

    [Header("Speeds")]
    [Tooltip("The speed of the avatar when moving.")]
    public float speed_movement = 1.0f;

    [Header("Abilities")]
    [Header("Slash")]
    public float attack_range                 = 1.0f;
    public float attack_angle                 = 120;
    public float attack_windup_duration       = 1.0f;
    public float attack_recovery_duration     = 1.0f;
    public float attack_bump_distance         = 1.0f;
    public float attack_bump_duration         = 1.0f;
    public int   slash_damage                 = 8;
    public float slash_input_bufer_duration = 0.25f;

    [Header("Slash")]
    public AnimationCurve dash_curve      = AnimationCurve.Linear(0,0,1,1);
    public float dash_distance            = 8f;
    public float dash_duration            = 0.125f;
    [Range(0,1)]
    public float dash_invulnerability     = 0.5f;
    public float dash_cooldown            = 1f;


    [Header("Prefabs")]
    public WindupArcView prefab_windup_arc_view;
}
