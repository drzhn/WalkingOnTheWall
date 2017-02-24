using UnityEngine;
using System.Collections;

public class FluidBehavior : MonoBehaviour
{

    public float gravity = 25;

    private Transform parent;
    private Rigidbody rigidbody;
    private Vector3 direction;

    private float t;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        parent = transform.parent;
        direction = -parent.up;
        t = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - t > 1f)
        {
            rigidbody.velocity = parent.right * rigidbody.velocity.magnitude;
            direction = parent.right;
        }
        if (Time.time - t > 10)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(direction * gravity);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Environment")
            Destroy(gameObject);
    }
}
