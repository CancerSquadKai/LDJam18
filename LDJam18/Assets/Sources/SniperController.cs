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
    [System.Serializable]
    public struct Anticipation
    {
        const float BASE_WIDTH = 0.125f;
        const float WIDTH      = 0.5f;

        public Transform    transform;
        public MeshRenderer renderer;
        [System.NonSerialized]
        public Material     material;

        public void Init()
        {
            renderer.material = material = new Material(renderer.material);
            material.SetFloat("_Width", (1f / WIDTH) * BASE_WIDTH);
        }

        public void SetTransform(Vector3 self, Vector3 target)
        {
            const float OVERSHOOT = 15f;
            float distance = Vector3.Distance(self, target);
            Vector3 position = Vector3.LerpUnclamped(self, target, (1 + OVERSHOOT / distance) * 0.5f);
            transform.position  = position;
            Vector3 scale = new Vector3(
                WIDTH,
                distance + OVERSHOOT,
                1f
            );
            Quaternion rotation  = Quaternion.LookRotation(target - self, Vector3.up);
            rotation             *= Quaternion.Euler(90.0f, 0f, 0.0f);
            transform.rotation   = rotation;
            transform.localScale = scale;
        }

        public void SetProgress(float value)
        {
            material.SetFloat("_Progress", value);
        }
    }

    public Anticipation anticipation;

    protected override void Start()
    {
        base.Start();
        if (life)
            life.currentLife = life.maxLife = 100;
        anticipation.Init();
        SetState(SniperState.FOLOWING);
    }

    protected override void OnStateEnter(SniperState state)
    {
        switch (state)
        {
            case SniperState.FOLOWING:
                {
                    anticipation.SetProgress(0.0f);
                }
                break;
            case SniperState.ANTICIPATING_ATTACK:
                {
                    anticipation.SetProgress(0.125f);
                }
                break;
        }
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

                    bool reached_agro_range = dist_to_target <= 20f;
                    if (reached_agro_range)
                    {
                        state = SniperState.ANTICIPATING_ATTACK;
                    }
                }
                break;
            case SniperState.ANTICIPATING_ATTACK:
                {
                    const float ANTICIPATION_LIFESPAN = 0.75f;
                    //anticipation.SetProgress(state_lifetime/ANTICIPATION_LIFESPAN);
                    if (state_lifetime >= ANTICIPATION_LIFESPAN)
                    {
                        state = SniperState.ACTIVATING_ATTACK;
                    }
                }
                break;
            case SniperState.ACTIVATING_ATTACK:
                {
                    const float ATACK_LIFESPAN = 0.25f;
                    anticipation.SetProgress(1 - (state_lifetime / ATACK_LIFESPAN));
                    if (state_lifetime >= ATACK_LIFESPAN)
                    {
                        state = SniperState.FOLOWING;
                    }
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
                    anticipation.SetTransform(transform.position, target);
                }
                break;
            case SniperState.ANTICIPATING_ATTACK:
                {
                    anticipation.SetTransform(transform.position, target);
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
