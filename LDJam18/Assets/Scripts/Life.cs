using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
	public int maxLife;
	public int currentLife;
	public bool isPlayer = false;

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
	}
}
