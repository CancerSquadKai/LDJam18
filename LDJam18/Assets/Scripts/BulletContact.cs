using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContact : MonoBehaviour
{
	[SerializeField] int damage = 5;
	private void OnTriggerEnter (Collider other)
	{
		Life otherLife = other.GetComponent<Life>();
		if(otherLife != null)
		{
			// Do DMG
			otherLife.UpdateLife(damage * -1);
			Destroy(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
