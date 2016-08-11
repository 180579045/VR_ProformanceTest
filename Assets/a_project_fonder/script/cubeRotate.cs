using UnityEngine;
using System.Collections;

public class cubeRotate : MonoBehaviour {

	float angle_rate = 10;

	void Start () {
		angle_rate = Random.Range(20,60);
	
	}

	void Update () {
	
			this.transform.Rotate(Vector3.up,angle_rate *Time.deltaTime);
	
	}
}
