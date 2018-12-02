using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	public Transform target;

	[System.Serializable]
	public class LookOptions
	{
		public bool lockXAxis = false;
		public bool lockYAxis = true;
		public bool lockZAxis = false;
	}

	public LookOptions look;
	public bool useTag;
	public string tagName;
	// Use this for initialization
	void Start ()
	{
		if (useTag && target == null)
		{
			target = GameObject.FindGameObjectWithTag(tagName).transform;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(target)
		{
			Vector3 lookAtPos = target.position;
			if (look.lockXAxis)
				lookAtPos.x = transform.position.x;
			if (look.lockYAxis)
				lookAtPos.y = transform.position.y;
			if (look.lockZAxis)
				lookAtPos.z = transform.position.z;
			transform.LookAt(lookAtPos);
		}
	}
}
