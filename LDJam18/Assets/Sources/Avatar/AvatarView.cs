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
    public Animator      animator;
    public MeshRenderer  mesh_renderer_hitbox;

    [ColorUsage(false, true)]
    public Color color_base;
    [ColorUsage(false, true)]
    public Color color_hit;
    public AnimationCurve hit_curve;
    public float progress_base = 0.755f;

    public Material emisive_blue_mat;

    private void Start()
    {
        trail_renderer.time = 0.0f;
        if (emisive_blue_mat)
            emisive_blue_mat.SetColor("_EmissionColor", color_base);
    }

    public void OnDashBegin()
    {
        trail_renderer.time = 0.2f;
        if(animator)
            animator.SetTrigger("UseDash");
    }

    public void OnDashEnd()
    {
        trail_renderer.time = 0.0f;
    }

    public IEnumerator GotHitCoroutine()
    {
        float DURATION = 0.5f;
        float progress = 0.0f;
        float speed = 1f / DURATION;
        while (progress < 1f)
        {
            progress    += Time.deltaTime;
            Color color = Color.Lerp(color_hit, color_base, progress);
            mesh_renderer_hitbox.material.SetColor("_ColorDanger", color);
            mesh_renderer_hitbox.material.SetFloat("_Progress", hit_curve.Evaluate(progress) * progress_base);
            mesh_renderer_hitbox.material.SetFloat("_BackgroundOpacity", 1 - hit_curve.Evaluate(progress));
            if(emisive_blue_mat)
                emisive_blue_mat.SetColor("_EmissionColor", color);
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnAttack()
    {
        if (animator)
            animator.SetTrigger("UseContactAttack");
    }

    public void UpdateAiming(bool isAiming)
    {
        if (animator)
            animator.SetBool("IsAiming", isAiming);
    }

    public void OnShoot()
    {
        if (animator)
            animator.SetTrigger("UseShoot");
    }
}
