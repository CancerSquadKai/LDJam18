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

    public void UpdateInput()
    {
        velocity_movement = input_movement.normalized * config.speed_movement;
    }

    public void UpdatePhysics(float dt)
    {
        Vector2 velocity = Vector2.zero;
        velocity += velocity_movement;
        position += velocity * dt;
    }

    public void UpdateView()
    {
        view.position = position;
    }
}
