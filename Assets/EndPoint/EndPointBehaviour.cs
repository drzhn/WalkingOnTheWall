using UnityEngine;
using System.Collections;

public class EndPointBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            GameObject gm = GameObject.Find("GameManager");
            gm.SendMessage("Win");
        }
    }
}
