using UnityEngine;
using UnityEngine.UI;

namespace LittleSpirit
{
    public class SkinView : MonoBehaviour
    {
        private ShopController shopController;
        public int skinNumber;
        public Image image;
        public Image imageBack;
        public Text skinName;
        public Text cost;
        // Use this for initialization
        void Start()
        {
            GameObject shopControllerObject = GameObject.FindWithTag("ShopController");
            if (shopControllerObject != null)
                shopController = shopControllerObject.GetComponent<ShopController>();
        }

        public void OnClick()
        {
            if (!shopController.skinsList[skinNumber].isBought)
            {
                if(CloudVariables.StarCoins >= shopController.skinsList[skinNumber].cost)
                    shopController.BuySkin(skinNumber);
                else
                    shopController.CantBuySkin(skinNumber);
            }
            else
            {
                shopController.SetSkin(skinNumber);
                imageBack.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
                
            }
            
        }
    }
}