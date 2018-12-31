using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapToggle : MonoBehaviour
{
	[SerializeField] BulletTrap[] traps;

	bool toggled = false;

	private void Awake ()
	{
		if(traps.Length == 0)
		{
			traps = GetComponentsInChildren<BulletTrap>();
		}
	}

	public void ToggleTraps()
	{
		//Debug.Log("Toggled : " + toggled);
		toggled = !toggled;
		foreach (BulletTrap trap in traps)
		{
			trap.enabled = toggled;
		}
		
	}
}
