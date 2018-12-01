using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] GameObject spawnedObject;
	[SerializeField] bool shareOrientation;
	[SerializeField] ParticleSystem spawnParticle;

	public void SpawnObject()
	{
		if(spawnedObject != null)
		{
			GameObject go = Instantiate(spawnedObject, transform.position, spawnedObject.transform.rotation);
			if(shareOrientation)
			{
				go.transform.forward = transform.forward;
			}

			if(spawnParticle != null)
			{
				spawnParticle.Simulate(0.0f, true, true);
				spawnParticle.Play();
			}
		}
	}
}
