using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class GameController : MonoBehaviour {

    const string SAVE_NAME = "LittleSpiritSaves";

    private PlayServicesController playServicesController;
    private ShopController shopController;

    bool isSaving;
    bool isCloudDataLoaded = false;

    public GameObject Spirit;
    public GameObject[] clouds;
    public GameObject mainStar;
    public GameObject star;
    public GameObject star_coin;
    public GameObject hintLight;

    public float cloudSpawnWait;
    public float starcoinSpawnWait;

    public Vector2 spawnValues;
    public float startWait;
    public float spawnWait;

    public GameObject tapFinger;
    public Text stepDescText;

    public Text NewRecordText;
    public Text StarCoinsText;
    public Text TotalScore;
    public Text scoreText;

    public Button beginTrainigButton;
    public Button nextStepButton;
    public Button startButton;
    public Button muteButton;

    public Sprite muteOffImage;
    public Sprite muteOnImage;

    public Text gameOverText;

    public GameObject loadScreen;
    public GameObject startMenu;
    public GameObject Tutorial;
    public GameObject restartMenu;
    public GameObject shopMenu;
    private bool gameBegin;
    private bool gamestart;
    private bool tutorial;
    private bool gameOver;
    private bool restart;
    private int score;
    private int prevStarCoins;
    private const string leaderboard = "CgkI8uaPkrwaEAIQAA";

    #region AudioClips
    public AudioClip backMusic;
    public AudioClip buttonTapSound;
    public AudioClip starShootingSound;
    public AudioClip starTakeSound;
    public AudioClip gameOverSound;
    #endregion

    private BannerView bottomBanner;
    private InterstitialAd interstitial;

    void Start() {
        gameBegin = false;
        gamestart = false;
        tutorial = false;
        gameOver = false;
        restart = false;

        //loadScreen.SetActive(true);
        Tutorial.SetActive(false);
        //startMenu.SetActive(false);
        shopMenu.SetActive(false);
        restartMenu.SetActive(false);
        tapFinger.SetActive(false);
        score = PlayerPrefs.GetInt("PrevScore", 0);

        if (PlayerPrefs.GetInt("SoundMute", 0) == 1)
        {
            muteButton.GetComponent<Image>().sprite = muteOnImage;
        }

        UpdateScore();
        UpdateHighScore();
        UpdateStarCoins();
        NewRecordText.enabled = false;
        //InstructionText.enabled = false;

        Spirit.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Spirit_skins/spirit_skin_"+CloudVariables.CurrentSkin.ToString());

        prevStarCoins = CloudVariables.StarCoins;

        #if UNITY_ANDROID
                string appId = "ca-app-pub-5591099082504824~4667851752";
        #elif UNITY_IPHONE
                    string appId = "ca-app-pub-3940256099942544~1458002511";
        #else
                    string appId = "unexpected_platform";
        #endif

        MobileAds.Initialize(appId);

        this.RequestBanner();
        this.RequestInterstitial();

        //StartCoroutine(DoFade());
    }

    void OnDestroy()
    {
        //bottomBanner.Destroy();
        if ((Application.isPlaying == false) && (Application.isEditor == true) && (Application.isLoadingLevel == false))
            Debug.Log(this.name + " is destroyed");
    }

    //IEnumerator DoFade()
    //{
    //    //CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
    //    //while (canvasGroup.alpha > 0)
    //    //{
    //    //    canvasGroup.alpha -= Time.deltaTime / 2;
    //    //    yield return null;
    //    //}
    //    //canvasGroup.interactable = false;
    //    //yield return null;
    //}

    public void StartTutorial(int step)
    {
        score = 0;
        UpdateScore();
        gameOver = false;
        Tutorial.SetActive(false);
        StartCoroutine(SpawnClouds());
        if(step == 1)
            StartCoroutine(SpawnWaves());
        else if (step == 2)
        {
            StartCoroutine(SpawnStarCoins());
            stepDescText.GetComponent<LocalizedText>().key = "step_2_task_text";
        }
    }

    public void StartGame()
    {
        SoundManager.instance.musicSource.Stop();
        SoundManager.instance.playTap(buttonTapSound);
        //SoundManager.instance.playBackMusic(backMusic);
        bottomBanner.Hide();
        startMenu.SetActive(false);
        tapFinger.SetActive(true);
        score = 0;
        UpdateScore();

        gameBegin = true;
    }

    public void TapTutorialEnd()
    {
        StartCoroutine(SpawnClouds());
        StartCoroutine(SpawnWaves());
        StartCoroutine(SpawnStarCoins());

        mainStar.GetComponent<Rigidbody2D>().angularVelocity = Random.value * 100;

        tapFinger.SetActive(false);
        gamestart = true;
    }

    public void SaveGame()
    {
        PlayServicesController.Instance.SaveData();
    }

    #region dev_buttons
    public void DevSetFirstRun()
    {
        PlayServicesController.Instance.DeleteGameData();
        PlayerPrefs.DeleteAll();
    }

    public void DevAddStarCoin()
    {
        CloudVariables.StarCoins = CloudVariables.StarCoins + 100;
        UpdateStarCoins();
    }

    public void DevClearSkins()
    {
        CloudVariables.Skins.Clear();
    }
    #endregion

    #region menu buttons
    public void LeaderBoardList()
    {
        SoundManager.instance.playTap(buttonTapSound);
        PlayServicesController.ShowLeaderboardsUI();
    }

    public void rateApp()
    {
        SoundManager.instance.playTap(buttonTapSound);
        #if UNITY_ANDROID
        Application.OpenURL("http://play.google.com/store/apps/details?id=" + Application.identifier);
        #endif
    }

    public void Mute() {
        if (PlayerPrefs.GetInt("SoundMute", 1) == 1)
        {
            PlayerPrefs.SetInt("SoundMute", 0);
            AudioListener.pause = false;
            //AudioListener.volume = 1.0f;
            muteButton.GetComponent<Image>().sprite = muteOffImage;
        }
        else {
            PlayerPrefs.SetInt("SoundMute", 1);
            AudioListener.pause = true;
            //AudioListener.volume = 0;
            muteButton.GetComponent<Image>().sprite = muteOnImage;
        }
    }

    public void Shop(bool active)
    {
        SoundManager.instance.playTap(buttonTapSound);
        shopMenu.SetActive(active);
    }
    #endregion


    public void setSkin(int skin_number)
    {
        CloudVariables.CurrentSkin = skin_number;
        PlayServicesController.Instance.SaveData();
        Spirit.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Spirit_skins/spirit_skin_" + skin_number);
        UpdateStarCoins();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && shopMenu.activeSelf)
            Shop(false);
        else if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    void UpdateHighScore()
    {
        TotalScore.text = CloudVariables.Highscore.ToString();
    }

    void UpdateStarCoins()
    {
        StarCoinsText.text = CloudVariables.StarCoins.ToString();
    }

    public void AddScore(int newScoreValue)
    {
        if (!gameOver)
        {
            score += newScoreValue;
            UpdateScore();
        }
    }

    public void AddStarCoin()
    {
        CloudVariables.StarCoins = CloudVariables.StarCoins + 10;
        //PlayServicesController.Instance.SaveData();
        UpdateStarCoins();
        SoundManager.instance.playMulti(1.0f, starTakeSound);
    }

    IEnumerator SpawnClouds()
    {

        yield return new WaitForSeconds(startWait);

        while (true)
        {
            for (int i = 0; i < clouds.Length; i++)
            {
                GameObject cloud = clouds[Random.Range(0, clouds.Length)];
                Vector2 spawnPos = new Vector2(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y);
                Quaternion spawnRot = Quaternion.identity;
                Instantiate(cloud, spawnPos, spawnRot);
                yield return new WaitForSeconds(cloudSpawnWait);
            }
            yield return new WaitForSeconds(cloudSpawnWait);

            if (gameOver)
            {
                startButton.gameObject.SetActive(true);
                restart = true;
                break;
            }
        }
    }

    IEnumerator SpawnWaves() {

        yield return new WaitForSeconds(startWait);

        while (true)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y);
            Vector2 hintSpawnPos = new Vector2(spawnPos.x, spawnPos.y-2);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hintLight, hintSpawnPos, spawnRotation);
            yield return new WaitForSeconds(spawnWait);
            Instantiate(star, spawnPos, spawnRotation);
            //SoundManager.instance.playMulti(0.1f,starShootingSound);


            if (gameOver)
            {
                startButton.gameObject.SetActive(true);
                restart = true;
                break;
            }
        }
    }

    IEnumerator SpawnStarCoins()
    {

        yield return new WaitForSeconds(startWait);

        while (true)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y);
            Quaternion spawnRotation = Quaternion.identity;
            yield return new WaitForSeconds(starcoinSpawnWait);
            Instantiate(star_coin, spawnPos, spawnRotation);



            if (gameOver)
            {
                startButton.gameObject.SetActive(true);
                restart = true;
                break;
            }
        }
    }

    public bool isGameBegin()
    {
        return gameBegin;
    }

    public bool isGameStarted()
    {
        return gamestart;
    }

    public bool isTutorial()
    {
        return tutorial;
    }

    public bool isGameOver()
    {
        return gameOver;
    }

    public void GameOver()
    {
        SoundManager.instance.playMulti(0.1f, gameOverSound);
        SoundManager.instance.musicSource.Stop();
        bottomBanner.Show();
        GlobalVariables.GameOverCount += 1;
        if (interstitial.IsLoaded() && GlobalVariables.GameOverCount == Random.Range(2,6))
        {
            GlobalVariables.GameOverCount = 0;
            interstitial.Show();
        }
        PlayerPrefs.SetInt("PrevScore", score);
        if (score > CloudVariables.Highscore)
        {
            CloudVariables.Highscore = score;
            PlayServicesController.Instance.SaveData();
            NewRecordText.enabled = true;
            UpdateHighScore();
            PlayServicesController.AddScoreToLeaderboard(GPGSIds.leaderboard_top, CloudVariables.Highscore);
        }
        else if(CloudVariables.StarCoins > prevStarCoins)
            PlayServicesController.Instance.SaveData();

        tapFinger.SetActive(false);
        restartMenu.SetActive(true);
        gamestart = false;
        gameOver = true;
    }

    public void RestartGame()
    {
        SoundManager.instance.playTap(buttonTapSound);
        bottomBanner.Destroy();
        SceneManager.LoadScene(1);
    }

    

    private void RequestBanner()
    {
        //Test ad - ca-app-pub-3940256099942544/6300978111
        #if UNITY_ANDROID
        string adUnitId = "ca-app-pub-5591099082504824/4616289981";
        //#elif UNITY_IPHONE
        // string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        //#else
        //string adUnitId = "unexpected_platform";
        #endif

        // Create a 320x50 banner at the top of the screen.
        bottomBanner = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
            //.AddTestDevice(SystemInfo.deviceUniqueIdentifier.ToUpper())
            .Build();

        // Load the banner with the request.
        bottomBanner.LoadAd(request);

        bottomBanner.Show();
    }

    private void RequestInterstitial()
    {
        //Test ad - ca-app-pub-3940256099942544/1033173712
        #if UNITY_ANDROID
        string adUnitId = "ca-app-pub-5591099082504824/6540506473";
        #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
                string adUnitId = "unexpected_platform";
        #endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
            //.AddTestDevice(SystemInfo.deviceUniqueIdentifier.ToUpper())
            .Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }
}