using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModalPanel : MonoBehaviour {

    public GameObject modalPanelObject;
    private static ModalPanel modalPanel;

    public Text question;
    public Image iconImage;
    public Button yesButton;
    public Button noButton;
    public Button OkButton;

    public static ModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(int skin_number, string question, UnityAction yesEvent, UnityAction noEvent)
    {
        modalPanelObject.SetActive(true);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        OkButton.gameObject.SetActive(false);
        if (yesEvent != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(yesEvent);
            yesButton.onClick.AddListener(ClosePanel);
            yesButton.gameObject.SetActive(true);

            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(noEvent);
            noButton.onClick.AddListener(ClosePanel);
            noButton.gameObject.SetActive(true);
        }
        else
        {
            OkButton.onClick.RemoveAllListeners();
            OkButton.onClick.AddListener(noEvent);
            OkButton.onClick.AddListener(ClosePanel);
            OkButton.gameObject.SetActive(true);
        }

        this.iconImage.sprite = Resources.Load<Sprite>("Spirit_skins/spirit_skin_" + skin_number.ToString());
        this.question.text = LocalizationManager.instance.GetLocalizedValue(question);

        //this.iconImage.gameObject.SetActive(false);
        //yesButton.gameObject.SetActive(true);
        
    }

    public void ClosePanel()
    {
        modalPanelObject.SetActive(false);
    }
}
