using UnityEngine;
using System.Collections;

public class FlowBehavior : MonoBehaviour {

    public float delta = 0.1f, radius = 1f, scale = 1f;
    public GameObject fluid;

    private float time;
	void Start () {
        time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - time >= delta)
        {
            time = Time.time;
            float randonAngle = Random.Range(0, 360);
            Vector3 down = - transform.up;
            Vector3 right = transform.right;
            float r = Random.Range(-radius, radius), alpha = Random.Range(-Mathf.PI,Mathf.PI);
            Vector3 localPos = new Vector3(r * Mathf.Cos(alpha), 0, r * Mathf.Sin(alpha));

            GameObject __fluid = (GameObject) Instantiate(fluid, transform.TransformPoint(localPos), Quaternion.Euler(Vector3.one * randonAngle));
            __fluid.transform.parent = gameObject.transform;
            __fluid.transform.localScale = Vector3.one * scale;
        }
	}
}
