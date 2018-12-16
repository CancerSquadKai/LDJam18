using UnityEngine;

public abstract class EnemyController : MonoBehaviour, IBumpable
{
    public        Bump      bump;
    public        Life      life;
    protected new Rigidbody rigidbody;

    protected virtual void Awake()
    {
        life      = GetComponent<Life>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Bump(Vector3 direction, float distance, float duration)
    {
        bump.Add(direction, distance, duration);
    }
}

public abstract class EnemyController<T_StateEnum> : EnemyController
{
    public AvatarController target;

    public T_StateEnum      state { get; private set; }
    public float            state_lifetime { get; private set; }

    public struct Movement
    {
        public Vector3 velocity;
        public Vector3 position;
        public Vector3 position_offset;
    }

    protected virtual void Start()
    {
        if (!target)
            target = FindObjectOfType<AvatarController>();
    }

    protected void Update()
    {
        float dt = Time.deltaTime;
        state_lifetime += dt;
        var new_state = StateUpdate(state, dt);
        if(!state.Equals(new_state))
        {
            SetState(new_state);
        }
    }

    protected void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        bump.Update(dt);
        Movement movement = new Movement();
        movement.velocity = bump.velocity;
        movement.position = rigidbody.position;
        movement = MovementUpdate(movement, target.transform.position, dt);

        Vector3 direction = movement.position - rigidbody.position;
        if (direction.magnitude > 0.01f)
        {
            direction.y = 0f;
            direction = direction.normalized;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(
                    direction,
                    Vector3.up
                ),
                270 * dt
            );
        }

        if (rigidbody)
        {
            rigidbody.velocity = movement.velocity;
            rigidbody.position = movement.position;
        }
        transform.position += movement.position_offset;
    }

    protected void SetState(T_StateEnum new_state)
    {
        state          = new_state;
        state_lifetime = 0.0f;
        OnStateEnter(state);
    }

    protected virtual void OnStateEnter(T_StateEnum state) { }

    protected virtual T_StateEnum StateUpdate(T_StateEnum state, float dt)
    {
        return state;
    }

    protected virtual Movement MovementUpdate(Movement movement, Vector3 target, float dt)
    {
        movement.position = Vector3.MoveTowards(movement.position, target, 10f * dt);
        return movement;
    }
}
