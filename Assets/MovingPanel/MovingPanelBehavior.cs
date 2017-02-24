using UnityEngine;
using System.Collections;

public class MovingPanelBehavior : MonoBehaviour
{

    // Use this for initialization
    private Transform parent;
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        collider.gameObject.SendMessage("ChangeDirection", -parent.transform.up);
    }
}
