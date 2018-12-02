using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AvatarDash
{
    public float progress_velocity;
    public float progress_position;
    public Vector2 direction;
}

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

    public Vector2 translation;

    public Vector2 direction = Vector2.down;

    /// <summary>
    /// Current bump velocity.
    /// </summary>
    public float acceleration_bump;

    /// <summary>
    /// Left stick input.
    /// </summary>
    public Vector2 input_movement;

    /// <summary>
    /// Right stick input.
    /// </summary>
    public Vector2 input_shoot;

    /// <summary>
    /// Serialized avatar configuration file.
    /// </summary>
    public AvatarConfig config;

    /// <summary>
    /// Sign & feedback related code.
    /// </summary>
    public AvatarView view;

    public AvatarDash dash;

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
        // tranlation
        translation = Vector2.zero;
        // dash
        if (dash.progress_position < 1) {
            float then = config.dash_curve.Evaluate(dash.progress_position);
            dash.progress_position += dt * dash.progress_velocity;
            dash.progress_position = Mathf.Clamp01(dash.progress_position);
            float now  = config.dash_curve.Evaluate(dash.progress_position);
            float delta = now - then;
            translation += delta * direction * config.dash_distance;

            if(dash.progress_position >= 1)
                view.OnDashEnd();
        }

        // velocity
        // bump
        {
            velocity_bump = Vector2.MoveTowards(
                velocity_bump,
                Vector3.zero,
                acceleration_bump * dt
            );
        }

        velocity = Vector2.zero;
        velocity += velocity_movement;
        velocity += velocity_bump;

        if(velocity.magnitude > 0.125f)
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

    public void Dash()
    {
        dash = new AvatarDash()
        {
            progress_velocity = 1f / config.dash_duration,
            progress_position = 0,
            direction         = direction
        };
        view.OnDashBegin();
    }
}
