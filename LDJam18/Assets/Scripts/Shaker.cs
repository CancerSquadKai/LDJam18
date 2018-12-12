using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
	[Header("Shaking parameters")]
	public AnimationCurve intensityCurveX = new AnimationCurve(
		new Keyframe(0f, 0f),
		new Keyframe(0.2f, 1f),
		new Keyframe(1.0f, 0f));
	public AnimationCurve intensityCurveY = new AnimationCurve(
		new Keyframe(0f, 0f),
		new Keyframe(0.2f, 1f),
		new Keyframe(1.0f, 0f));
	public AnimationCurve intensityCurveZ = new AnimationCurve(
		new Keyframe(0f, 0f),
		new Keyframe(0.2f, 1f),
		new Keyframe(1.0f, 0f));

	public static Shaker instance;

	public bool shakeOnXAxis = true;
	public bool shakeOnYAxis = true;
	public bool shakeOnZAxis = false;

	Transform targetToShake;

	float intensityCurveIndex = 0.0f;
	bool isShaking = false;
	Vector3 initialPos;
	// Use this for initialization
	void Start ()
	{
		instance = this;
		targetToShake = GetComponent<Transform>();
		initialPos = targetToShake.localPosition;
	}

	float pendingShakeDuration = 0f;

	public void Shake (float duration, float intensity)
	{
		if (duration > 0 && !isShaking)
		{
			//Debug.Log("Shaking !");
			pendingShakeDuration += duration;
			StartCoroutine(DoShake(intensity));
		}
	}



	private void Update ()
	{
		/*if (pendingShakeDuration > 0 && !isShaking)
		{
			StartCoroutine(DoShake());
		}*/
	}

	IEnumerator DoShake (float intensity)
	{
		isShaking = true;

		intensityCurveIndex = 0f;

		float startTime = Time.realtimeSinceStartup;

		while (Time.realtimeSinceStartup < startTime + pendingShakeDuration)
		{
			intensityCurveIndex += Time.deltaTime;
			//Vector3 randomPoint = Random.onUnitSphere * intensity * intensityCurve.Evaluate(intensityCurveIndex / pendingShakeDuration);
			Vector3 randomPoint = Vector3.zero;
			if (shakeOnXAxis)
			{
				randomPoint.x = Random.Range(-1f, 1f) * intensity * intensityCurveX.Evaluate(intensityCurveIndex / pendingShakeDuration);
			}
			if (shakeOnYAxis)
			{
				randomPoint.y = Random.Range(-1f, 1f) * intensity * intensityCurveY.Evaluate(intensityCurveIndex / pendingShakeDuration);
			}
			if (shakeOnZAxis)
			{
				randomPoint.z = Random.Range(-1f, 1f) * intensity * intensityCurveZ.Evaluate(intensityCurveIndex / pendingShakeDuration);
			}
			targetToShake.localPosition = randomPoint;
			yield return null;
		}

		pendingShakeDuration = 0f;
		targetToShake.localPosition = initialPos;

		isShaking = false;
	}
}
