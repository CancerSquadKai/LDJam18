using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
	[SerializeField] float bulletSpeed;
	public float bulletSpdMult = 1f;

	// Update is called once per frame
	void FixedUpdate ()
	{
		transform.Translate(Vector3.forward * bulletSpeed * bulletSpdMult * Time.fixedDeltaTime, Space.Self);
	}
}
