using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorOnTrigger : MonoBehaviour
{
	[FMODUnity.EventRef]
	public string DoorOpenEvent = "event:/DoorOpen";
	FMOD.Studio.EventInstance DoorSnd;

	[Space]

	[SerializeField] Animator doorAnimator;
	[SerializeField] string tagName = "Player";
	[SerializeField] bool openDoor = false;
	//[SerializeField] Animator doorAnim; LATER
	bool closed = false;

	private void OnTriggerEnter (Collider other)
	{
		if(!closed)
		{
			if(other.tag == tagName)
			{
				if(openDoor)
				{
					DoorSnd = FMODUnity.RuntimeManager.CreateInstance(DoorOpenEvent);
					DoorSnd.start();
				}

				doorAnimator.SetBool("IsOpen", openDoor);
				closed = true;
				Shaker.instance.Shake(1f, 0.3f);
			}
		}
	}
}
