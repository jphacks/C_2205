using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class AnchorCreatedEvent : UnityEvent<Transform> { }

public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera = null;
    [SerializeField] private float resolveAnchorPassedTimeout = 5.0f;
    private ARAnchorManager arAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchor = null;
    private PlacementController placementController = null;
    private string anchorToResolve;
    private bool anchorUpdateInProgress = false, anchorResolveInProgress = false;
    private float safeToResolvePassed = 0;
    private AnchorCreatedEvent anchorCreatedEvent = null;

    private void Awake()
    {
        anchorCreatedEvent = new AnchorCreatedEvent();
        //anchorCreatedEvent.AddListener((t) => placementController.ReCreatePlacement(t));
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

    }
    public void Resolve()
    {

    }
    public void CheckHostingProgress()
    {

    }
    public void CheckResolveProgress()
    {

    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    }
}
