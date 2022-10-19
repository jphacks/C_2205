using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using NCMB;

public class ResolveARCloudAnchor : MonoBehaviour
{
    [SerializeField] private Button resolveButton;
    private ARAnchorManager arAnchorManager = null;
    private ARCloudAnchor cloudAnchorResolved = null;
    private float safeToResolvePassed = 0;
    private float resolveAnchorPassedTimeout = 5.0f;
    private string anchorIDtoResolve;
    private bool anchorResolveInProgress = false;
    [SerializeField] private GameObject resolveObject;
    private Transform resolvedObject;
    [SerializeField] private TextMeshProUGUI debugText;
    NCMBObject resolveIDClass;
    private void Awake()
    {
        //NCMB上のIDの場所を特定
        resolveIDClass = new NCMBObject("ResolveAnchorID");
        resolveIDClass.ObjectId = "vF64xh7DPtrds3ol";
        if (resolveButton != null)
        {
            resolveButton.onClick.AddListener(ResolveAnchor);
        }
        arAnchorManager = GetComponent<ARAnchorManager>();
    }
    //呼び出し
    public void ResolveAnchor()
    {
        debugText.text = "Resolve call in progress";
        resolveIDClass.FetchAsync((NCMBException e) =>
        {
            if(e != null)
            {
                debugText.text = "NCMB Error:" + e;
            }
            else
            {
                cloudAnchorResolved = ARAnchorManagerExtensions.ResolveCloudAnchorId(arAnchorManager, resolveIDClass["ResolveID"].ToString());
                if (cloudAnchorResolved == null)
                {
                    debugText.text = $"Unable to resolve cloud anchor:{resolveIDClass["ResolveID"]}";
                }
                else
                {
                    //debugText.text = $"Resolving cloud anchor with ID:{resolveIDClass["ResolveID"]}";
                    anchorResolveInProgress = true;
                }
            }
        });
        
    }
    //呼び出し作業の監督
    public void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchorResolved.cloudAnchorState;
        //成功したらその場所に生成
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
    // Update is called once per frame
    void Update()
    {
        //ロードチェック
        if (anchorResolveInProgress && safeToResolvePassed <= 0)
        {
            safeToResolvePassed = resolveAnchorPassedTimeout;
            if (!string.IsNullOrEmpty(resolveIDClass["ResolveID"].ToString()))
            {
                debugText.text = $"Resolving Anchor Id: {resolveIDClass["ResolveID"]}";
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
}
