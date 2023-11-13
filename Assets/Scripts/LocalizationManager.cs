using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {

    public static LocalizationManager instance;
    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    // Use this for initialization
    void Start() {
        if (Application.systemLanguage == SystemLanguage.Russian)
        {
            LoadLocalizedText("localizedText_ru.json");
        }
        else if (Application.systemLanguage == SystemLanguage.English)
        {
            LoadLocalizedText("localizedText_en.json");
        }
        else
        {
            LoadLocalizedText("localizedText_en.json");
        }
    }

    void Awake () 
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	public void LoadLocalizedText (string fileName) {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        //if (File.Exists(filePath))
        //{
            string dataAsJson = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW reader = new WWW(filePath);
                while (!reader.isDone) { }

                dataAsJson = reader.text;
            }
            else
            {
                dataAsJson = File.ReadAllText(filePath);
            }
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        //}
        ///else
        //{
           // missingTextString = "cannot find file"+ filePath;
           // Debug.Log("cannot find file");
        //}

        isReady = true;
    }

    public string GetLocalizedValue(string key)
    {

        string result = missingTextString; //gameObject.GetComponent<GUIText>().text;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;

    }

    public bool GetIsReady()
    {
        return isReady;
    }
}
