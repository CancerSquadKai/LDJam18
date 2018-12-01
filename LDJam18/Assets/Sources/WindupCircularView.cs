using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindupCircularView : MonoBehaviour {

    private const float _WIDTH = 0.125f;

    private MeshRenderer mesh_renderer;

    private float lifetime = 0.0f;
    private float lifespan = 1.0f;

    private void Awake()
    {
        mesh_renderer = GetComponent<MeshRenderer>();
    }

    public void Init(float lifespan, float radius)
    {
        this.lifespan = lifespan;
        float size = radius * 2.0f;
        transform.localScale = Vector3.one * size;
        mesh_renderer.material.SetFloat("_Width", (1f / size) * _WIDTH);
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
