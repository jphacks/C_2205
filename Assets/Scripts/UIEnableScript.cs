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

    //Inspector�ɕ\�������
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
        instructionText.text = "�K�v�Ȃ�A���ƍ���\n�悤�ɕ��ʂ̍�����\n�������悤�B";
        ChangeUIState(UIState.AdjustPlane);
    }

    public void SetRenderLineState()
    {
        instructionText.text = "�V�Ԕ͈͂��͂ނ悤��\n�_��ݒu���A�Z�[�t\n�G���A����낤�B";
        ChangeUIState(UIState.RenderLine);
    }

    public void SetShareAnchorState()
    {
        instructionText.text = "�Z�[�t�G���A��\n�A�b�v���[�h���悤�B";
        ChangeUIState(UIState.ShareAnchor);
    }
}

//Inspector�ɕ����f�[�^��\�����邽�߂̃N���X
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
