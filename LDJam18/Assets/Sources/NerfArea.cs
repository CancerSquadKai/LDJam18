using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NerfArea : MonoBehaviour {

    private MeshRenderer mesh_renderer;

    AvatarController avatar;

    public float range;

    private float progress = 0;

    public float duration = 4;

    public enum NerfType
    {
        SLASH,
        DASH,
        SHOT
    }

    public NerfType nerfType;

    private void OnValidate()
    {
        if (!mesh_renderer)
            mesh_renderer = GetComponentInChildren<MeshRenderer>();
        if (mesh_renderer)
            mesh_renderer.transform.localScale = Vector3.one * range * 2;
    }

    private void Start()
    {
        if (!mesh_renderer)
            mesh_renderer = GetComponentInChildren<MeshRenderer>();
        avatar = FindObjectOfType<AvatarController>();
        progress = 0;
    }

    void Update ()
    {
        if(progress >= 1)
        {
            return;
        }

        var dist =
            Vector3.Distance(
                transform.position,
                avatar.transform.position
            );

        if(dist < range)
            progress += Time.deltaTime / duration;
        else
            progress -= Time.deltaTime / duration;

        progress = Mathf.Clamp01(progress);

        mesh_renderer.material.SetFloat("_Progress", 1 - progress);

        if(progress >= 1f)
        {
            switch (nerfType)
            {
                case NerfType.SLASH:
                    {
                        avatar.model.NerfSlash();
                    }
                    break;
                case NerfType.DASH:
                    {
                        avatar.model.NerfDash();
                    }
                    break;
                case NerfType.SHOT:
                    {
                        avatar.model.NerfShot();
                    }
                    break;
            }
        }
    }
}
