using UnityEngine;
using System.Collections;

public class SpiritBaseRotate : MonoBehaviour {


    private float _PreAngle;
    public float _Angle;
    public float _Period;
    public TouchPad touchPad;
    private float _Time;
    // Use this for initialization
    private bool rotateLeft;
    private GameController gameController;
    private Rigidbody2D rigidbody;
    public GameObject mainStar;

    public AudioClip spiritMoveSound;
    private bool isTap = false;

    void Start () {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
            gameController = gameControllerObject.GetComponent<GameController>();

        _PreAngle = _Angle-60;
        rigidbody = GetComponent<Rigidbody2D>();
        rotateLeft = false;
    }

    void Update()
    {
        if (gameController.isGameBegin())
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                // use the input stuff
                isTap = Input.GetMouseButton(0);
            }
            else
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    isTap = true;
            }
        }
    }

    void FixedUpdate() {
        //transform.Rotate(Vector3.forward * tumble);
        //if (touchPad.GetTouchCount() > 0 && !touchPad.isTouched() && touchPad.isFirstTouch())
        if (!gameController.isGameOver())
        {
            if (isTap)
            {
                if(gameController.isGameBegin() && !gameController.isGameStarted())
                    gameController.TapTutorialEnd();
                isTap = false;
                rotateLeft = !rotateLeft;
                touchPad.setFirstTouch(false);
                SoundManager.instance.playMulti(1.0f,spiritMoveSound);
            }
            //if (player.GetTouchCount() > 0 && !player.isTouched())
            //    _Angle = _Angle - 5;

            if (rotateLeft)
            {
                _Time = _Time - Time.deltaTime;
            }
            else
            {
                _Time = _Time + Time.deltaTime;
            }


            float phase = Mathf.Sin(_Time / _Period);
            //transform.rotation = Quaternion.Euler(new Vector3(0, 0, phase * _Angle));
            if (gameController.isGameStarted())
                rigidbody.rotation = phase * _Angle;
            else
                rigidbody.rotation = phase * _PreAngle;
            //rigidbody.MoveRotation(rigidbody.rotation + 50 * Time.fixedDeltaTime);
        }
        else
        {
            if (rigidbody != null && mainStar != null)
            {
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                mainStar.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
