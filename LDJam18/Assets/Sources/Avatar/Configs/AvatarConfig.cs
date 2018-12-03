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
    public SlashConfig[] slash;
    public WindupArcView prefab_windup_arc_view;
    [Header("Dash")]
    public AnimationCurve dash_curve      = AnimationCurve.Linear(0,0,1,1);
    public DashConfig[]  dash;
    [Header("Shot")]
    public ShotConfig[]  shot;
}
