using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SniperState
{
    FOLOWING,
    ANTICIPATING_ATTACK,
    ACTIVATING_ATTACK,
    RECOVERING_ATTACK,
    DYING
}

public class SniperController : EnemyController<SniperState>
{
    protected override void Start()
    {
        base.Start();
        if (life)
            life.currentLife = life.maxLife = 100;
        SetState(SniperState.FOLOWING);
    }

    protected override SniperState StateUpdate(SniperState state, float dt)
    {
        switch (state)
        {
            case SniperState.FOLOWING:
                {
                    // Check if can attack
                    float dist_to_target =
                        Vector3.Distance(
                            transform.position,
                            target.transform.position
                        );

                    bool reached_agro_range = dist_to_target <= 2f;
                    if (reached_agro_range)
                    {
                        state = SniperState.ANTICIPATING_ATTACK;
                    }
                }
                break;
            case SniperState.ANTICIPATING_ATTACK:
                {
                    if (state_lifetime >= 1f)
                    {
                        state = SniperState.ACTIVATING_ATTACK;
                    }
                }
                break;
            case SniperState.ACTIVATING_ATTACK:
                {
                    state = SniperState.FOLOWING;
                }
                break;
            case SniperState.DYING:
                {
                    if (state_lifetime > 2f)
                    {
                        Destroy(gameObject);
                    }
                }
                break;
        }
        if (life && life.currentLife <= 0)
        {
            state = SniperState.DYING;
        }
        return state;
    }

    protected override Movement MovementUpdate(Movement movement, Vector3 target, float dt)
    {
        switch (state)
        {
            case SniperState.FOLOWING:
                {
                    movement = base.MovementUpdate(movement, target, dt);
                }
                break;
            case SniperState.DYING:
                {
                    movement.position_offset +=  -Vector3.up * state_lifetime * state_lifetime;
                }
                break;
        }
        return movement;
    }

}
