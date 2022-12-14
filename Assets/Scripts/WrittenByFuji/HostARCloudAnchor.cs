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
    private ARAnchorManager arAnchorManager = null;
    [HideInInspector] public ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchorHosted = null;
    private bool anchorHostInProgress = false;

    [SerializeField] private TextMeshProUGUI debugText,VRdebugText;

    [SerializeField] private UIEnableScript UIManager;

    [SerializeField] private LineRenderingTest wallLineScript;
    [SerializeField] private WallPointsNCMBScript pointsNCMBScript;

    [SerializeField] private ShareAura shareAura;
    [HideInInspector] public GameObject anchorObject, initialCircle;

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
    //ホスト
    public void HostAnchor()
    {
        //ボタンを押した瞬間にホストが始まる。結構調子いいけど正確さを保証できない。
        debugText.text = "アップロード中…\nオレンジの棒を色んな\n角度からうつそう。";

        //とっておいたアンカーをホスト、有効期限1日
        cloudAnchorHosted = ARAnchorManagerExtensions.HostCloudAnchor(arAnchorManager, pendingHostAnchor, 1);
        if(cloudAnchorHosted== null)
        {
            debugText.text = "アップロード失敗…\nアプリを再起動\nしてください。";
        }
        else
        {
            anchorHostInProgress = true;
        }
    }

    //ホスト作業の監督、成功したらロード用のIDを格納、失敗したらエラー表示
    public void CheckHostingProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchorHosted.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "アップロード成功!";//\nゲームスタートした後\nゴーグルをつけて\n戦いを始めよう。";
            anchorHostInProgress = false;
            //LineRendererを持つオブジェクトの位置をクラウドアンカーに合わせる
            //lineRendererTransform.position = cloudAnchorHosted.transform.position;
            //NCMBに呼び出し用のIDをアップロード
            resolveIDClass["ResolveID"] = cloudAnchorHosted.cloudAnchorId;
            resolveIDClass.SaveAsync();
            //気配共有の基準にすべくPosition,Rotationを渡し、オーラ発生オブジェクトの親をアンカーと同じ位置、角度にする
            shareAura.cloudAnchorPos = cloudAnchorHosted.transform.position;
            shareAura.cloudAnchorRot = cloudAnchorHosted.transform.rotation.eulerAngles;
            shareAura.auraGenerator.position = cloudAnchorHosted.transform.position;
            shareAura.auraGenerator.rotation = cloudAnchorHosted.transform.rotation;
            shareAura.SetDebugAxis(cloudAnchorHosted.transform.position, cloudAnchorHosted.transform.rotation.eulerAngles);
            UIManager.SetRenderLineState();
            //視界の左下のテキスト
            VRdebugText.text = $"X:{cloudAnchorHosted.transform.rotation.x}\nY:{cloudAnchorHosted.transform.rotation.y}\nZ:{cloudAnchorHosted.transform.rotation.z}";
            if (anchorObject!= null && initialCircle != null)
            {
                anchorObject.SetActive(false);
                initialCircle.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"エラー発生: {cloudAnchorState}";
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
        //アンカーをアップしていない、または現在のLineRenderer位置V3と現在のアンカー位置V3が10cm未満ならreturn
        if (cloudAnchorHosted == null || Vector3.Distance(wallLineScript.uploadBasePosition, cloudAnchorHosted.transform.position) < .1f)
        {
            return;
        }
        //そうでないならLineRendererのオブジェクトの位置をクラウドアンカーに合わせる
        else
        {
            wallLineScript.uploadBasePosition = cloudAnchorHosted.transform.position;
        }
    }
}
