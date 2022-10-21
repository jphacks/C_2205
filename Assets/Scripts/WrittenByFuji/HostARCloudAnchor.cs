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
        //NCMB上のIDの場所を特定
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
    //ホスト
    public void QueueAnchor(ARAnchor anchor)
    {
        pendingHostAnchor = anchor;
    }
    public void HostAnchor()
    {
        //ボタンを押した瞬間にホストが始まる。結構調子いいけど正確さを保証できない。
        debugText.text = "HostAnchor call in progress";

        //とっておいたアンカーをホスト、有効期限1日
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

    //ホスト作業の監督、成功したらロード用のIDを格納、失敗したらエラー表示
    public void CheckHostingProgress()
    {
        //ホスト前に30秒程のスキャンが推奨される。
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
            //NCMBに呼び出し用のIDをアップロード
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
        //ホストチェック
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }
    }
}
