using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarView : MonoBehaviour {

    public Vector2 position
    {
        get { return transform.position; }
        set {
            transform.position = new Vector3(
                value.x,
                0,
                value.y
            );
        }
    }

}
