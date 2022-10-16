using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class cloudAnchorCreatedEvent : UnityEvent<Transform> { }

public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera = null;
    [SerializeField] private float resolveAnchorPassedTimeout = 5.0f;
    private ARAnchorManager arAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchor = null;
    private CreateBasePlane createBasePlane = null;
    private string anchorIDtoResolve;
    private bool anchorHostInProgress = false, anchorResolveInProgress = false;
    private float safeToResolvePassed = 0;
    private cloudAnchorCreatedEvent cloudAnchorCreatedEvent = null;

    //ホスト前に30秒程のスキャンが推奨される。
    FeatureMapQuality quality;
    [SerializeField] private TextMeshProUGUI debugText,scanQuality;

    private void Awake()
    {
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
        debugText.text = "HostAnchor call in progress";
        quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        //とっておいたアンカーをホスト、有効期限1日
        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 1);
        if(cloudAnchor== null)
        {
            debugText.text = "Unable to host cloud anchor";
        }
        else
        {
            anchorHostInProgress = true;
        }
    }
    public void Resolve()
    {
        debugText.text = "Resolve call in progress";
        cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorIDtoResolve);

        if (cloudAnchor == null)
        {
            debugText.text = $"Unable to resolve cloud anchor:{anchorIDtoResolve}";
        }
        else
        {
            anchorResolveInProgress = true;
        }
    }
    public void CheckHostingProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            anchorHostInProgress = false;
            anchorIDtoResolve = cloudAnchor.cloudAnchorId;
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"Error while hosting: {cloudAnchorState}";
            anchorHostInProgress = false;
        }
    }
    public void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            anchorResolveInProgress = false;
            cloudAnchorCreatedEvent?.Invoke(cloudAnchor.transform);
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
        scanQuality.text = $"Feature Map Quality: {quality}";

        //ホストチェック
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }

        //ロードチェック
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
}
