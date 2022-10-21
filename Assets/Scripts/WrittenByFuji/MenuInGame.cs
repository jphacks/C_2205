using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuInGame : MonoBehaviour
{
    [SerializeField] private Button confirmButton, cancelButton;
    [SerializeField] private Image confirmScreen;
    private AsyncOperation titleScene;

    private void Awake()
    {
        if(confirmButton != null && cancelButton != null)
        {
            confirmButton.onClick.AddListener(Confirm);
            cancelButton.onClick.AddListener(Cancel);
        }
        confirmScreen.rectTransform.localScale = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        //‘JˆÚæ‚ªˆê‚Â‚µ‚©‚È‚¢‚Ì‚ÅƒvƒŒƒ[ƒh
        titleScene = SceneManager.LoadSceneAsync("Title");
        titleScene.priority = 2;
        titleScene.allowSceneActivation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && confirmScreen.rectTransform.localScale == Vector3.zero)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                confirmScreen.rectTransform.localScale = Vector3.one;
            }
        }
    }
    private void Confirm()
    {
        titleScene.allowSceneActivation = true;
    }
    private void Cancel()
    {
        confirmScreen.rectTransform.localScale = Vector3.zero;
    }
}
