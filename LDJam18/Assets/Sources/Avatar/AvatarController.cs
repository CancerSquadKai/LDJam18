using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour, IBumpable
{

	public AvatarView view;
	public AvatarConfig config;
	public AvatarModel model;
	public BulletTrap shot;

	public Transform oriented_direction;
	public Transform oriented_shoot;
	
	[Space]

	[FMODUnity.EventRef]
	public string AttackEvent;
	FMOD.Studio.EventInstance AttackSnd;

	[FMODUnity.EventRef]
	public string DashEvent;
	FMOD.Studio.EventInstance DashSnd;

	private new Rigidbody rigidbody;

	private bool _lt = false;
	private float _lt_step = 0.25f;

	private bool _rt = false;
	private float _rt_step = 0.25f;
	private float _rt_down_time = 0;

	float can_slash_time = 0;

	public BulletTrap shoot;

	private List<AttackArc> attack_slash_group = new List<AttackArc>(32);

	private void Awake ()
	{
		rigidbody = GetComponent<Rigidbody>();

		var life = GetComponent<Life>();
		if (life)
		{
			life.onUpdateLife += OnHealthUpdate;
		}
	}

	private void Start ()
	{
		model = new AvatarModel(config, view, shot);
	}

	private void Update ()
	{
		float dt = Time.deltaTime;

		// inputs
		{
			{
				Vector2 left_stick_input = Vector2.zero;
				left_stick_input.x = Input.GetAxisRaw("Horizontal");
				left_stick_input.y = Input.GetAxisRaw("Vertical");
				model.input_movement = left_stick_input;
			}

			{
				Vector2 right_stick_input = Vector2.zero;
				right_stick_input.x = Input.GetAxisRaw("RSX");
				right_stick_input.y = Input.GetAxisRaw("RSY");
				model.input_shoot = right_stick_input.normalized;
				shoot.canShoot = model.input_shoot.magnitude > 0.125f;
				view.UpdateAiming(shoot.canShoot);
			}

			// model
			model.UpdateInput();

			// Left trigger
			{
				bool lt_new = Input.GetAxisRaw("LT") > _lt_step;
				bool lt_down = !_lt && lt_new;
				bool lt_up = _lt && !lt_new;
				if (lt_down)
				{ // dash
					DashSnd = FMODUnity.RuntimeManager.CreateInstance(DashEvent);
					DashSnd.start();
					model.Dash();
				}
				_lt = lt_new;
			}

			// Right trigger
			{
				bool rt_new = Input.GetAxisRaw("RT") > _rt_step;
				bool rt_down = !_rt && rt_new;
				bool rt_up = _rt && !rt_new;
				if (rt_down)
				{
					_rt_down_time = Time.time;
				}
				if (Time.time > can_slash_time &&
					_rt_down_time + config.slash[model.slash_level].input_bufer_duration > can_slash_time)
				{
					// slash
					var attack_slash_new = new AttackArc(Mathf.Atan2(model.direction.y, model.direction.x));
					SetAttackPhase(ref attack_slash_new, Attack.Phase.WINDUP);
					attack_slash_group.Add(attack_slash_new);
				}
				_rt = rt_new;
			}

			// orientation
			if (
				model.input_shoot.magnitude < 0.85f ||
				attack_slash_group.Count > 0 ||
				model.dash.progress_position < 1.0f
			)
				oriented_direction.rotation =
					Quaternion.LookRotation(
						new Vector3(
							model.direction.x,
							0,
							model.direction.y
						),
						Vector3.up
					);
			else
				oriented_direction.rotation = Quaternion.RotateTowards(
					oriented_direction.rotation,
					Quaternion.LookRotation(
						new Vector3(
							model.input_shoot.x,
							0,
							model.input_shoot.y
						),
						Vector3.up
					),
					Time.deltaTime * 360f * 2f
				);

			if (model.input_shoot.magnitude > 0.85f)
				oriented_shoot.rotation = Quaternion.LookRotation(
					new Vector3(
						model.input_shoot.x,
						0,
						model.input_shoot.y
					),
					Vector3.up
				);
		}

		int attack_slash_count = attack_slash_group.Count;

		AttackArc attack_slash;
		for (int attack_slash_index = attack_slash_count - 1; attack_slash_index >= 0; --attack_slash_index)
		{
			attack_slash = attack_slash_group[attack_slash_index];
			// Advance attack phases
			attack_slash.attack.phase_lifetime += dt;
			switch (attack_slash.attack.phase)
			{
				case Attack.Phase.WINDUP:
					{
						if (attack_slash.attack.phase_lifetime >= config.slash[model.slash_level].anticipation_duration)
						{
							SetAttackPhase(ref attack_slash, Attack.Phase.ACTIVE);
						}
					}
					break;
				case Attack.Phase.ACTIVE:
					{
						AttackSnd = FMODUnity.RuntimeManager.CreateInstance(AttackEvent);
						AttackSnd.start();
						SetAttackPhase(ref attack_slash, Attack.Phase.RECOVERY);
					}
					break;
				case Attack.Phase.RECOVERY:
					{
						if (attack_slash.attack.phase_lifetime >= config.slash[model.slash_level].recovery_duration)
						{
							SetAttackPhase(ref attack_slash, Attack.Phase.COMPLETED);
						}
					}
					break;
				case Attack.Phase.COMPLETED:
					{
						attack_slash_group.RemoveAt(attack_slash_index);
					}
					continue;
			}
			attack_slash_group[attack_slash_index] = attack_slash;
		}
	}

	private void LateUpdate ()
	{
		model.UpdateView();
	}

	public void SetAttackPhase (ref AttackArc attack, Attack.Phase phase)
	{
		attack.SetPhase(phase);
		switch (phase)
		{
			case Attack.Phase.WINDUP:
				{
					can_slash_time = Time.time + config.slash[model.slash_level].recovery_duration;
					// avatar anim
					view.OnAttack();

					// Play arcwindup anim
					var windup_view = Instantiate(config.prefab_windup_arc_view, this.oriented_direction);
					windup_view.transform.position = transform.position;
					windup_view.transform.localRotation = Quaternion.Euler(90, 0, 0);
					windup_view.Init(
						config.slash[model.slash_level].anticipation_duration,
						config.slash[model.slash_level].range,
						Vector3.forward,
						config.slash[model.slash_level].angle * Mathf.Deg2Rad
						);
				}
				break;
			case Attack.Phase.ACTIVE:
				{

					var enemy_group = FindObjectsOfType<BasicEnemyController>();
					foreach (var enemy in enemy_group)
					{
						float dist_to_target =
							Vector3.Distance(
								transform.position,
								enemy.transform.position
							);
						bool target_in_attack_range = dist_to_target <= config.slash[model.slash_level].range * 1.25f;
						if (target_in_attack_range)
						{
							Vector3 dir = (enemy.transform.position - transform.position).normalized;
							float angle_to_target = Mathf.Atan2(dir.z, dir.x);
							float avatar_angle = Mathf.Atan2(model.direction.y, model.direction.x);

							target_in_attack_range = false;
							target_in_attack_range |=
								Mathf.Abs(Mathf.DeltaAngle(
									angle_to_target * Mathf.Rad2Deg,
									avatar_angle * Mathf.Rad2Deg
								)) < (config.slash[model.slash_level].angle * 0.5f);

							target_in_attack_range |=
								Mathf.Abs(Mathf.DeltaAngle(
									angle_to_target * Mathf.Rad2Deg,
									attack.angle * Mathf.Rad2Deg
								)) < (config.slash[model.slash_level].angle * 0.75f);

							if (target_in_attack_range)
							{
								enemy.Bump(
									(enemy.transform.position - transform.position).normalized,
									config.slash[model.slash_level].bump_distance,
									config.slash[model.slash_level].bump_duration
								);

								Shaker.instance.Shake(config.slash[model.slash_level].bump_duration, config.slash[model.slash_level].bump_distance / 40f);

								var life = enemy.life;
								if (life)
								{
									life.UpdateLife(-config.slash[model.slash_level].damage);
								}
							}
						}
					}
				}
				break;
		}
	}

	private void FixedUpdate ()
	{
		model.UpdatePhysics(Time.fixedDeltaTime);
		rigidbody.velocity = new Vector3(
			model.velocity.x,
			0,
			model.velocity.y
			);
		rigidbody.position = rigidbody.position + new Vector3(
			model.translation.x,
			0,
			model.translation.y
			);
	}

	public void Bump (Vector3 direction, float distance, float duration)
	{
		model.Bump(
			new Vector2(
				direction.x,
				direction.z
			),
			distance,
			duration
		);

		Shaker.instance.Shake(duration * 4f, distance / 10f);
	}

	private void OnHealthUpdate (int health, int delta)
	{
		view.StartCoroutine(view.GotHitCoroutine());
	}
}
