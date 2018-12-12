using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
	public int maxLife;
	public int currentLife;
	public bool isPlayer = false;
    public bool isInvulnerableToBullet = false;

	[SerializeField] GameObject[] toDisable;
	[SerializeField] GameObject particleOnHit;
	[SerializeField] GameObject particleOnPlayerDeath;
	[SerializeField] float invFramesWhenHit = 0.2f;

	float timer = 0f;

    public System.Action<int,int> onUpdateLife;


	private void Awake ()
	{
		currentLife = maxLife;
	}

	public void UpdateLife(int amount)
	{
		if(amount < 0)
		{
			if(timer >= invFramesWhenHit)
			{
				currentLife += amount;
				CheckLife();
				if(isPlayer)
				{
					LifeBarManager.instance.SetNewLifeValue(currentLife);
				}
				timer = 0f;
                if (onUpdateLife != null)
                    onUpdateLife(currentLife, amount);

				if(particleOnHit != null)
				{
					GameObject particle = Instantiate(particleOnHit, transform.position, particleOnHit.transform.rotation);
					Destroy(particle, 4f);
					particle.transform.parent = transform;
				}

			}
		}
		else
		{
			currentLife += amount;
			CheckLife();
			if (isPlayer)
			{
				LifeBarManager.instance.SetNewLifeValue(currentLife);
			}
		}

    }

	void CheckLife()
	{
		//Debug.Log("CurrentLife : " + currentLife);
		if(currentLife <= 0)
		{
			if(isPlayer)
			{
				GameObject go = Instantiate(particleOnPlayerDeath, transform.position, particleOnPlayerDeath.transform.rotation);
				Transform[] children = GetComponentsInChildren<Transform>();

				foreach(Transform g in children)
				{
					g.gameObject.SetActive(false);
				}
				SceneMgr sceneMgr = FindObjectOfType<SceneMgr>();
				sceneMgr.LoadScene();
			}
		}
		else if (currentLife > maxLife)
		{
			currentLife = maxLife;
		}
	}

	private void Update ()
	{
		if(timer < invFramesWhenHit)
		{
			timer += Time.deltaTime;
		}
	}
}
