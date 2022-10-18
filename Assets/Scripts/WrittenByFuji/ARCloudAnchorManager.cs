using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField] private Button hostButton, resolveButton,forwardButton,backButton,rightButton,leftButton;
    [SerializeField] private Camera arCamera = null;
    [SerializeField] private float resolveAnchorPassedTimeout = 5.0f;
    private ARAnchorManager arAnchorManager = null;

    [HideInInspector] public ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchorHosted = null,cloudAnchorResolved = null;
    private CreateBasePlane createBasePlane = null;
    private string anchorIDtoResolve;
    private bool anchorHostInProgress = false, anchorResolveInProgress = false;
    private float safeToResolvePassed = 0;

    [SerializeField] private GameObject resolveObject;
    private Transform resolvedObject;
    
    [SerializeField] private TextMeshProUGUI debugText,scanQuality;

    private void Awake()
    {
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(HostAnchor);
        }
        if (resolveButton != null)
        {
            resolveButton.onClick.AddListener(Resolve);
        }
        if (forwardButton != null && backButton != null && rightButton != null && backButton != null)
        {
            forwardButton.onClick.AddListener(() => AdjustResolve(Vector3.forward));
            backButton.onClick.AddListener(() => AdjustResolve(Vector3.back));
            rightButton.onClick.AddListener(() => AdjustResolve(Vector3.right));
            leftButton.onClick.AddListener(() => AdjustResolve(Vector3.left));
        }
        arAnchorManager = GetComponent<ARAnchorManager>();
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position, arCamera.transform.rotation);
    }

    #region Cloud Anchor Cycle
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
        FeatureMapQuality quality = ARAnchorManagerExtensions.EstimateFeatureMapQualityForHosting(arAnchorManager, GetCameraPose());
        scanQuality.text = quality.ToString();

        CloudAnchorState cloudAnchorState = cloudAnchorHosted.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "Host Success!\nPosition:" + cloudAnchorHosted.pose.position + "\nRotation:" + cloudAnchorHosted.pose.rotation;
            anchorHostInProgress = false;
            anchorIDtoResolve = cloudAnchorHosted.cloudAnchorId;
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"Error while hosting: {cloudAnchorState}";
            anchorHostInProgress = false;
        }
    }
    public void Resolve()
    {
        debugText.text = "Resolve call in progress";
        cloudAnchorResolved = ARAnchorManagerExtensions.ResolveCloudAnchorId(arAnchorManager, anchorIDtoResolve);

        if (cloudAnchorResolved == null)
        {
            debugText.text = $"Unable to resolve cloud anchor:{anchorIDtoResolve}";
        }
        else
        {
            anchorResolveInProgress = true;
        }
    }

    //���[�h��Ƃ̊ē�
    public void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchorResolved.cloudAnchorState;
        //���������炻�̏ꏊ�ɐ���
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "Resolve Success!\nPosition: " + cloudAnchorResolved.pose.position + "\nRotation: " + cloudAnchorResolved.pose.rotation;
            anchorResolveInProgress = false;
            resolvedObject = Instantiate(resolveObject, cloudAnchorResolved.transform).transform;
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"Error while resolving: {cloudAnchorState}";
            anchorResolveInProgress = false;
        }
    }
    #endregion
    // Update is called once per frame
    void Update()
    {   
        //�z�X�g�`�F�b�N
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }

        //���[�h�`�F�b�N
        if(anchorResolveInProgress && safeToResolvePassed <= 0)
        {
            safeToResolvePassed = resolveAnchorPassedTimeout;
            if (!string.IsNullOrEmpty(anchorIDtoResolve))
            {
                debugText.text = $"Resolving Anchor Id: {anchorIDtoResolve}";
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }

    private void AdjustResolve(Vector3 direction)
    {
        resolvedObject.Translate(direction * 0.1f);
    }
}
