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

    public void Update()
    {
        float dt         = Time.deltaTime;
        Vector3 position = transform.position;
        switch (state)
        {
            case State.FOLOWING:
                {
                    // Move
                    position = Vector3.MoveTowards(
                        position,
                        target.transform.position,
                        config.speed_movement * dt
                    );
                    transform.position  = position;

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
}
