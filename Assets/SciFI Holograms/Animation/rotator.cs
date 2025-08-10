using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour {
    public float xSpeed = 0.0f;
    public float ySpeed = 1.0f;
    public float zSpeed = 0.0f;
    // Use this for initialization
    void Start () {
	
	}

    void Update()
    {
        transform.Rotate(
             xSpeed * Time.deltaTime,
             ySpeed * Time.deltaTime,
             zSpeed * Time.deltaTime
        );
    }
}
