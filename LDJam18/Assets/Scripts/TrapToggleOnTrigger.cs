using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapToggleOnTrigger : MonoBehaviour
{
	[SerializeField] TrapToggle traps;
	[SerializeField] string tagName = "Player";
	//[SerializeField] Animator doorAnim; LATER
	bool triggered = false;

	private void OnTriggerEnter (Collider other)
	{
		if (!triggered)
		{
			traps.ToggleTraps();
			triggered = true;
		}
	}
}