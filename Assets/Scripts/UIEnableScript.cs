using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIEnableScript : MonoBehaviour
{
    public enum UIState
    {
        DetectPlane,
        AdjustPlane,
        RenderLine
    }

    //Inspectorに複数データを表示するためのクラス
    [System.SerializableAttribute]
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
    //Inspectorに表示される
    [SerializeField]
    private List<EnableUIList> _UIListList = new List<EnableUIList>();

    // Start is called before the first frame update
    void Start()
    {
        ChangeUIState(UIState.DetectPlane);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeUIState(UIState next)
    {
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
    }

    public void FinishDetectionButton()
    {
        ChangeUIState(UIState.RenderLine);
    }

    public void SetBasePlaneButton()
    {
        ChangeUIState(UIState.AdjustPlane);
    }
}
