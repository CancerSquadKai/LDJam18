using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrap : MonoBehaviour
{
	[System.Serializable]
	public class Burst
	{
		public GameObject spawnedObject;

		[Space]

		public Spawner[] spawners = new Spawner[1];

		[Space]

		public int shotAmount = 1;
		public float shotCD = 0.3f;
	}

	public Burst[] bursts;
    public bool canShoot = true;

    [SerializeField] float timeBetweenBursts = 1f;

	float timer = 0.0f;
	int index = 0;
	bool isFiring = false;
	
	// Use this for initialization
	void Start ()
	{
		
	}

	public IEnumerator BurstTrigger(int ind)
	{
		isFiring = true;
		int spawnerLength = bursts[ind].spawners.Length;

		for(int j = 0; j < bursts[ind].shotAmount; j++)
		{
			for (int i = 0; i < spawnerLength; i++)
			{
				Spawner spawner = bursts[ind].spawners[i];
				if(spawner != null)
				{
					spawner.SpawnObject(bursts[ind].spawnedObject);
				}		
			}
            if (!canShoot) break;
			yield return new WaitForSeconds(bursts[ind].shotCD);
		}
		isFiring = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!isFiring && canShoot)
		{
			/*if(Input.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine(BurstTrigger(index));
				if (index < bursts.Length - 1 && bursts.Length > 1)
				{
					index++;
				}
				else
				{
					index = 0;
				}
			}*/
			
			timer += Time.deltaTime;
			if(timer >= timeBetweenBursts)
			{
				timer = 0f;
				StartCoroutine(BurstTrigger(index));
				if(index < bursts.Length -1 && bursts.Length > 1)
				{
					index++;
				}
				else
				{
					index = 0;
				}
			}

			
		}
	}

}
