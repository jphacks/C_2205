using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    private AsyncOperation hostScene;//, guestScene;
    [SerializeField] private Button buttonForHost, buttonForGuest;
    [SerializeField] private TMPro.TextMeshProUGUI startText;

    private void Awake()
    {
        if(buttonForHost != null && buttonForGuest != null)
        {
            buttonForHost.onClick.AddListener(ButtonForHost);
            buttonForGuest.onClick.AddListener(ButtonForGuest);
        }
        hostScene = SceneManager.LoadSceneAsync("Host");
        hostScene.allowSceneActivation = false;
        //guestScene = SceneManager.LoadSceneAsync("Guest");
        //guestScene.allowSceneActivation = false;
        buttonForHost.gameObject.SetActive(false);
        buttonForGuest.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        if (string.IsNullOrEmpty(startText.text))
        {
            return;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    buttonForHost.gameObject.SetActive(true);
                    buttonForGuest.gameObject.SetActive(true);
                    startText.text = "";
                }
            }
        }
    }
    private void ButtonForHost()
    {
        hostScene.allowSceneActivation = true;
    }
    private void ButtonForGuest()
    {
        //guestScene.allowSceneActivation = true;
    }
}
