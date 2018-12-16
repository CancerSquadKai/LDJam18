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

/// <summary>
/// Basic enemy state
/// </summary>
public enum BeetleState
{
    FOLOWING, // Folows the target until entering attack range
    ANTICIPATING_ATTACK,
    ACTIVATING_ATTACK,
    DYING
}

public class BasicEnemyController : EnemyController<BeetleState>
{
    public BasicEnemyConfig config;
    public Vector3          folow_origin;
    public float rng;
    
	[SerializeField] GameObject deathParticle;

    public Animator      animator;
    
    protected override void Start()
    {
        base.Start();
        if(life)
            life.currentLife = life.maxLife = config.health;
    }

    protected override BeetleState StateUpdate(BeetleState state, float dt)
    {
        switch (state)
        {
            case BeetleState.FOLOWING:
                {
                    if (life && life.currentLife <= 0)
                    {
                        state = BeetleState.ANTICIPATING_ATTACK;
                    } else
                    {
                        // Check if can attack
                        float dist_to_target =
                        Vector3.Distance(
                            transform.position,
                            target.transform.position
                        );

                        bool reached_agro_range = dist_to_target <= config.attack_agro_range;
                        if (reached_agro_range)
                        {
                            state = BeetleState.ANTICIPATING_ATTACK;
                        }
                    }
                }
                break;
            case BeetleState.ANTICIPATING_ATTACK:
                {
                    if (state_lifetime >= config.attack_windup_duration)
                    {
                        state = BeetleState.ACTIVATING_ATTACK;
                    }
                }
                break;
            case BeetleState.ACTIVATING_ATTACK:
                {
                    state = BeetleState.DYING;
                }
                break;
            case BeetleState.DYING:
                {
                    if(state_lifetime > 2f)
                    {
                        Destroy(gameObject);
                    }
                }
                break;
        }
        return state;
    }
    
    protected override Movement MovementUpdate(Movement movement, Vector3 target, float dt)
    {
        switch (state)
        {
            case BeetleState.FOLOWING:
                {
                    // Move
                    Vector3 position_new = MoveTowardTarget(movement.position, target, config.speed_movement * dt, rng);
                    if (!float.IsNaN(position_new.x) && bump.velocity.magnitude <= 0.0625f)
                    {
                        position_new = Vector3.MoveTowards(movement.position, position_new, config.speed_movement * dt);
                    }

                    movement.position = position_new;
                }
                break;
            case BeetleState.DYING:
                {
                    movement.position_offset +=  -Vector3.up * state_lifetime * state_lifetime;
                }
                break;
        }
        return movement;
    }

    protected override void OnStateEnter(BeetleState state)
    {
        switch (state)
        {
            case BeetleState.FOLOWING:
                {
                    folow_origin= transform.position;
                    rng = Random.Range(-1.0f, 1.0f);
                }
                break;
            case BeetleState.ANTICIPATING_ATTACK:
                {
                    // Play windup anim
                    var windup_view = Instantiate(config.prefab_windup_circular_view, transform);
                    windup_view.transform.position = transform.position;
                    windup_view.Init(config.attack_windup_duration, config.attack_range);

                    if (animator)
                        animator.SetBool("Vibrate", true);
                    if (life && life.currentLife <= 0)
                    {
                        if (animator)
                            animator.SetTrigger("UseExplode");
                    }
                }
                break;
            case BeetleState.ACTIVATING_ATTACK:
                {
                    if (animator)
                        animator.SetBool("Vibrate", false);

                    GameObject go = Instantiate(deathParticle, transform.position, deathParticle.transform.rotation);
                    Destroy(go, 5f);

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
            case BeetleState.DYING:
                {
                    // Play recovery anim
                    if (animator)
                        animator.SetTrigger("UseExplode");
                }
                break;
        }
    }

    public Vector3 MoveTowardTarget(Vector3 current, Vector3 target, float speed, float rng)
    {
        Vector3 folow_origin     = this.folow_origin;

        Vector3 direction = (target - folow_origin).normalized;

        float dot = Vector3.Dot(
                current - folow_origin,
                direction
            );

        float diameter = Vector3.Distance(target, folow_origin);
        if(dot < 0 || dot > diameter)
        {
            return Vector3.MoveTowards(current, target, speed);
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
}
