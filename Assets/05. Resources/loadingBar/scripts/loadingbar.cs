using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class loadingbar : MonoBehaviour {

    private RectTransform rectComponent;
    private Image imageComp;

    public GameObject loginPanel;
    public GameObject roomPanel;

    public GameObject me;

    // Use this for initialization
    void Start ()
    {
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
        imageComp.fillAmount = 0.0f;
    }

    void Update()
    {
        if(loginPanel.activeInHierarchy)
        {
            if (imageComp.fillAmount != 1f)
            {
                imageComp.fillAmount = imageComp.fillAmount + Time.deltaTime * 0.1f;
            }

            if(Photonmanager.instance.userState == Photonmanager.UserState.Master)
            {
                imageComp.fillAmount = imageComp.fillAmount + Time.deltaTime * 1f;
                if (imageComp.fillAmount == 1)
                {
                    Photonmanager.instance.OnLoadCompleate_EnterLobby();
                    me.SetActive(false);
                    imageComp.fillAmount = 0.0f;
                }
            }

            if (Photonmanager.instance.userState == Photonmanager.UserState.Room)
            {
                imageComp.fillAmount = imageComp.fillAmount + Time.deltaTime * 1f;
                if (imageComp.fillAmount == 1)
                {
                    Photonmanager.instance.OnLoadComplete_EnterRoom();
                    me.SetActive(false);
                    imageComp.fillAmount = 0.0f;
                }
            }
        }

        if (roomPanel.activeInHierarchy)
        {
            if (imageComp.fillAmount != 1f)
            {
                imageComp.fillAmount = imageComp.fillAmount + Time.deltaTime * 0.4f;
            }

            if (imageComp.fillAmount == 1)
            {
                Photonmanager.instance.OnLoadComplete_GameStart();
                me.SetActive(false);
                imageComp.fillAmount = 0.0f;
            }
        }
    }
}
