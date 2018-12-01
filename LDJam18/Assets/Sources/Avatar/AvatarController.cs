using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour {

    public AvatarView   view;
    public AvatarConfig config;
    public AvatarModel  model;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        model = new AvatarModel(config, view);
    }

    private void Update ()
    {
        // inputs
        {
            Vector2 left_stick_input = Vector2.zero;
            left_stick_input.x       = Input.GetAxisRaw("Horizontal");
            left_stick_input.y       = Input.GetAxisRaw("Vertical");
            model.input_movement     = left_stick_input;
        }



        // model
        model.UpdateInput();
    }

    private void FixedUpdate()
    {
        model.UpdatePhysics(Time.fixedDeltaTime);
        rigidbody.velocity = new Vector3(
            model.velocity.x,
            0,
            model.velocity.y
            );
    }

    public void Bump(Vector3 direction, float distance, float duration)
    {
        model.Bump(
            new Vector2(
                direction.x,
                direction.z
            ),
            distance,
            duration 
        );
    }
}
