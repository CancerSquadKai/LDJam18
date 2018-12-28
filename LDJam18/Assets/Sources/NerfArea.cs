using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NerfArea : MonoBehaviour
{
	[FMODUnity.EventRef]
	public string DoorOpenEvent = "event:/DoorOpen";
	FMOD.Studio.EventInstance DoorSnd;

	[Space]

	public Animator doorAnimator;

	public GameObject otherNerfArea;
	[SerializeField] GameObject particleOnActivate;
	[SerializeField] ParticleSystem statueParticleOnActivate;
	[SerializeField] ParticleSystem statueChannelParticle;
	[SerializeField] ParticleSystem playerChannelParticle;

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
		{
            progress += Time.deltaTime / duration;
			if (playerChannelParticle != null)
			{
				if(!playerChannelParticle.isPlaying)
				{
					playerChannelParticle.Play();
				}
			}
			if (statueChannelParticle != null)
			{
				if (!statueChannelParticle.isPlaying)
				{
					statueChannelParticle.Play();
				}
			}
		}
        else
		{
            progress -= Time.deltaTime / duration;
			if (playerChannelParticle != null)
			{
				if (playerChannelParticle.isPlaying)
				{
					playerChannelParticle.Stop();
				}
			}
			if (statueChannelParticle != null)
			{
				if (statueChannelParticle.isPlaying)
				{
					statueChannelParticle.Stop();
				}
			}
		}

        progress = Mathf.Clamp01(progress);

        mesh_renderer.material.SetFloat("_Progress", 1 - progress);

        if(progress >= 1f)
        {
			if(playerChannelParticle != null)
			{
				playerChannelParticle.Stop();
			}
			if(statueChannelParticle != null)
			{
				statueChannelParticle.Stop();
			}

			if(statueParticleOnActivate != null)
			{
				statueParticleOnActivate.Play();
			}
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

			AvatarController player = FindObjectOfType<AvatarController>();
			if(player != null)
			{
				GameObject go = Instantiate(particleOnActivate, player.transform.position, particleOnActivate.transform.rotation);
				Destroy(go, 5f);
			}

			Shaker.instance.Shake(2f, 0.5f);

			DoorSnd = FMODUnity.RuntimeManager.CreateInstance(DoorOpenEvent);
			DoorSnd.start();
			if (otherNerfArea != null)
			{
				otherNerfArea.SetActive(false);
			}
			if(doorAnimator != null)
			{
				doorAnimator.SetBool("IsOpen", true);
			}
		}
    }
}
