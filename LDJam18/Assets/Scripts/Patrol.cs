using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
	[SerializeField] Vector3[] patrolPoints;
	[SerializeField] bool areCoordinatesLocal = false;
	[SerializeField] float waitTimeBetweenPoints;
	[SerializeField] float moveSpeed;
	[SerializeField] bool loop = true;

	[System.Serializable]
	public class LookAtOptions
	{
		public bool lookAtTargetPos = false;
		public bool smoothLookAt = false;
		public bool lockXRotation = false;
		public bool lockYRotation = false;
		public bool lockZRotation = false;
	}

	public LookAtOptions lookAtOptions = new LookAtOptions();

	float timer;
	int index = 0;
	bool waiting = false;
	// Use this for initialization
	void Start ()
	{
		timer = waitTimeBetweenPoints;
	}

	void WaitTime()
	{
		timer -= Time.deltaTime;
		if(timer <= 0f)
		{
			waiting = false;
			timer = waitTimeBetweenPoints;
		}
	}

	void MoveTowardsNextPoint()
	{
		if(!waiting)
		{
			if (areCoordinatesLocal)
			{
				if (Vector3.Distance(transform.localPosition, patrolPoints[index]) > 0.1f)
				{
					if(lookAtOptions.lookAtTargetPos)
					{
						if(lookAtOptions.smoothLookAt)
						{
							var targetRotation = Quaternion.LookRotation(patrolPoints[index] - transform.position);

							if(lookAtOptions.lockXRotation)
							{
								targetRotation.x = 0f;
							}
							if (lookAtOptions.lockYRotation)
							{
								targetRotation.y = 0f;
							}
							if (lookAtOptions.lockZRotation)
							{
								targetRotation.z = 0f;
							}
							// Smoothly rotate towards the target point.
							transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
						}
						else
						{
							transform.LookAt(patrolPoints[index]);
						}
					}
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, patrolPoints[index], moveSpeed * Time.deltaTime);
				}
				else
				{
					if (loop)
					{
						if (index == patrolPoints.Length - 1)
						{
							index = 0;
						}
						else
						{
							index++;
						}
					}
					else
					{
						if (index < patrolPoints.Length - 1)
						{
							index++;
						}
					}
					waiting = true;
				}
			}
			else
			{
				if (Vector3.Distance(transform.position, patrolPoints[index]) > 0.1f)
				{
					if (lookAtOptions.lookAtTargetPos)
					{
						if (lookAtOptions.smoothLookAt)
						{
							var targetRotation = Quaternion.LookRotation(patrolPoints[index] - transform.position);

							if (lookAtOptions.lockXRotation)
							{
								targetRotation.x = 0f;
							}
							if (lookAtOptions.lockYRotation)
							{
								targetRotation.y = 0f;
							}
							if (lookAtOptions.lockZRotation)
							{
								targetRotation.z = 0f;
							}

							// Smoothly rotate towards the target point.
							transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
						}
						else
						{
							transform.LookAt(patrolPoints[index]);

						}
					}
					transform.position = Vector3.MoveTowards(transform.position, patrolPoints[index], moveSpeed * Time.deltaTime);
				}
				else
				{
					if (loop)
					{
						if (index == patrolPoints.Length - 1)
						{
							index = 0;
						}
						else
						{
							index++;
						}
					}
					else
					{
						if (index < patrolPoints.Length - 1)
						{
							index++;
						}
					}
					waiting = true;
				}
			}
		
		}
		else
		{
			WaitTime();
		}

	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		MoveTowardsNextPoint();
	}
}
