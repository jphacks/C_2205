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
    [SerializeField] private TextMeshProUGUI debugText,VRdebugText;
    NCMBObject resolveIDClass;
    [SerializeField]
    private WallPointsNCMBScript pointsNCMBScript;
    [SerializeField] private AllPlayerPositionInfoForKehai allPlayerPositionInfoForKehai;
    [SerializeField] private SwitchToVR switchToVR;
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
        debugText.text = "呼び出し中…";
        resolveIDClass.FetchAsync((NCMBException e) =>
        {
            if(e != null)
            {
                debugText.text = "エラー発生:" + e;
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
            debugText.text = "呼び出し成功!\nゲームスタートした後\nゴーグルをつけて\n戦いを始めよう。";
            anchorResolveInProgress = false;
            resolvedObject = Instantiate(resolveObject, cloudAnchorResolved.transform).transform;
            // NCMBから壁座標を取得してlineRendererに反映
            pointsNCMBScript.ReceiveWallPoints(resolvedObject.position, cloudAnchorResolved.transform.eulerAngles);
            //気配共有の基準にすべくPosition,Rotationを渡し、オーラ発生オブジェクトの親をアンカーと同じ位置、角度にする
            allPlayerPositionInfoForKehai.cloudAnchorPos = cloudAnchorResolved.transform.position;
            allPlayerPositionInfoForKehai.cloudAnchorRot = cloudAnchorResolved.transform.rotation.eulerAngles;
            allPlayerPositionInfoForKehai.auraGenerator.position = cloudAnchorResolved.transform.position;
            allPlayerPositionInfoForKehai.auraGenerator.rotation = cloudAnchorResolved.transform.rotation;
            switchToVR.switchToVRButton.gameObject.SetActive(true);
            //視界の左下のテキスト
            VRdebugText.text = $"X:{cloudAnchorResolved.transform.position.x}\nY:{cloudAnchorResolved.transform.position.y}\nZ:{cloudAnchorResolved.transform.position.z}";
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"呼び出し中の\nエラー: {cloudAnchorState}";
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
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
}
