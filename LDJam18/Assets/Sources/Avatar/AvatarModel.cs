using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarModel
{
    /// <summary>
    /// Current movement velocity.
    /// </summary>
    public Vector2 velocity_movement;

    /// <summary>
    /// Current bump velocity.
    /// </summary>
    public Vector2 velocity_bump;

    /// <summary>
    /// Packed total velocity.
    /// </summary>
    public Vector2 velocity;

    public Vector2 direction;

    /// <summary>
    /// Current bump velocity.
    /// </summary>
    public float acceleration_bump;

    /// <summary>
    /// Left stick input.
    /// </summary>
    public Vector2 input_movement;

    /// <summary>
    /// Serialized avatar configuration file.
    /// </summary>
    public AvatarConfig config;

    /// <summary>
    /// Sign & feedback related code.
    /// </summary>
    public AvatarView view;

    public AvatarModel(AvatarConfig config, AvatarView view)
    {
        // feed config
        this.config = config;
        // feed view
        this.view   = view;
    }

    /// <summary>
    /// Change model state depending on user inputs.
    /// </summary>
    public void UpdateInput()
    {
        velocity_movement = input_movement.normalized * config.speed_movement;
    }

    public void UpdatePhysics(float dt)
    {
        // velocity
        velocity_bump = Vector2.MoveTowards(
            velocity_bump,
            Vector3.zero,
            acceleration_bump * dt
        );

        velocity = Vector2.zero;
        velocity += velocity_movement;
        velocity += velocity_bump;

        if(velocity.magnitude > 0.25)
        {
            direction = velocity.normalized;
        }
    }
    
    public void Bump(Vector2 direction, float distance, float duration)
    {
        acceleration_bump = (2 * distance) / (duration * duration);
        Vector2 impulse_bump = direction * ((2 * distance) / duration);
        velocity_bump += impulse_bump;
    }
}
