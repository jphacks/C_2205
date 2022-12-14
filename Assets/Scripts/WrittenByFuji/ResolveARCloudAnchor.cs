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
    private bool anchorResolveInProgress = false;
    [SerializeField] private GameObject resolveObject;
    [SerializeField] private TextMeshProUGUI debugText, VRdebugText;
    NCMBObject resolveIDClass;
    [SerializeField]
    private WallPointsNCMBScript pointsNCMBScript;
    [SerializeField] private ShareAura shareAura;
    [SerializeField] private SwitchToVR switchToVR;
    [SerializeField] private Transform lineRendererTransform, DEBUGlineRenderer;
    private Transform cloudAnchorTransform = null;
    private Vector3 anchorPosTemp = Vector3.zero;
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
            if (e != null)
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
    IEnumerator CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchorResolved.cloudAnchorState;
        //成功したらその場所に生成
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "呼び出し成功!\nゲームスタートした後\nゴーグルをつけて\n戦いを始めよう。";
            anchorResolveInProgress = false;
            cloudAnchorTransform = Instantiate(resolveObject, cloudAnchorResolved.transform.position, cloudAnchorResolved.transform.rotation).transform;
            yield return null;
            // NCMBから壁座標を取得してlineRendererに反映
            pointsNCMBScript.ReceiveWallPoints(cloudAnchorTransform.position, cloudAnchorTransform.eulerAngles);
            anchorPosTemp = cloudAnchorTransform.position;
            yield return null;
            //InvokeRepeating("AdjustPosition", 0, 5);

            //気配共有の基準にすべくPosition,Rotationを渡し、オーラ発生オブジェクトの親をアンカーと同じ位置、角度にする
            shareAura.cloudAnchorPos = cloudAnchorTransform.position;
            shareAura.cloudAnchorRot = cloudAnchorTransform.rotation.eulerAngles;
            shareAura.auraGenerator.position = cloudAnchorTransform.position;
            shareAura.auraGenerator.rotation = cloudAnchorTransform.rotation;
            shareAura.SetDebugAxis(cloudAnchorTransform.position, cloudAnchorTransform.rotation.eulerAngles);
            switchToVR.switchToVRButton.gameObject.SetActive(true);
            //視界の左下のテキスト
            VRdebugText.text = $"X:{cloudAnchorResolved.transform.rotation.x}\nY:{cloudAnchorResolved.transform.rotation.y}\nZ:{cloudAnchorResolved.transform.rotation.z}";
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"呼び出し中の\nエラー: {cloudAnchorState}";
            anchorResolveInProgress = false;
        }
        yield return null;
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
                StartCoroutine("CheckResolveProgress");
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
    void AdjustPosition()
    {
        //保存したアンカーの位置V3と現在のアンカー位置V3の距離が10cm未満ならreturn
        if (Vector3.Distance(anchorPosTemp, cloudAnchorTransform.transform.position) < 0.1f)
        {
            debugText.text = Vector3.Distance(anchorPosTemp, cloudAnchorTransform.transform.position).ToString();
            return;
        }
        //そうでないならLineRendererのオブジェクトの位置を差分だけ移動させ、保存を更新
        else
        {
            shareAura.SetDebugAxis(cloudAnchorTransform.position, cloudAnchorTransform.rotation.eulerAngles);
            debugText.text = Vector3.Distance(lineRendererTransform.position, cloudAnchorTransform.transform.position).ToString();
            lineRendererTransform.position = lineRendererTransform.position + (cloudAnchorTransform.transform.position - anchorPosTemp);
            anchorPosTemp = cloudAnchorTransform.transform.position;
        }
    }
}
