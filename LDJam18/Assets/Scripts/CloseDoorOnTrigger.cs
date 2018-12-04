using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorOnTrigger : MonoBehaviour
{
	[SerializeField] GameObject door;
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
				door.SetActive(!openDoor);
				closed = true;
				Shaker.instance.Shake(1f, 0.3f);
			}
		}
	}
}
