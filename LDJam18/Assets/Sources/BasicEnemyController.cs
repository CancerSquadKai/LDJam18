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

    public Transform        target;
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
                        target.position,
                        config.speed_movement * dt
                    );
                    transform.position  = position;

                    // Check if can attack
                    float dist_to_target =
                        Vector3.Distance(
                            position,
                            target.position
                        );

                    bool reached_attack_range = dist_to_target <= config.attack_range;
                    if (reached_attack_range)
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
            attack = new Attack();
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
                }
                break;
            case Attack.Phase.ACTIVE:
                {
                    // Todo : hit people in range
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
