using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public enum UpdateType
    {
        UPDATE,
        LATE_UPDATE,
        FIXED_UPDATE
    }

	[Header("Follow Position Parameters")]
	[Tooltip("The target transform this object should follow")]
	public Transform target;
	public bool followOnXAxis = true;
	public bool followOnYAxis = true;
	public bool followOnZAxis = true;
	[Tooltip("The time it takes to this object to reach the followed object")]
	[Range(0f,1f)]
	public float smoothTime = 0.3f;
    public UpdateType updateType = UpdateType.LATE_UPDATE;

    [Space]

	[Header("Follow Orientation Parameters")]
	public bool shareOrientation;
	public bool rotateOnXAxis = true;
	public bool rotateOnYAxis = true;
	public bool rotateOnZAxis = true;
	public float rotateSpeed = 10f;

	Vector3 velocity = Vector3.zero;

	Vector3 GetTargetPos()
	{
		Vector3 targetPos = transform.position;
		if(followOnXAxis)
		{
			targetPos.x = target.position.x;
		}
		if (followOnYAxis)
		{
			targetPos.y = target.position.y;
		}
		if (followOnZAxis)
		{
			targetPos.z = target.position.z;
		}

		return targetPos;
	}

	Vector3 GetTargetRot ()
	{
		Vector3 targetRot = transform.eulerAngles;
		if (rotateOnXAxis)
		{
			targetRot.x = target.forward.x;
		}
		if (rotateOnYAxis)
		{
			targetRot.y = target.forward.y;
		}
		if (rotateOnZAxis)
		{
			targetRot.z = target.forward.z;
		}

		return targetRot;
	}

	void MoveTowards(float dt)
	{
		if(target)
		{
			transform.position = Vector3.SmoothDamp(transform.position, GetTargetPos(), ref velocity, smoothTime);
			if (shareOrientation)
			{
				Vector3 newDir = Vector3.RotateTowards(transform.forward, GetTargetRot(), rotateSpeed * dt, 0f);
				transform.rotation = Quaternion.LookRotation(newDir);
			}
		}
		else
		{
			Debug.Log("No target to follow !!");
		}
    }

    void Update()
    {
        if (updateType == UpdateType.UPDATE)
            MoveTowards(Time.deltaTime);
    }

    void LateUpdate()
    {
        if (updateType == UpdateType.LATE_UPDATE)
            MoveTowards(Time.deltaTime);
    }

    void FixedUpdate()
	{
        if(updateType == UpdateType.FIXED_UPDATE)
		    MoveTowards(Time.fixedDeltaTime);
	}
}
