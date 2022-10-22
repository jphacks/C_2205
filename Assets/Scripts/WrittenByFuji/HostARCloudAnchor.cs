using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using NCMB;

public class HostARCloudAnchor : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Camera arCamera = null;
    private ARAnchorManager arAnchorManager = null;
    [HideInInspector] public ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchorHosted = null;
    private bool anchorHostInProgress = false;

    [SerializeField] private TextMeshProUGUI debugText;

    [SerializeField] private WallPointsNCMBScript pointsNCMBScript;

    [SerializeField] private SwitchToVR switchToVR;
    [HideInInspector] public GameObject anchorObject, initialCircle;

    NCMBObject resolveIDClass;
    private void Awake()
    {
        //NCMB���ID�̏ꏊ�����
        resolveIDClass = new NCMBObject("ResolveAnchorID");
        resolveIDClass.ObjectId = "vF64xh7DPtrds3ol";
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(HostAnchor);
        }
        arAnchorManager = GetComponent<ARAnchorManager>();
    }
    //�z�X�g
    public void HostAnchor()
    {
        //�{�^�����������u�ԂɃz�X�g���n�܂�B���\���q�������ǐ��m����ۏ؂ł��Ȃ��B
        debugText.text = "�A�b�v���[�h���c\n�I�����W�̖_��F���\n�p�x���炤�����B";

        //�Ƃ��Ă������A���J�[���z�X�g�A�L������1��
        cloudAnchorHosted = ARAnchorManagerExtensions.HostCloudAnchor(arAnchorManager, pendingHostAnchor, 1);
        if(cloudAnchorHosted== null)
        {
            debugText.text = "�A�b�v���[�h���s�c\n�A�v�����ċN��\n���Ă��������B";
        }
        else
        {
            anchorHostInProgress = true;
        }
    }

    //�z�X�g��Ƃ̊ēA���������烍�[�h�p��ID���i�[�A���s������G���[�\��
    public void CheckHostingProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchorHosted.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "�A�b�v���[�h����!\n�Q�[���X�^�[�g������\n�S�[�O��������\n�킢���n�߂悤�B";
            anchorHostInProgress = false;
            //NCMB�ɕǍ��W�f�[�^�𑗐M
            pointsNCMBScript.HostPointsData(cloudAnchorHosted.transform.position);
            //NCMB�ɌĂяo���p��ID���A�b�v���[�h
            resolveIDClass["ResolveID"] = cloudAnchorHosted.cloudAnchorId;
            resolveIDClass.SaveAsync();
            if(anchorObject!= null && initialCircle != null)
            {
                anchorObject.SetActive(false);
                initialCircle.SetActive(false);
            }

            switchToVR.switchToVRButton.gameObject.SetActive(true);
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"�G���[����: {cloudAnchorState}";
            anchorHostInProgress = false;
        }
    }
    // Update is called once per frame
    void Update()
    {   
        //�z�X�g�`�F�b�N
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }
    }
}
