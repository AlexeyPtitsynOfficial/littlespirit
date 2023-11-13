using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {

    private GameController gameController;
    public float tumble;
	// Use this for initialization
	void Start () {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.angularVelocity = Random.value * tumble;
	}
}
