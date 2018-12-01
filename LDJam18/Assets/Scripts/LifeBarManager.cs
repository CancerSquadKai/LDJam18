using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBarManager : MonoBehaviour
{
	[SerializeField] Life playerLife;
	[SerializeField] Slider greenLifeSlider;
	[SerializeField] Slider redLifeSlider;
	[SerializeField] float timeBeforeLerp = 0.5f;
	[SerializeField] float lerpSpeed = 2f;

	float timer;
	float playerLifeValue;
	float currentLifeValue;

	public static LifeBarManager instance;

	// Use this for initialization
	void Start ()
	{
		instance = this;
		if(playerLife != null)
		{
			playerLifeValue = playerLife.currentLife;
			currentLifeValue = playerLifeValue;
			greenLifeSlider.value = playerLifeValue;
			redLifeSlider.value = playerLifeValue;
		}
	}

	public void SetNewLifeValue(float newValue)
	{
		playerLifeValue = newValue;
		greenLifeSlider.value = playerLifeValue;
		timer = 0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(timer >= timeBeforeLerp)
		{
			if(currentLifeValue != playerLifeValue)
			{
				currentLifeValue = Mathf.MoveTowards(currentLifeValue, playerLifeValue, lerpSpeed * Time.deltaTime);
				redLifeSlider.value = currentLifeValue;
			}
		}
		else
		{
			timer += Time.deltaTime;
		}
	}
}
