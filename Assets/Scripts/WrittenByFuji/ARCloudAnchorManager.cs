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
    private PlacementController placementController = null;
    private string anchorIDtoResolve;
    private bool anchorHostInProgress = false, anchorResolveInProgress = false;
    private float safeToResolvePassed = 0;
    private cloudAnchorCreatedEvent cloudAnchorCreatedEvent = null;

    [SerializeField] private TextMeshProUGUI DebugText;

    private void Awake()
    {
        cloudAnchorCreatedEvent = new cloudAnchorCreatedEvent();
        cloudAnchorCreatedEvent.AddListener((t) => placementController.ReCreatePlacement(t));
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
        DebugText.text = "HostAnchor call in progress";

        //ホスト前に30秒程のスキャンが推奨される。
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        DebugText.text = $"Feature Map Quality: {quality}";

        //とっておいたアンカーをホスト、有効期限1日
        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 1);
        if(cloudAnchor== null)
        {
            DebugText.text = "Unable to host cloud anchor";
        }
        else
        {
            anchorHostInProgress = true;
        }
    }
    public void Resolve()
    {
        DebugText.text = "Resolve call in progress";
        cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorIDtoResolve);

        if (cloudAnchor == null)
        {
            DebugText.text = $"Unable to resolve cloud anchor:{anchorIDtoResolve}";
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
            DebugText.text = $"Error while hosting: {cloudAnchorState}";
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
            DebugText.text = $"Error while resolving: {cloudAnchorState}";
            anchorResolveInProgress = false;
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
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
                DebugText.text = $"Resolving Anchor Id: {anchorIDtoResolve}";
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
}
