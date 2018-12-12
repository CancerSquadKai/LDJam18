using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
	[SerializeField] WindupCircularView aoEView;
	[SerializeField] float windupDuration;
	[SerializeField] float attackRange;

	[SerializeField] int attackDamage = 30;
	[SerializeField] float bumpDistance = 15f;
	[SerializeField] float bumpDuration = 0.25f;

	[SerializeField] float shakeDuration = 0.5f;
	[SerializeField] float shakeIntensity = 1.5f;
	[SerializeField] float shakeRange = 20f;
	[SerializeField] GameObject mortarParticle;

	//[SerializeField] Animator anim;

	float timer = 0f;

	Vector3 initialPos;
	Vector3 targetPos;

	AvatarController player;

	bool hasSpawned = false;
	// Use this for initialization
	void Start ()
	{
		initialPos = transform.position;
		player = FindObjectOfType<AvatarController>();
		if(player != null)
		{
			targetPos = player.transform.position;
		}

		var windup_view = Instantiate(aoEView);
		windup_view.transform.position = targetPos;
		windup_view.Init(windupDuration, attackRange);
		//Destroy(gameObject, 6.01f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		transform.position = Vector3.Lerp(initialPos, targetPos, timer / windupDuration);
		if(transform.position == targetPos && !hasSpawned)
		{
			GameObject particle = Instantiate(mortarParticle, transform.position, mortarParticle.transform.rotation);
			// Deal DMG + Bump
			Destroy(particle, 2f);

			AvatarController player = FindObjectOfType<AvatarController>();
			if(player != null)
			{
				Vector3 toPlayer = player.transform.position - targetPos;

				//Debug.Log("To Player : " + toPlayer);
				if(toPlayer.magnitude < attackRange)
				{
					player.Bump(toPlayer.normalized, bumpDistance, bumpDuration);
					Life playerLife = player.GetComponent<Life>();

					if(playerLife != null)
					{
						playerLife.UpdateLife(attackDamage * -1);
					}
				}

				if(toPlayer.magnitude < shakeRange)
				{
					Shaker.instance.Shake(shakeDuration, shakeIntensity);
				}
			}
			hasSpawned = true;
		}
	}
}
