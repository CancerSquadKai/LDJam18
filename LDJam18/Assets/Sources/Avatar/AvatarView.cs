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

    public TrailRenderer trail_renderer;

    private void Start()
    {
        trail_renderer.time = 0.0f;
    }

    public void OnDashBegin()
    {
        trail_renderer.time = 0.2f;
    }

    public void OnDashEnd()
    {
        trail_renderer.time = 0.0f;
    }
}
