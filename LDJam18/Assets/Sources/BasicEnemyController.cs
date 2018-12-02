using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        WINDUP = 0,
        ACTIVE = 1,
        RECOVERY = 2,
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

public struct AttackArc
{
    public Attack attack;
    public float angle;
    public AttackArc(float angle)
    {
        this.attack = new Attack();
        this.angle = angle;
    }

    public void SetPhase(Attack.Phase phase)
    {
        this.attack.SetPhase(phase);
    }
}

public class BasicEnemyController : MonoBehaviour, IBumpable
{

    /// <summary>
    /// Basic enemy state
    /// </summary>
    public enum State
    {
        FOLOWING, // Folows the target until entering attack range
        ATTACKING // Windup, attack target if in attack range, recover then back to folowing.
    }

    public BasicEnemyConfig config;

    public AvatarController target;
    public State            state;
    public Attack           attack;
    public Vector3          folow_origin;
    public float rng;

    public float acceleration_bump;
    public Vector3 velocity_bump;

    private new Rigidbody rigidbody;

    public Life life;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        life = GetComponent<Life>();
    }
    
    public void Start()
    {
        if(life)
            life.currentLife = life.maxLife = config.health;
        if (!target)
            target = FindObjectOfType<AvatarController>();
        StateReset();
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
                    Vector3 position_new = MoveTowardTarget(config.speed_movement * dt, rng);
                    if (!float.IsNaN(position_new.x) && velocity_bump.magnitude <= 0.0625f)
                    {
                        position_new = Vector3.MoveTowards(position, position_new, config.speed_movement * dt);
                        position = position_new;
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

        velocity_bump = Vector3.MoveTowards(
            velocity_bump,
            Vector3.zero,
            acceleration_bump * dt
        );

        //position += velocity_bump * dt;
        rigidbody.velocity = velocity_bump;
        if (rigidbody != null)
            rigidbody.position = position;

        if(life && life.currentLife <= 0 && state != State.ATTACKING)
            SetState(State.ATTACKING);
    }

    public void StateReset()
    {
        if (life && life.currentLife > 0)
            SetState(State.FOLOWING);
        else
            Destroy(gameObject);
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
                    var windup_view = Instantiate(config.prefab_windup_circular_view, transform);
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
                        var life = target.GetComponent<Life>();
                        if (life)
                            life.UpdateLife(-config.attack_damage);
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
                    StateReset();
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
        if(dot < 0 || dot > diameter)
        {
            return Vector3.MoveTowards(current_position, target_position, speed);
        }

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

    public void Bump(Vector3 direction, float distance, float duration)
    {
        acceleration_bump = (2 * distance) / (duration * duration);
        Vector3 impulse_bump = direction * ((2 * distance) / duration);
        velocity_bump += impulse_bump;
    }
}
