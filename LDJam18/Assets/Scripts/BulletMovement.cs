using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
	[SerializeField] float bulletSpeed;

	Rigidbody rigid;
	// Use this for initialization
	void Start ()
	{
		rigid = GetComponent<Rigidbody>();
		rigid.AddForce(transform.forward * bulletSpeed);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//transform.Translate(Vector3.forward * bulletSpeed * Time.fixedDeltaTime, Space.Self);
	}
}
