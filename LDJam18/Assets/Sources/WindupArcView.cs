using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindupArcView : MonoBehaviour {

    private const float _WIDTH = 0.125f;

    private MeshRenderer mesh_renderer;

    private float lifetime = 0.0f;
    private float lifespan = 1.0f;

    private void Awake()
    {
        mesh_renderer = GetComponent<MeshRenderer>();
    }

    public void Init(float lifespan, float radius, Vector3 direction, float angle)
    {
        this.lifespan = lifespan;
        float size = radius * 1.0f;
        transform.localScale = Vector3.one * size;
        mesh_renderer.material.SetFloat("_Width", (1f / size) * _WIDTH);

        direction.Normalize();
        float start_angle = Mathf.Atan2(direction.z, direction.x);
        mesh_renderer.material.SetFloat("_Angle", angle);
        mesh_renderer.material.SetFloat("_StartAngle", start_angle - angle * 0.5f);
    }

    public void Update()
    {
        float dt = Time.deltaTime;
        lifetime += dt;

        {
            float progress = lifetime / lifespan;
            mesh_renderer.material.SetFloat("_Progress", progress);
        }

        if(lifetime >= lifespan)
        {
            Destroy(gameObject);
        }
    }

}
