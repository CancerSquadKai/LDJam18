using UnityEngine;

public interface IBumpable
{
    void Bump(Vector3 direction, float distance, float duration);
}

public struct Bump
{
    public float   acceleration;
    public Vector3 velocity;

    public void Add(Vector3 direction, float distance, float duration)
    {
        float acceleration = (2 * distance) / (duration * duration);
        this.acceleration = Mathf.Max(this.acceleration, acceleration);
        velocity += direction * ((2 * distance) / duration);

    }

    public void Update(float dt)
    {
        velocity = Vector3.MoveTowards(
            velocity,
            Vector3.zero,
            acceleration * dt
        );
        if (velocity.magnitude <= 0)
        {
            acceleration = 0;
        }
    }
}