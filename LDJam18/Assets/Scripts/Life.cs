using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
	public int maxLife;
	public int currentLife;

	[SerializeField] float invFramesWhenHit = 0.2f;

	float timer = 0f;


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
				LifeBarManager.instance.SetNewLifeValue(currentLife);
				timer = 0f;
			}
		}
		else
		{
			currentLife += amount;
			CheckLife();
			LifeBarManager.instance.SetNewLifeValue(currentLife);
		}

	}

	void CheckLife()
	{
		if(currentLife <= 0)
		{
			//RIP
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

		/*if(Input.GetKeyDown(KeyCode.Space))
		{
			UpdateLife(-10);
		}*/
	}
}
