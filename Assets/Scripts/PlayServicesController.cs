using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using UnityEngine;
using Boomlagoon.JSON;
using System;
using System.Collections;

public class PlayServicesController : MonoBehaviour {

    public static PlayServicesController Instance { get; private set; }

    const string SAVE_NAME = "LittleSpiritSaves";
    bool isSaving;
    bool isCloudDataLoaded = false;
    bool isDataLoaded = false;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        //setting default value, if the game is played for the first time
        if (!PlayerPrefs.HasKey(SAVE_NAME))
        {
            PlayerPrefs.SetString(SAVE_NAME, string.Empty);
            //SaveLocal();
        }
        //tells us if it's the first time that this game has been launched after install - 0 = no, 1 = yes 
        CloudVariables.Skins.Clear();
        if (PlayerPrefs.GetInt("IsFirstTime", 1) == 1)
        {
            CloudVariables.Skins.Add(0);
            PlayerPrefs.SetInt("IsFirstTime", 0);

        }

        if (PlayerPrefs.GetInt("IsFirstTimePS",1) == 1)
        {
            PlayerPrefs.SetInt("IsFirstTimePS", 1);
            
        }

        //LoadData(); //we want to load local data first because loading from cloud can take quite a while, if user progresses while using local data, it will all
        //sync in our comparating loop in StringToGameData(string, string)

       /* StartCoroutine(checkInternetConnection((isConnected) => {
            if (true)
            {

            }
        }));*/

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        LocalStringToGameData(PlayerPrefs.GetString(SAVE_NAME));

        SignIn();

        SaveData();

    }

    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }

    public void SignIn()
    {
        if (!Social.localUser.authenticated)
            Social.localUser.Authenticate(success => {
                LoadData();
                
                 });
    }

    public bool isLocalDataLoad()
    {
        return isDataLoaded;
    }
    
    public bool isCloudDataLoad()
    {
        return isCloudDataLoaded;
    }

    #region Saved Games
    

    private void SaveLocal()
    {
        PlayerPrefs.SetString(SAVE_NAME, GameDataToJsonString());
    }
    //making a string out of game data (highscores...)
    string GameDataToJsonString()
    {
        JSONObject jObj = new JSONObject();

        jObj.Add("high_score", CloudVariables.Highscore);
        jObj.Add("star_coins", CloudVariables.StarCoins);
        jObj.Add("bought_skins", JsonUtil.CollectionToJsonArray(CloudVariables.Skins.ToArray()));
        jObj.Add("current_skin", CloudVariables.CurrentSkin);

        return jObj.ToString();
    }
    //this overload is used when user is connected to the internet
    //parsing string to game data (stored in CloudVariables), also deciding if we should use local or cloud save
    void CompareStringToGameData(string cloudData, string localData)
    {
        if (cloudData == string.Empty)
        {
            if (localData != string.Empty)
                LocalStringToGameData(localData);
            isCloudDataLoaded = true;
            return;
        }
        
        long cloud_highscore = Int32.Parse(JsonUtil.JsonStringToStr(cloudData, "high_score"));

        //if(Social.localUser.authenticated)
        //    PlayGamesPlatform.Instance.LoadScores(
        //                GPGSIds.leaderboard_top,
        //                LeaderboardStart.PlayerCentered,
        //                1,
        //                LeaderboardCollection.Public,
        //                LeaderboardTimeSpan.AllTime,
        //            (LeaderboardScoreData data) => {
        //                cloud_highscore = data.PlayerScore.value;
        //            });

        int cloud_star_coins = Int32.Parse(JsonUtil.JsonStringToStr(cloudData, "star_coins"));
        int[] cloud_skins = JsonUtil.JsonStringToArray(cloudData, "bought_skins", str => int.Parse(str));
        int cloud_current_skin = Int32.Parse(JsonUtil.JsonStringToStr(cloudData, "current_skin"));

        if (localData == string.Empty)
        {
            CloudVariables.Highscore = cloud_highscore;
            CloudVariables.StarCoins = cloud_star_coins;
            CloudVariables.Skins.Clear();
            for (int i=0; i<cloud_skins.Length; i++)
                CloudVariables.Skins.Add(cloud_skins[i]);

            CloudVariables.CurrentSkin = cloud_current_skin;
            PlayerPrefs.SetString(SAVE_NAME, cloudData);
            isCloudDataLoaded = true;
            return;
        }

        int local_highscore = Int32.Parse(JsonUtil.JsonStringToStr(localData, "high_score"));
        int local_star_coins = Int32.Parse(JsonUtil.JsonStringToStr(localData, "star_coins"));
        int[] local_skins = JsonUtil.JsonStringToArray(localData, "bought_skins", str => int.Parse(str));
        int local_current_skin = Int32.Parse(JsonUtil.JsonStringToStr(localData, "current_skin"));

        //if it's the first time that game has been launched after installing it and successfuly logging into Google Play Games
        if (PlayerPrefs.GetInt("IsFirstTimePS") == 1)
        {
            //set playerpref to be 0 (false)
            PlayerPrefs.SetInt("IsFirstTimePS", 0);
            CloudVariables.Skins.Clear();
            CloudVariables.Skins.Add(0);
            if ((cloud_highscore > local_highscore) || (cloud_star_coins != local_star_coins) || (cloud_current_skin != local_current_skin)) //cloud save is more up to date
            {
                //set local save to be equal to the cloud save
                PlayerPrefs.SetString(SAVE_NAME, localData);
            }

            if (cloud_skins.Length > local_skins.Length) //cloud save is more up to date
            {
                //set local save to be equal to the cloud save
                PlayerPrefs.SetString(SAVE_NAME, cloudData);
            }
        }
        //if it's not the first time, start comparing

        CloudVariables.Highscore = cloud_highscore;
        CloudVariables.StarCoins = cloud_star_coins;
        CloudVariables.Skins.Clear();
        for (int i = 0; i < cloud_skins.Length; i++)
            CloudVariables.Skins.Add(cloud_skins[i]);
        CloudVariables.CurrentSkin = cloud_current_skin;

        if (local_highscore > cloud_highscore) //cloud save is more up to date
        {
            CloudVariables.Highscore = local_highscore;
            if (Social.localUser.authenticated)
                AddScoreToLeaderboard(GPGSIds.leaderboard_top, CloudVariables.Highscore);
        }
            //comparing integers, if one int has higher score in it than the other, we update it
        if (local_skins.Length > cloud_skins.Length)
        {
            CloudVariables.StarCoins = local_star_coins;
            CloudVariables.CurrentSkin = local_current_skin;
            CloudVariables.Skins.Clear();
            for (int i = 0; i < local_skins.Length; i++)
                CloudVariables.Skins.Add(local_skins[i]);
        }
            
        //if the code above doesn't trigger return and the code below executes,
        //cloud save and local save are identical, so we can load either on

        isCloudDataLoaded = true;

        SaveData();

    }

    //this overload is used when there's no internet connection - loading only local data
    void LocalStringToGameData(string localData)
    {
        
        if (localData != string.Empty)
        {
            CloudVariables.Highscore = Int32.Parse(JsonUtil.JsonStringToStr(localData, "high_score"));
            CloudVariables.StarCoins = Int32.Parse(JsonUtil.JsonStringToStr(localData, "star_coins"));

            int[] local_skins = JsonUtil.JsonStringToArray(localData, "bought_skins", str => int.Parse(str));
            CloudVariables.Skins.Clear();
            for (int i = 0; i < local_skins.Length; i++)
                CloudVariables.Skins.Add(local_skins[i]);
            CloudVariables.CurrentSkin = Int32.Parse(JsonUtil.JsonStringToStr(localData, "current_skin"));
        }
    }

    //used for loading data from the cloud or locally
    public void LoadData()
    {
        if (Social.localUser.authenticated)
        {
            isSaving = false;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }
        else
        {
            LoadLocal();
        }

        if (CloudVariables.Skins.Count == 0)
        {
            CloudVariables.Skins.Clear();
            CloudVariables.Skins.Add(0);
            CloudVariables.CurrentSkin = 0;
        }
    }

    private void LoadLocal()
    {
        LocalStringToGameData(PlayerPrefs.GetString(SAVE_NAME));
        isDataLoaded = true;
    }

    //used for saving data to the cloud or locally
    public void SaveData()
    {
        //if we're still running on local data (cloud data has not been loaded yet), we also want to save only locally
        if (!isCloudDataLoaded)
        {
            SaveLocal();
            return;
        }
        //same as in LoadData
        if (Social.localUser.authenticated)
        {
            isSaving = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
            //((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(SAVE_NAME, DataSource.ReadCacheOrNetwork,
            //ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
        }
        else
        {
            SaveLocal();
        }
    }

    

    private void ResolveConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData,
        ISavedGameMetadata unmerged, byte[] unmergedData)
    {
        Debug.Log("ResolveConflict start");

        if (originalData == null)
            resolver.ChooseMetadata(unmerged);
        else if (unmergedData == null)
            resolver.ChooseMetadata(original);
        else
        {
            //decoding byte data into string
            string originalStr = "";
            string unmergedStr = "";
            int[] original_skins;
            int[] unmerged_skins;
            int origin_starcoins;
            int unmerged_starcoins;
            try
            {
                originalStr = Encoding.ASCII.GetString(originalData);
                original_skins = JsonUtil.JsonStringToArray(originalStr, "bought_skins", str => int.Parse(str));
                origin_starcoins = Int32.Parse(JsonUtil.JsonStringToStr(originalStr, "star_coins"));
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                resolver.ChooseMetadata(unmerged);
                return;
            }
            try
            {
                unmergedStr = Encoding.ASCII.GetString(unmergedData);
                unmerged_skins = JsonUtil.JsonStringToArray(unmergedStr, "bought_skins", str => int.Parse(str));
                unmerged_starcoins = Int32.Parse(JsonUtil.JsonStringToStr(unmergedStr, "star_coins"));
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                resolver.ChooseMetadata(original);
                return;
            }

            //parsing
            
            //if original score is greater than unmerged
            if (original_skins.Length > unmerged_skins.Length)
            {
                resolver.ChooseMetadata(original);
                return;
            }
            //else (unmerged score is greater than original)
            else if (unmerged_skins.Length > original_skins.Length)
            {
                resolver.ChooseMetadata(unmerged);
                return;
            }
            else if (origin_starcoins > unmerged_starcoins)
            {
                resolver.ChooseMetadata(original);
                return;
            }
            //else (unmerged score is greater than original)
            else if (unmerged_starcoins > origin_starcoins)
            {
                resolver.ChooseMetadata(unmerged);
                return;
            }
            //if return doesn't get called, original and unmerged are identical
            //we can keep either one
            resolver.ChooseMetadata(original);
        }
        Debug.Log("ResolveConflict end");
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //if we are connected to the internet
        if (status == SavedGameRequestStatus.Success)
        {
            //if we're LOADING game data
            if (!isSaving)
                LoadGame(game);
            //if we're SAVING game data
            else
                SaveGame(game);
        }
        //if we couldn't successfully connect to the cloud, runs while on device,
        //the same code that is in else statements in LoadData() and SaveData()
        else
        {
            if (!isSaving)
                LoadLocal();
            else
                SaveLocal();
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private void SaveGame(ISavedGameMetadata game)
    {
        string stringToSave = GameDataToJsonString();
        //saving also locally (can also call SaveLocal() instead)
        PlayerPrefs.SetString(SAVE_NAME, stringToSave);

        //encoding to byte array
        byte[] dataToSave = Encoding.ASCII.GetBytes(stringToSave);
        //updating metadata with new description
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            //.WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now);
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        //uploading data to the cloud
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updatedMetadata, dataToSave,
            OnSavedGameDataWritten);
    }

    //callback for ReadBinaryData
    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] savedData)
    {
        //if reading of the data was successful
        if (status == SavedGameRequestStatus.Success)
        {
            string cloudDataString;
            //if we've never played the game before, savedData will have length of 0
            if (savedData.Length == 0)
                //in such case, we want to assign default value to our string
                cloudDataString = string.Empty;
            //otherwise take the byte[] of data and encode it to string
            else
            {
                try
                {
                    cloudDataString = Encoding.ASCII.GetString(savedData);
                    Debug.Log("OnSavedGameDataRead - " + cloudDataString);
                }
                catch (Exception e)
                {
                    cloudDataString = string.Empty;
                }
                    
            }

            //getting local data (if we've never played before on this device, localData is already
            //string.Empty, so there's no need for checking as with cloudDataString)
            string localDataString = PlayerPrefs.GetString(SAVE_NAME);

            //this method will compare cloud and local data
            CompareStringToGameData(cloudDataString, localDataString);
        }
    }


    //callback for CommitUpdate
    private void OnSavedGameDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Game " + game.Description + " written");
        }
        else
        {
            Debug.LogWarning("Error saving game: " + status);
        }
    }
    #endregion /Saved Games

    #region Leaderboards
    public static void AddScoreToLeaderboard(string leaderboardId, long score)
    {
        Social.ReportScore(score, leaderboardId, success => { });
    }

    public static void ShowLeaderboardsUI()
    {
        Social.ShowLeaderboardUI();
    }
    #endregion /Leaderboards

    public void DeleteGameData()
    {
        // Open the file to get the metadata.
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(SAVE_NAME, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, DeleteSavedGame);
    }

    public void DeleteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.Delete(game);
        }
        else
        {
            // handle error
        }
    }
}
