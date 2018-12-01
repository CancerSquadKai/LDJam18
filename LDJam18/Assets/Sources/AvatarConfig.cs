using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarConfig", menuName = "!!!JAM!!!/AvatarConfig")]
public class AvatarConfig : ScriptableObject {

    [Header("Speeds")]
    [Tooltip("The speed of the avatar when moving.")]
    public float speed_movement = 1.0f;


}
