using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour
{
    public int speed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {

        transform.Rotate(Vector3.right, speed* Time.deltaTime);
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
	}
}
