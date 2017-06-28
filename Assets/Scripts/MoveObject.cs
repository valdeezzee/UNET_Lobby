using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {
    private Rigidbody r;
	// Use this for initialization
	void Start () {
        r = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.P))
            transform.Translate(Vector3.up * Time.deltaTime);

        


    }
}
