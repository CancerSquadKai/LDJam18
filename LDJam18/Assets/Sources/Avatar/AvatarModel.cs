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
	public System.Action onDashRefill;
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

    public List<float> dash_cooldown_progresses;

    public int slash_level;

    public int dash_level;

    public int shot_level;

    public BulletTrap shot;

    public AvatarModel(AvatarConfig config, AvatarView view, BulletTrap shot)
    {
        // feed config
        this.config = config;
        // feed view
        this.view   = view;

        this.shot = shot;

        // refill all dash charges
        dash_cooldown_progresses = new List<float>(6);
        int dash_count = dash_cooldown_progresses.Capacity;
        for (int dash_index = 0; dash_index < dash_count; ++dash_index)
            dash_cooldown_progresses.Add(1.0f);
        
        slash_level = config.slash.Length;
        NerfSlash();

        dash_level  = config.dash.Length - 1;

        shot_level  = config.shot.Length;
        NerfShot();

        // rs dash
        dash.progress_position = 1f;
    }

    public void UpdateView()
    {
        view.UpdateDashCooldowns(dash_cooldown_progresses);
    }

    /// <summary>
    /// Change model state depending on user inputs.
    /// </summary>
    public void UpdateInput()
    {
        velocity_movement = input_movement.normalized * config.speed_movement;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NerfDash();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            NerfSlash();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            NerfShot();
        }
    }

    public void UpdatePhysics(float dt)
    {
        // tranlation
        translation = Vector2.zero;
        // dash
        RefillDashes(dt / config.dash[dash_level].cooldown);
        if (dash.progress_position < 1) {
            float then = config.dash_curve.Evaluate(dash.progress_position);
            dash.progress_position += dt * dash.progress_velocity;
            dash.progress_position = Mathf.Clamp01(dash.progress_position);
            float now  = config.dash_curve.Evaluate(dash.progress_position);
            float delta = now - then;
            translation += delta * direction * config.dash[dash_level].distance;

            if(dash.progress_position >= 1)
            {
                view.OnDashEnd();
            }
            if(dash.progress_position >= config.dash[dash_level].invulnerability)
            {
                var life = view.GetComponent<Life>();
                if (life)
                    life.isInvulnerableToBullet = false;
            }
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

    public void RefillDashes(float amounth)
    {
        int dash_count = dash_cooldown_progresses.Count;
        for (int dash_index = 0; dash_index < dash_count; ++dash_index)
		{
			if(dash_cooldown_progresses[dash_index]<1)
			{
				dash_cooldown_progresses[dash_index] += amounth;
				if(dash_cooldown_progresses[dash_index]>=1)
				{
					if(onDashRefill != null)
					{
						onDashRefill.Invoke();
					}
				}
			}
		}
    }

    public void Dash()
    {
        int available_dash_index = -1;
        // find available charge
        int dash_count = dash_cooldown_progresses.Count;
        for (int dash_index = ((config.dash.Length - 1) - dash_level) * 2; dash_index < dash_count; ++dash_index)
        {
            if(dash_cooldown_progresses[dash_index] >= 1.0f)
            {
                available_dash_index = dash_index;
                break;
            }
        }
        if (available_dash_index == -1) // no dash charge found
            return;

        // put charge on cooldown
        dash_cooldown_progresses[available_dash_index] = 0.0f;

        dash = new AvatarDash()
        {
            progress_velocity = 1f / config.dash[dash_level].duration,
            progress_position = 0,
            direction         = direction
        };
        view.OnDashBegin();
        var life = view.GetComponent<Life>();
        if (life)
            life.isInvulnerableToBullet = true;
    }

    public void NerfSlash()
    {
        --slash_level;
        slash_level = Mathf.Clamp(slash_level, 0, config.slash.Length - 1);

        view.OnNerfSlash(slash_level);
    }

    public void NerfDash()
    {
        --dash_level;
        dash_level = Mathf.Clamp(dash_level, 0, config.dash.Length - 1);

        view.OnNerfDash(dash_level);

        for (int i = 0; i < Mathf.Min(dash_cooldown_progresses.Count, dash_level * 2); ++i)
            dash_cooldown_progresses[i] = 0f;
    }

    public void NerfShot()
    {
        --shot_level;
        shot_level = Mathf.Clamp(shot_level, 0, config.shot.Length - 1);

        if (!shot) return;
        foreach(var burst in shot.bursts)
        {
            burst.spawnedObject = config.shot[shot_level].prefab;
        }
        shot.timeBetweenBursts = config.shot[shot_level].cooldown;
    }
}
