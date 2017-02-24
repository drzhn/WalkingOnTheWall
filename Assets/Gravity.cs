using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

	// Use this for initialization
    public float gravity_magnitude = 10;

    private Vector3 gravity = Vector3.down;
    private Rigidbody rigidbody;
	void Start () {
        rigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        rigidbody.AddForce(gravity * gravity_magnitude, ForceMode.Acceleration);
    }

    public void SetGravityDirection(Vector3 direction)
    {
        gravity = direction;
    }

    public void AddGravityDirection(Vector3 direction)
    {
        gravity += direction;
    }

    public void ChangeDirection(Vector3 newDirection)
    {
        rigidbody.velocity = Vector3.zero;
        gravity = newDirection;
    }
}
