﻿using System.Collections;
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
	[FMODUnity.EventRef]
	public string DashEvent = "event:/SFX_Dash";
	FMOD.Studio.EventInstance DashSnd;

	public TrailRenderer  trail_renderer;
    public Animator       animator;
    public MeshRenderer   mesh_renderer_hitbox;
    public MeshRenderer[] swords;

    [ColorUsage(false, true)]
    public Color color_base;
    [ColorUsage(false, true)]
    public Color color_hit;
    public AnimationCurve hit_curve;
    public float progress_base = 0.755f;

    public Material emisive_blue_mat;

    public List<MeshRenderer> wings_renderer_group = new List<MeshRenderer>(6);

    private void Start()
    {
        trail_renderer.time = 0.0f;
        if (emisive_blue_mat)
            emisive_blue_mat.SetColor("_EmissionColor", color_base);
        Shader.SetGlobalColor("_PlayerColor", color_base);

        foreach(var wings_renderer in wings_renderer_group)
        {
            wings_renderer.materials[1] = new Material(wings_renderer.materials[1]);
        }
    }

    public void UpdateDashCooldowns(List<float> dash_cooldow_progresses)
    {
        int dash_count = Mathf.Min(wings_renderer_group.Count, dash_cooldow_progresses.Count);
        for (int dash_index = 0; dash_index < dash_count; ++dash_index)
        {
            var mr = wings_renderer_group[dash_index];
            mr.materials[1].SetFloat("_Progress", dash_cooldow_progresses[dash_index]);
            mr.transform.localRotation = Quaternion.RotateTowards(
                    mr.transform.localRotation,
                    dash_cooldow_progresses[dash_index] >= 1.0f ?
                        (dash_index % 2 == 0 ?
                            Quaternion.Euler(30 + 20 * (dash_index / 2), 40,  40) :
                            Quaternion.Euler(30 + 20 * (dash_index / 2), -40, -40)
                        ) :
                        Quaternion.Euler(0, 0, dash_index % 2 == 0 ? 30 : - 30),
                    360 * Time.deltaTime
                );
        }
    }

    public void OnDashBegin()
    {
		DashSnd = FMODUnity.RuntimeManager.CreateInstance(DashEvent);
		DashSnd.start();
		trail_renderer.time = 0.2f;
        if (animator)
        {
            animator.SetTrigger("UseDash");
            animator.SetBool("IsDashing", true);
        }
    }

    public void OnDashEnd()
    {
        trail_renderer.time = 0.0f;
        if (animator)
            animator.SetBool("IsDashing", false);
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
            Shader.SetGlobalColor("_PlayerColor", color);
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

    public void OnNerfSlash(int index)
    {
        foreach (var sword in swords)
        {
            sword.enabled = false;
            var sub_renderers = swords[index].GetComponentsInChildren<MeshRenderer>();
            foreach(var sub_renderer in sub_renderers)
                sub_renderer.enabled = false;
        }

        if (swords.Length <= index) return;

        swords[index].enabled = true;
        var sub_rnd = swords[index].GetComponentsInChildren<MeshRenderer>();
        foreach (var sub_renderer in sub_rnd)
            sub_renderer.enabled = true;
    }

    public void OnNerfDash(int index)
    {
        for (int wing_index = 0; wing_index < Mathf.Min(wings_renderer_group.Count, (wings_renderer_group.Count / 2) - (index * 2) + 1); ++wing_index)
            wings_renderer_group[wing_index].enabled = false;
    }
}
