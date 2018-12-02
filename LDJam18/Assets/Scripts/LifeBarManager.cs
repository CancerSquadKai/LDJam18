using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class LifeBarManager : MonoBehaviour
{
	[SerializeField] Life playerLife;
	[SerializeField] Slider greenLifeSlider;
	[SerializeField] Slider redLifeSlider;
	[SerializeField] float timeBeforeLerp = 0.5f;
	[SerializeField] float lerpSpeed = 2f;

	[SerializeField] PostProcessingProfile postProcess;
	[SerializeField] Color initialColor;
	[SerializeField] Color hurtColor;

	[SerializeField] float maxChromaticIntensity = 1f;
	float timer;
	float playerLifeValue;
	float currentLifeValue;
	float maxLife;
	public static LifeBarManager instance;
	VignetteModel.Settings vign;
	ChromaticAberrationModel.Settings chrom;
	// Use this for initialization
	void Start ()
	{
		instance = this;

		postProcess = Camera.main.GetComponent<PostProcessingBehaviour>().profile;

		if(playerLife != null)
		{
			playerLifeValue = playerLife.currentLife;
			currentLifeValue = playerLifeValue;
			greenLifeSlider.value = playerLifeValue;
			redLifeSlider.value = playerLifeValue;

			if(postProcess != null)
			{
				maxLife = playerLife.maxLife;
				vign = postProcess.vignette.settings;
				vign.color = initialColor;

				chrom = postProcess.chromaticAberration.settings;
				chrom.intensity = 0f;

				postProcess.chromaticAberration.settings = chrom;
				postProcess.vignette.settings = vign;
			}
		}

	}

	public void SetNewLifeValue(float newValue)
	{
		playerLifeValue = newValue;
		greenLifeSlider.value = playerLifeValue;
		timer = 0f;
	}

	void CheckEffects()
	{
		if (postProcess != null)
		{
			vign.color = Color.Lerp(initialColor, hurtColor, 1 - (currentLifeValue / maxLife));
			chrom.intensity = Mathf.Lerp(0f, maxChromaticIntensity, 1 - (currentLifeValue / maxLife));

			postProcess.chromaticAberration.settings = chrom;
			postProcess.vignette.settings = vign;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckEffects();
		if (timer >= timeBeforeLerp)
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
