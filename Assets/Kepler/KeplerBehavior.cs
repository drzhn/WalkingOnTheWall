using UnityEngine;
using System.Collections;

public class KeplerBehavior : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider col)
    {
        Rigidbody rig = col.GetComponent<Rigidbody>();
        Vector3 pos = col.transform.position;
        //rig.AddForce((transform.position - pos).normalized * 130 / (transform.position - pos).sqrMagnitude,
        //    ForceMode.Acceleration);
        col.gameObject.SendMessage("SetGravityDirection", (transform.position - pos).normalized * 20 / (transform.position - pos).sqrMagnitude);
    }
}
