using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopController : MonoBehaviour {

    private GameController gameController;

    private ModalPanel modalPanel;
    private DisplayManager displayManager;

    private UnityAction myYesAction;
    private UnityAction myNoAction;

    public int buySkinNumber;
    public int SkinsCount;
    public List<SkinObject> skinsList;
    public VerticalLayoutGroup ListView;
    public GameObject shopItem;
    // Use this for initialization
    void Start () {
        modalPanel = ModalPanel.Instance();
        displayManager = DisplayManager.Instance();

        myYesAction = new UnityAction(TestYesFunction);
        myNoAction = new UnityAction(TestNoFunction);

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
            gameController = gameControllerObject.GetComponent<GameController>();

        skinsList = new List<SkinObject>();

        for (int i = 0; i < SkinsCount; i++)
        {
            skinsList.Add(Resources.Load<SkinObject>("Spirit_skins/Skin_" + i.ToString()));
            skinsList[i].isBought = false;
            shopItem.transform.GetChild(5).GetComponent<LittleSpirit.SkinView>().skinNumber = i;
            shopItem.transform.GetChild(0).GetComponent<Image>().color = new Color32(20, 40, 60, 100);
            shopItem.transform.GetChild(3).gameObject.SetActive(true);
            shopItem.transform.GetChild(5).GetComponentInChildren<LocalizedText>().key = "buy_skin";

            shopItem.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Spirit_skins/spirit_skin_" + i.ToString());
            shopItem.transform.GetChild(2).GetComponent <Text>().text = LocalizationManager.instance.GetLocalizedValue(skinsList[i].skinName);
            shopItem.transform.GetChild(4).GetComponent<Text>().text = skinsList[i].cost.ToString();

            for (int j = 0; j < CloudVariables.Skins.Count; j++)
            {
                if (i == CloudVariables.Skins[j])
                {
                    skinsList[i].isBought = true;
                    
                    shopItem.transform.GetChild(3).gameObject.SetActive(false);
                    shopItem.transform.GetChild(4).GetComponent<Text>().text = "";
                    shopItem.transform.GetChild(5).GetComponentInChildren<LocalizedText>().key = "select_skin";
                    break;
                }

            }
            if (CloudVariables.CurrentSkin == i) //Текущий скин
            {
                shopItem.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 225, 100);
                shopItem.transform.GetChild(5).GetComponentInChildren<LocalizedText>().key = "current_skin";
            }

            GameObject GO = Instantiate(shopItem);
            //GO.transform.GetChild(5).GetComponentInChildren<Text>().text = "Bought"; = "Select";
            GO.transform.parent = ListView.transform;
            GO.transform.localScale = new Vector3(1, 1, 1);
        }
	}

    public void BuySkin(int skinNumber)
    {
        buySkinNumber = skinNumber;
        modalPanel.Choice(skinNumber, "do_buy_skin", TestYesFunction, TestNoFunction);
    }

    public void CantBuySkin(int skinNumber)
    {
        buySkinNumber = skinNumber;
        modalPanel.Choice(skinNumber, "cant_buy_skin", null, TestNoFunction);
    }

    public void SetSkin(int skinNumber)
    {
        for (int i = 0; i < SkinsCount; i++)
        {
            for (int j = 0; j < CloudVariables.Skins.Count; j++)
            {
                if (i == CloudVariables.Skins[j])
                {
                    ListView.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color32(20, 40, 60, 100);
                    ListView.transform.GetChild(i).transform.GetChild(3).gameObject.SetActive(false);
                    ListView.transform.GetChild(i).transform.GetChild(4).GetComponent<Text>().text = "";
                    ListView.transform.GetChild(i).transform.GetChild(5).GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalizedValue("select_skin");
                    break;
                }

            }
        }
        ListView.transform.GetChild(skinNumber).transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        ListView.transform.GetChild(skinNumber).transform.GetChild(5).GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalizedValue("current_skin");
        gameController.setSkin(skinNumber);
    }

    //  These are wrapped into UnityActions
    void TestYesFunction()
    {
        CloudVariables.Skins.Add(buySkinNumber);
        CloudVariables.StarCoins = CloudVariables.StarCoins - skinsList[buySkinNumber].cost;
        SetSkin(buySkinNumber);
        ListView.transform.GetChild(buySkinNumber).gameObject.SetActive(true);
        skinsList[buySkinNumber].isBought = true;
    }

    void TestNoFunction()
    {
        modalPanel.ClosePanel();
    }
}
