using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class CreateBasePlane : MonoBehaviour
{
    [SerializeField] private Button setPlaneButton, planeUpButton, planeDownButton, finishSettingButton;
    private ARPlaneManager arPlaneManager;
    private ARRaycastManager arRaycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARPlane planeSelected, basePlane;
    [SerializeField] private Material defaultMaterial, selectedPlaneMaterial;
    [SerializeField] private GameObject testObject,initialCircle;

    private ARAnchorManager arAnchorManager;
    private HostARCloudAnchor hostARCloudAnchor;

    //起動時、コンポーネント取得、ボタンに機能付与
    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arRaycastManager = GetComponent<ARRaycastManager>();
        hostARCloudAnchor = GetComponent<HostARCloudAnchor>();
        arAnchorManager = GetComponent<ARAnchorManager>();

        //ホスト側のボタン、基準平面設定、基準平面高さ調整、調整終了
        if (setPlaneButton != null && planeUpButton != null && planeDownButton && finishSettingButton != null)
        {
            setPlaneButton.onClick.AddListener(SetBasePlane);
            planeUpButton.onClick.AddListener(() => AdjustPlaneHeight(1));
            planeDownButton.onClick.AddListener(() => AdjustPlaneHeight(-1));
            finishSettingButton.onClick.AddListener(FinishSetting);
        }
    }

    //UIをタッチしていない有効なタッチであるかどうか、有効ならばタッチした場所を出力する
    bool IsValidTouch(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //GameObjectならUIにタッチしていないと確定できるのでtrue
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    touchPosition = touch.position;
                    return true;
                }
                //タッチした点にイベントがあるならRaycast結果に格納、結果が0ならUIに触れていない。
                PointerEventData eventPosition = new PointerEventData(EventSystem.current);

                //Raycastの始点とするため位置を設定する
                eventPosition.position = touch.position;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventPosition, results);

                //出力するtouchPositionはRaycastの結果による
                touchPosition = results.Count == 0 ? touch.position : default;
                return results.Count == 0;
            }
        }
        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        //有効なタッチならば場所とARPlaneを取得
        if (!IsValidTouch(out Vector2 touchPosition))
        {
            return;
        }
        if (arRaycastManager.Raycast(touchPosition,hits,UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            SetTouchPositionAndPlane();
        }
    }
    //平面選択
    private void SetTouchPositionAndPlane()
    {
        //基準平面を設定していないなら
        if (basePlane == null)
        {
            //最初にヒットしたARPlaneを格納、マテリアルを別のに
            foreach (ARPlane plane in arPlaneManager.trackables)
            {
                plane.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
            planeSelected = arPlaneManager.GetPlane(hits[0].trackableId);
            planeSelected.gameObject.GetComponent<MeshRenderer>().material = selectedPlaneMaterial;
        }
        //マーカーが生成されていないならタップ位置に生成、AnchorをつけTransform型で格納し平面を親とする。
        if (testObject.activeSelf == false)
        {
            testObject.SetActive(true);
        }
        testObject.transform.position = hits[0].pose.position;
    }

    //基準平面設定ボタン
    private void SetBasePlane()
    {
        //基準平面が設定されていないなら設定
        if (basePlane == null)
        {
            basePlane = planeSelected;
        }
        //基準平面でない平面を非表示
        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        basePlane.gameObject.SetActive(true);

        //平面検出機能を無効化
        arPlaneManager.enabled = false;
    }
    
    //平面高さ調整
    private void AdjustPlaneHeight(int vec)
    {
        if(basePlane != null)
        {
            basePlane.transform.Translate(Vector3.up * vec * 0.01f);
            testObject.transform.Translate(Vector3.up * vec * 0.01f);
        }
    }

    //設定終了、円形の初期フィールドを配置、マーカーをローカルアンカーとする
    private void FinishSetting()
    {
        GameObject circle = Instantiate(initialCircle, testObject.transform.position, Quaternion.identity);
        if (hostARCloudAnchor.enabled)
        {
            //hostARCloudAnchor.pendingHostAnchor = testObject.transform.gameObject.AddComponent<ARAnchor>();
            hostARCloudAnchor.pendingHostAnchor = arAnchorManager.AttachAnchor(basePlane, new Pose(testObject.transform.position, testObject.transform.rotation));
            hostARCloudAnchor.anchorObject = testObject;
            hostARCloudAnchor.initialCircle = circle;
        }
        basePlane.gameObject.SetActive(false);
    }
}
