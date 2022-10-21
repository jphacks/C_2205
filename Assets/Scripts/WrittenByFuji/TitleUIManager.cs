using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private Button buttonForHost, buttonForGuest;
    [SerializeField] private TMPro.TextMeshProUGUI startText;

    private void Awake()
    {
        if(buttonForHost != null && buttonForGuest != null)
        {
            buttonForHost.onClick.AddListener(ButtonForHost);
            buttonForGuest.onClick.AddListener(ButtonForGuest);
        }
        buttonForHost.gameObject.SetActive(false);
        buttonForGuest.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
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
        SceneManager.LoadScene("Host");
    }
    private void ButtonForGuest()
    {
        SceneManager.LoadScene("Guest");
    }
}
