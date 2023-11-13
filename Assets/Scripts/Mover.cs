using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

    public float speed;

    void Start()
    {
        //if (gameObject.tag == "circle"|| gameObject.tag == "Cloud")
        //{
           Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
           rigidbody.velocity = transform.up * speed;
        // }
    }
}
