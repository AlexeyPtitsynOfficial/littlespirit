using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour
{
    public GameController gameController;

    public int scoreValue;

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Star")
            gameController.AddScore(scoreValue);
        else if (other.gameObject.tag == "MainStar")
            other.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        if (other.gameObject.tag != "MainStar")
            Destroy(other.gameObject);
    }
}
