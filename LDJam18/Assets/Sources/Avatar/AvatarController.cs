using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour, IBumpable
{

    public AvatarView   view;
    public AvatarConfig config;
    public AvatarModel  model;

    public Transform oriented_direction;
    public Transform oriented_shoot;

    private new Rigidbody rigidbody;

    private bool _lt = false;
    private float _lt_step = 0.5f;

    private bool _rt = false;
    private float _rt_step = 0.5f;

    public BulletTrap shoot;

    private List<AttackArc> attack_slash_group = new List<AttackArc>(32);

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        var life= GetComponent<Life>();
        if (life)
        {
            life.onUpdateLife += OnHealthUpdate;
        }
    }

    private void Start()
    {
        model = new AvatarModel(config, view);
    }

    private void Update ()
    {
        float dt         = Time.deltaTime;

        // inputs
        {
            {
                Vector2 left_stick_input = Vector2.zero;
                left_stick_input.x       = Input.GetAxisRaw("Horizontal");
                left_stick_input.y       = Input.GetAxisRaw("Vertical");
                model.input_movement     = left_stick_input;
            }

            {
                Vector2 right_stick_input = Vector2.zero;
                right_stick_input.x       = Input.GetAxisRaw("RSX");
                right_stick_input.y       = Input.GetAxisRaw("RSY");
                model.input_shoot         = right_stick_input.normalized;
                shoot.canShoot = model.input_shoot.magnitude > 0.125f;
                view.UpdateAiming(shoot.canShoot);
            }

            // model
            model.UpdateInput();

            // Left trigger
            {
                bool lt_new  = Input.GetAxisRaw("LT") > _lt_step;
                bool lt_down = !_lt && lt_new;
                bool lt_up   = _lt  && !lt_new;
                if (lt_down)
                { // dash
                    model.Dash();
                }
                _lt = lt_new;
            }

            // Right trigger
            {
                bool rt_new  = Input.GetAxisRaw("RT") > _rt_step;
                bool rt_down = !_rt &&  rt_new;
                bool rt_up   =  _rt && !rt_new;
                if (rt_down)
                { // slash
                    var attack_slash_new = new AttackArc(Mathf.Atan2(model.direction.y, model.direction.x));
                    SetAttackPhase(ref attack_slash_new, Attack.Phase.WINDUP);
                    attack_slash_group.Add(attack_slash_new);
                }
                _rt = rt_new;
            }

            // orientation
            oriented_direction.rotation = Quaternion.LookRotation(
                new Vector3(
                    model.direction.x,
                    0,
                    model.direction.y
                ),
                Vector3.up
            );

            if(model.input_shoot.magnitude > 0.85f)
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
        for (int attack_slash_index = attack_slash_count - 1; attack_slash_index > 0; --attack_slash_index)
        {
            attack_slash = attack_slash_group[attack_slash_index];
            // Advance attack phases
            attack_slash.attack.phase_lifetime += dt;
            switch (attack_slash.attack.phase)
            {
                case Attack.Phase.WINDUP:
                    {
                        if (attack_slash.attack.phase_lifetime >= config.attack_windup_duration)
                        {
                            SetAttackPhase(ref attack_slash, Attack.Phase.ACTIVE);
                        }
                    }
                    break;
                case Attack.Phase.ACTIVE:
                    {
                        SetAttackPhase(ref attack_slash, Attack.Phase.RECOVERY);
                    }
                    break;
                case Attack.Phase.RECOVERY:
                    {
                        if (attack_slash.attack.phase_lifetime >= config.attack_recovery_duration)
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

    public void SetAttackPhase(ref AttackArc attack, Attack.Phase phase)
    {
        attack.SetPhase(phase);
        switch (phase)
        {
            case Attack.Phase.WINDUP:
                {
                    // avatar anim
                    view.OnAttack();

                    // Play arcwindup anim
                    var windup_view = Instantiate(config.prefab_windup_arc_view, this.oriented_direction);
                    windup_view.transform.position = transform.position;
                    windup_view.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    windup_view.Init(
                        config.attack_windup_duration,
                        config.attack_range,
                        Vector3.forward,
                        config.attack_angle * Mathf.Deg2Rad
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
                        bool target_in_attack_range = dist_to_target <= config.attack_range;
                        if (target_in_attack_range)
                        {
                            Vector3 dir = (enemy.transform.position - transform.position).normalized;
                            float angle_to_target = Mathf.Atan2(dir.z, dir.x);
                            float avatar_angle = Mathf.Atan2(model.direction.y, model.direction.x);

                            target_in_attack_range &=
                                Mathf.Abs(Mathf.DeltaAngle(
                                    angle_to_target * Mathf.Rad2Deg,
                                    avatar_angle * Mathf.Rad2Deg
                                )) < (config.attack_angle * 0.5f);


                            if (target_in_attack_range)
                            {
                                enemy.Bump(
                                    (enemy.transform.position - transform.position).normalized,
                                    config.attack_bump_distance,
                                    config.attack_bump_duration
                                );

								Shaker.instance.Shake(config.attack_bump_duration, config.attack_bump_distance / 40f);
							}
                        }
                    }
                }
                break;
        }
    }

    private void FixedUpdate()
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

    public void Bump(Vector3 direction, float distance, float duration)
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

    private void OnHealthUpdate(int health, int delta)
    {
        view.StartCoroutine(view.GotHitCoroutine());
    }
}
