using UnityEngine;
using System.Collections;

public class SpiritTrigger : MonoBehaviour {

    public GameController gameController;

    void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "Star")
        {
            gameController.GameOver();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "StarCoin")
        {
            gameController.AddStarCoin();
            Destroy(other.gameObject);
        }
    }
}
