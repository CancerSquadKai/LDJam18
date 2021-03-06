﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] bool shareOrientation;
	[SerializeField] ParticleSystem spawnParticle;

	public void SpawnObject(GameObject spawnedObject, float timeBeforeDestroy, float speedFactor)
	{
		if(spawnedObject != null)
		{
			GameObject go = Instantiate(spawnedObject, transform.position, spawnedObject.transform.rotation);
			if(shareOrientation)
			{
				go.transform.forward = transform.forward;
			}

			BulletMovement bMvt = go.GetComponent<BulletMovement>();
			if(bMvt != null)
			{
				bMvt.bulletSpdMult = speedFactor;
			}

			if(timeBeforeDestroy != 0)
			{
				Destroy(go, timeBeforeDestroy);
			}

			if(spawnParticle != null)
			{
				spawnParticle.Simulate(0.0f, true, true);
				spawnParticle.Play();
			}
		}
	}
}
