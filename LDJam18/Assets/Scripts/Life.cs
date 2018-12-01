using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
	public int maxLife;
	public int currentLife;

	private void Awake ()
	{
		currentLife = maxLife;
	}

	public void UpdateLife(int amount)
	{
		currentLife += amount;
		CheckLife();
		LifeBarManager.instance.SetNewLifeValue(currentLife);
	}

	void CheckLife()
	{
		if(currentLife <= 0)
		{
			//RIP
		}
	}

	private void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			UpdateLife(-10);
		}
	}
}
