using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToVR : MonoBehaviour
{
    public Button switchToVRButton;
    [SerializeField] private GameObject[] deactivateOnVRstart;
    [SerializeField] private GameObject vrFunctions, arFunctions;
    
    private void Awake()
    {
        if(switchToVRButton != null)
        {
            switchToVRButton.onClick.AddListener(Switch);
        }
        arFunctions.SetActive(true);
        vrFunctions.SetActive(false);
        switchToVRButton.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void Switch()
    {
        switchToVRButton.gameObject.SetActive(false);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        arFunctions.SetActive(false);
        for (int i = 0; i < deactivateOnVRstart.Length; i++)
        {
            deactivateOnVRstart[i].SetActive(false);
        }
        vrFunctions.SetActive(true);
        vrFunctions.transform.position = arFunctions.transform.position;
        vrFunctions.transform.rotation = arFunctions.transform.rotation;
    }
}
