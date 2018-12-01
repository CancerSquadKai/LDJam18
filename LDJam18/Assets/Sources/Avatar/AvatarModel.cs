using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarModel
{
    /// <summary>
    /// Current position of the avatar.
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// Current movement velocity.
    /// </summary>
    public Vector2 velocity_movement;

    /// <summary>
    /// Current bump velocity.
    /// </summary>
    public Vector2 velocity_bump;

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

    public void Update(float dt)
    {
        UpdateInput();
        UpdatePhysics(dt);
        UpdateView();
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

        Vector2 velocity = Vector2.zero;
        velocity += velocity_movement;
        velocity += velocity_bump;

        // position
        position += velocity * dt;
    }

    public void UpdateView()
    {
        view.position = position;
    }

    public void Bump(Vector2 direction, float distance, float duration)
    {
        acceleration_bump = (2 * distance) / (duration * duration);
        Vector2 impulse_bump = direction * ((2 * distance) / duration);
        velocity_bump += impulse_bump;
    }
}
