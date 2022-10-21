using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToVR : MonoBehaviour
{
    [SerializeField] private Button switchToVRButton;
    [SerializeField] private GameObject vrFunctions, arFunctions;
    private bool vrOnGoing;
    private void Awake()
    {
        if(switchToVRButton != null)
        {
            switchToVRButton.onClick.AddListener(Switch);
        }
        vrOnGoing = false;
        arFunctions.SetActive(true);
        vrFunctions.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && vrOnGoing)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                //【デバッグ】シーンリロード
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
    }
    private void Switch()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        arFunctions.SetActive(false);
        vrFunctions.SetActive(true);
        vrOnGoing = true;
        vrFunctions.transform.position = arFunctions.transform.position;
        vrFunctions.transform.rotation = arFunctions.transform.rotation;
    }
}
