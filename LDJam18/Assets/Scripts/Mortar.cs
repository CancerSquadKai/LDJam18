using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
	[SerializeField] WindupCircularView aoEView;
	[SerializeField] float windupDuration;
	[SerializeField] float attackRange;

	//[SerializeField] Animator anim;

	float timer = 0f;

	Vector3 initialPos;
	Vector3 targetPos;

	AvatarController player;
	// Use this for initialization
	void Start ()
	{
		initialPos = transform.position;
		player = FindObjectOfType<AvatarController>();
		targetPos = player.transform.position;

		var windup_view = Instantiate(aoEView);
		windup_view.transform.position = targetPos;
		windup_view.Init(windupDuration, attackRange);

	}
	
	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		transform.position = Vector3.Lerp(initialPos, targetPos, timer / windupDuration);
	}
}
