using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContact : MonoBehaviour
{

	private void OnTriggerEnter (Collider other)
	{
		Life otherLife = other.GetComponent<Life>();
		if(otherLife != null)
		{
			// Do DMG
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
