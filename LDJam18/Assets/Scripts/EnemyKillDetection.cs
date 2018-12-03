using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillDetection : MonoBehaviour
{
	public RoomController roomController;
	// Use this for initialization

	private void OnDestroy ()
	{
		roomController.RemoveEnemy(this);
	}
}
