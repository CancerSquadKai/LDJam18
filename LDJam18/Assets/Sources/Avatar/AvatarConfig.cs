﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarConfig", menuName = "!!!JAM!!!/AvatarConfig")]
public class AvatarConfig : ScriptableObject {

    [Header("Speeds")]
    [Tooltip("The speed of the avatar when moving.")]
    public float speed_movement = 1.0f;

    [Header("Abilities")]
    [Header("Slash")]
    public float attack_range             = 1.0f;
    public float attack_angle             = 120;
    public float attack_windup_duration   = 1.0f;
    public float attack_recovery_duration = 1.0f;
    public float attack_bump_distance     = 1.0f;
    public float attack_bump_duration     = 1.0f;


    [Header("Prefabs")]
    public WindupArcView prefab_windup_arc_view;
}
