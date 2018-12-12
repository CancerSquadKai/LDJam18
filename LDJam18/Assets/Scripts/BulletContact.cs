using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContact : MonoBehaviour
{
	[SerializeField] int damage = 5;
	[SerializeField] GameObject hitParticle;
	[SerializeField] GameObject hitWallParticle;
	[SerializeField] float bumpForce = 1f;
	[SerializeField] float bumpDuration = 0.1f;

	private void OnTriggerEnter (Collider other)
	{
		Life otherLife = other.GetComponent<Life>();
		if(otherLife != null)
		{
            // Do DMG
            if (!otherLife.isInvulnerableToBullet)
			{
                otherLife.UpdateLife(damage * -1);

				BasicEnemyController enemy = otherLife.GetComponent<BasicEnemyController>();
				if (enemy != null)
				{
					Vector3 dir = enemy.transform.position - transform.position;
					enemy.Bump(transform.forward, bumpForce, bumpDuration);
				}

				if (hitParticle != null)
				{
					GameObject hit = Instantiate(hitParticle, transform.position, hitParticle.transform.rotation);
					Destroy(hit, 2f);
				}
			}
            else
                return;
		}
		else
		{
			if (hitWallParticle != null)
			{
				GameObject hit = Instantiate(hitWallParticle, transform.position, hitParticle.transform.rotation);
				Destroy(hit, 2f);
			}
		}

		EnemyKillDetection enemyD = other.GetComponent<EnemyKillDetection>();

		if(enemyD == null)
		{
			TrailRenderer childTrail = GetComponentInChildren<TrailRenderer>();
			if(childTrail != null)
			{
				childTrail.time /= 1.5f;
				GetComponent<BulletMovement>().enabled = false;
				StartCoroutine(Shrink(0.2f, childTrail));
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	IEnumerator Shrink(float duration, TrailRenderer trail)
	{
		Vector3 initScale = transform.localScale;
		float initTrail = trail.startWidth;
		for(float i = 0; i<duration; i +=Time.deltaTime)
		{
			transform.localScale = Vector3.Lerp(initScale, Vector3.zero, i / duration);
			trail.startWidth = Mathf.Lerp(initTrail, 0f, i / duration);
			yield return null;
		}
		Destroy(gameObject);
	}
}
