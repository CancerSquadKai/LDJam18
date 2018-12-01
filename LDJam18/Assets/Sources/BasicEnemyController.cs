using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour {

    /// <summary>
    /// Basic enemy state
    /// </summary>
    public enum State
    {
        FOLOWING, // Folows the target until entering attack range
        ATTACKING // Windup, attack target if in attack range, recover then back to folowing.
    }

    /// <summary>
    /// Basic enemy attack
    /// </summary>
    public struct Attack
    {
        /// <summary>
        /// Attack phase
        /// </summary>
        public enum Phase
        {
            WINDUP    = 0,
            ACTIVE    = 1,
            RECOVERY  = 2,
            COMPLETED = 3
        }

        public Phase phase;
        public float phase_lifetime;

        public void SetPhase(Phase phase)
        {
            this.phase          = phase;
            this.phase_lifetime = 0.0f;
        }
    }

    public BasicEnemyConfig config;

    public AvatarController target;
    public State            state;
    public Attack           attack;
    public Vector3          folow_origin;
    public float rng;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Start()
    {
        if (target)
            target = FindObjectOfType<AvatarController>();
        SetState(State.FOLOWING);
    }

    public void Update()
    {
        float dt         = Time.deltaTime;
        Vector3 position = transform.position;
        switch (state)
        {
            case State.FOLOWING:
                {
                    // Move
                    if(dt > 0)
                    {
                        position = MoveTowardTarget(config.speed_movement * dt, rng);
						if(rigidbody != null)
						{
							rigidbody.position = position;
						}
                    }

                    // Check if can attack
                    float dist_to_target =
                        Vector3.Distance(
                            position,
                            target.transform.position
                        );

                    bool reached_agro_range = dist_to_target <= config.attack_agro_range;
                    if (reached_agro_range)
                    {
                        SetState(State.ATTACKING);
                    }
                }
                break;
            case State.ATTACKING:
                {
                    // Advance attack phases
                    attack.phase_lifetime += dt;
                    switch (attack.phase)
                    {
                        case Attack.Phase.WINDUP:
                            {
                                if(attack.phase_lifetime >= config.attack_windup_duration)
                                {
                                    SetAttackPhase(Attack.Phase.ACTIVE);
                                }
                            }
                            break;
                        case Attack.Phase.ACTIVE:
                            {
                                SetAttackPhase(Attack.Phase.RECOVERY);
                            }
                            break;
                        case Attack.Phase.RECOVERY:
                            {
                                if (attack.phase_lifetime >= config.attack_recovery_duration)
                                {
                                    SetAttackPhase(Attack.Phase.COMPLETED);
                                }
                            }
                            break;
                    }
                }
                break;
        }
    }

    public void SetState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.FOLOWING:
                {
                    folow_origin= transform.position;
                    rng = Random.Range(-1.0f, 1.0f);
                }
                break;
        }
        if (state == State.ATTACKING)
        {
            rigidbody.velocity = Vector3.zero;
            SetAttackPhase(Attack.Phase.WINDUP);
        }
    }

    public void SetAttackPhase(Attack.Phase phase)
    {
        attack.SetPhase(phase);
        switch (phase)
        {
            case Attack.Phase.WINDUP:
                {
                    // Play windup anim
                    var windup_view = Instantiate(config.prefab_windup_circular_view);
                    windup_view.transform.position = transform.position;
                    windup_view.Init(config.attack_windup_duration, config.attack_range);
                }
                break;
            case Attack.Phase.ACTIVE:
                {
                    // Todo : hit people in range
                    float dist_to_target =
                        Vector3.Distance(
                            transform.position,
                            target.transform.position
                        );
                    bool target_in_attack_range = dist_to_target <= config.attack_range;
                    if (target_in_attack_range)
                    {
                        target.Bump(
                            (target.transform.position - transform.position).normalized,
                            config.attack_bump_distance,
                            config.attack_bump_duration
                        );
                    }
                }
                break;
            case Attack.Phase.RECOVERY:
                {
                    // Play recovery anim
                }
                break;
            case Attack.Phase.COMPLETED:
                {
                    SetState(State.FOLOWING);
                }
                break;
        }
    }

    public Vector3 MoveTowardTarget(float speed, float rng)
    {
        Vector3 target_position  = target.transform.position;
        Vector3 current_position = transform.position;
        Vector3 folow_origin     = this.folow_origin;

        Vector3 direction = (target_position - folow_origin).normalized;

        float dot = Vector3.Dot(
                current_position - folow_origin,
                direction
            );

        float diameter = Vector3.Distance(target_position, folow_origin);
        float radius = diameter * 0.5f;
        float length = Mathf.MoveTowards(dot, diameter, speed);
        length /= diameter;
        float p = Mathf.Sqrt(1-Mathf.Pow((1-(length)*2), 2));
        Vector3 direction_clockwise_perpendicular = Quaternion.Euler(0,90,0) * direction;
        Vector3 new_pos =
                    folow_origin + direction * length * diameter +
                    direction_clockwise_perpendicular * p * radius * rng;
        return new_pos;
    }
}
