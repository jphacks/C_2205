using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UIEnableScript;

using TMPro;

public class UIEnableScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText;
    public enum UIState
    {
        DetectPlane,
        AdjustPlane,
        RenderLine,
        ShareAnchor,
    }

    //Inspectorに表示される
    [SerializeField]
    private List<EnableUIList> _UIListList = new List<EnableUIList>();

    // Start is called before the first frame update
    void Start()
    {
        ChangeUIState(UIState.DetectPlane);
    }


    public void ChangeUIState(UIState next)
    {
        var disableObjects = _UIListList.Where(_ => _.uIState != next)
.Select(_ => _.List).ToArray();
        foreach (var objs in disableObjects)
        {
            foreach (var obj in objs)
            {
                Debug.Log(obj.transform.name);
                obj.SetActive(false);
            }
        }
        var uiObjects = _UIListList.Where(_ => _.uIState == next)
        .Select(_ => _.List).ToArray();
        foreach (var objs in uiObjects)
        {
            foreach (var obj in objs)
            {
                Debug.Log(obj.transform.name);
                obj.SetActive(true);
            }
        }
    }

    public void SetDetectPlaneState()
    {
        ChangeUIState(UIState.DetectPlane);
    }

    public void SetAdjustPlaneState()
    {
        instructionText.text = "必要なら、床と合う\nように平面の高さを\n調整しよう。";
        ChangeUIState(UIState.AdjustPlane);
    }

    public void SetRenderLineState()
    {
        instructionText.text = "遊ぶ範囲を囲むように\n点を設置し、セーフ\nエリアを作ろう。";
        ChangeUIState(UIState.RenderLine);
    }

    public void SetShareAnchorState()
    {
        instructionText.text = "セーフエリアを\nアップロードしよう。";
        ChangeUIState(UIState.ShareAnchor);
    }
}

//Inspectorに複数データを表示するためのクラス
[System.Serializable]
public class EnableUIList
{
    public UIState uIState;
    public List<GameObject> List = new List<GameObject>();

    public EnableUIList(UIState uI, List<GameObject> list)
    {
        uIState = uI;
        List = list;
    }
}
