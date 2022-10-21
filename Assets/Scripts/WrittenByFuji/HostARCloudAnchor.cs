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

    [SerializeField] private TextMeshProUGUI debugText;//,scanQuality;

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
    /*
    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position, arCamera.transform.rotation);
    }
    */
    //�z�X�g
    public void QueueAnchor(ARAnchor anchor)
    {
        pendingHostAnchor = anchor;
    }
    public void HostAnchor()
    {
        //�{�^�����������u�ԂɃz�X�g���n�܂�B���\���q�������ǐ��m����ۏ؂ł��Ȃ��B
        debugText.text = "HostAnchor call in progress";

        //�Ƃ��Ă������A���J�[���z�X�g�A�L������1��
        cloudAnchorHosted = ARAnchorManagerExtensions.HostCloudAnchor(arAnchorManager, pendingHostAnchor, 1);
        if(cloudAnchorHosted== null)
        {
            debugText.text = "Unable to host cloud anchor";
        }
        else
        {
            anchorHostInProgress = true;
        }
    }

    //�z�X�g��Ƃ̊ēA���������烍�[�h�p��ID���i�[�A���s������G���[�\��
    public void CheckHostingProgress()
    {
        //�z�X�g�O��30�b���̃X�L���������������B
        //FeatureMapQuality quality = ARAnchorManagerExtensions.EstimateFeatureMapQualityForHosting(arAnchorManager, GetCameraPose());
        //scanQuality.text = quality.ToString();

        CloudAnchorState cloudAnchorState = cloudAnchorHosted.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "Host Success!" +
                "             \nPosition:" + cloudAnchorHosted.transform.position
                           + "\nRotation:" + cloudAnchorHosted.transform.rotation
                           + "\nResolveID:" + cloudAnchorHosted.cloudAnchorId;
            anchorHostInProgress = false;
            //NCMB�ɌĂяo���p��ID���A�b�v���[�h
            resolveIDClass["ResolveID"] = cloudAnchorHosted.cloudAnchorId;
            resolveIDClass.SaveAsync();
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"Error while hosting: {cloudAnchorState}";
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
