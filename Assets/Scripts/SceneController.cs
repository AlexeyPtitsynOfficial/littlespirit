using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public GameObject loadingStar;
    private RectTransform rectTransform;
    // Use this for initialization
    void Start () {
        Application.targetFrameRate = 60;
        GlobalVariables.GameOverCount = 0;

        if (PlayerPrefs.GetInt("SoundMute", 0) == 0)
        {
            if (SoundManager.instance.musicSource.isPlaying == false)
                SoundManager.instance.musicSource.Play();
        }
        else
        {
            AudioListener.pause = true;
            //AudioListener.volume = 0;
        }

        rectTransform = loadingStar.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

        if (PlayServicesController.Instance.isLocalDataLoad() || PlayServicesController.Instance.isCloudDataLoad())
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            rectTransform.Rotate(new Vector3(0, 0, 5));
        }
    }
}
