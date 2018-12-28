using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
	[FMODUnity.EventRef]
	public string DoorOpenEvent = "event:/DoorOpen";
	FMOD.Studio.EventInstance DoorSnd;

	[Space]

	[SerializeField] TrapToggle traps;
	[SerializeField] Animator doorAnimator;
	[SerializeField] string boolParameterName = "IsOpen";
	[SerializeField] float shakeDurationOnOpen = 2f;
	[SerializeField] float shakeIntensity = 0.2f;
	public List<EnemyKillDetection> enemies = new List<EnemyKillDetection>();
	int enemyAmount = 0;
	// Use this for initialization
	void Start ()
	{
		if(enemies.Count == 0)
		{
			enemies = GetComponentsInChildren<EnemyKillDetection>().ToList();
		}
		enemyAmount = enemies.Count;

		foreach(EnemyKillDetection enemy in enemies)
		{
			enemy.roomController = this;
		}
	}

	public void AddEnemy(EnemyKillDetection enemy)
	{
		enemies.Add(enemy);
		enemyAmount++;
	}
	
	public void RemoveEnemy(EnemyKillDetection enemy)
	{
		if(enemies.Contains(enemy))
		{
			enemies.Remove(enemy);
			enemyAmount--;

			if(enemyAmount == 0)
			{
				if(traps != null)
				{
					traps.ToggleTraps();
				}
				Shaker.instance.Shake(2f, 0.35f);
				//door.SetActive(false);
				doorAnimator.SetBool(boolParameterName, true);
				DoorSnd = FMODUnity.RuntimeManager.CreateInstance(DoorOpenEvent);
				DoorSnd.start();
			}
		}
		else
		{
			Debug.LogWarning("This enemy doesn't belong to the right RoomController !");
		}
	}
}
