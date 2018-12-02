using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContact : MonoBehaviour
{
	[SerializeField] int damage = 5;
	[SerializeField] GameObject hitParticle;
	private void OnTriggerEnter (Collider other)
	{
		Life otherLife = other.GetComponent<Life>();
		if(otherLife != null)
		{
            // Do DMG
            if (!otherLife.isInvulnerableToBullet)
                otherLife.UpdateLife(damage * -1);
            else
                return;
		}

        if (hitParticle != null)
        {
            GameObject hit = Instantiate(hitParticle, transform.position, hitParticle.transform.rotation);
		    Destroy(hit, 2f);
        }

		Destroy(gameObject);

	}
}
