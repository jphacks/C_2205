using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private Button setPlaneButton, clearPlaneButton, toggleButton, setAnchorButton, planeUpButton, planeDownButton, hostButton, resolveButton, clearButton;
    private ARPlaneManager arPlaneManager;
    private ARRaycastManager arRaycastManager;
    //private ARAnchorManager arAnchorManager;
    private List<ARAnchor> anchors = new List<ARAnchor>();
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField] private GameObject placedPrefab;

    private ARPlane planeSelected, basePlane;
    [SerializeField] private Material defaultMaterial, selectedPlaneMaterial;
    private Pose hitPose;
    private Transform spawnedObject;

    ARCloudAnchorManager arCloudAnchorManager;

    public GameObject PlacedPrefab
    {
        get
        {
            return placedPrefab;
        }
        set
        {
            placedPrefab = value;
        }
    }

    //起動時、コンポーネント取得、ボタンに機能付与
    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arRaycastManager = GetComponent<ARRaycastManager>();
        arCloudAnchorManager = GetComponent<ARCloudAnchorManager>();

        //ホスト側のボタン、基準平面設定、不要な平面除去、検出機能オンオフ、アンカー設置、ホスト
        if (setPlaneButton != null && clearPlaneButton != null && toggleButton != null && setAnchorButton != null && planeUpButton != null && planeDownButton　&& hostButton != null)
        {
            setPlaneButton.onClick.AddListener(SetBasePlane);
            clearPlaneButton.onClick.AddListener(ClearUnselectedPlane);
            toggleButton.onClick.AddListener(TogglePlaneDetection);
            setAnchorButton.onClick.AddListener(SetAnchorAtTouchPoint);
            planeUpButton.onClick.AddListener(() => AdjustPlaneHeight(1));
            planeDownButton.onClick.AddListener(() => AdjustPlaneHeight(-1));
            hostButton.onClick.AddListener(arCloudAnchorManager.HostAnchor);
        }
        //
        if (resolveButton != null)
        {
            resolveButton.onClick.AddListener(arCloudAnchorManager.Resolve);
        }
        //デバッグ、アンカーオブジェクト削除
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearObject);
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

    private void SetTouchPositionAndPlane()
    {
        //最初にヒットしたposeを格納
        hitPose = hits[0].pose;

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
    }
    //基準平面設定ボタン
    private void SetBasePlane()
    {
        if (basePlane == null)
        {
            basePlane = planeSelected;
            setPlaneButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unset Base Plane";
        }
        else
        {
            basePlane.GetComponent<MeshRenderer>().material = defaultMaterial;
            basePlane = null;
            setPlaneButton.GetComponentInChildren<TextMeshProUGUI>().text = "Set Base Plane";
        }
    }
    private void ClearUnselectedPlane()
    {
        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
            basePlane.gameObject.SetActive(true);
        }
    }
    //最後の有効タッチの場所に生成、1体まで
    private void SetAnchorAtTouchPoint()
    {
        if (hitPose != null)
        {
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).transform;
                ARAnchor anchor = spawnedObject.gameObject.AddComponent<ARAnchor>();
                anchors.Add(anchor);
                arCloudAnchorManager.QueueAnchor(anchor);
            }
            else
            {
                spawnedObject.position = hitPose.position;
                spawnedObject.rotation = hitPose.rotation;
            }
        }
    }

    private void ClearObject()
    {
        if(spawnedObject != null)
        {
            Destroy(spawnedObject.gameObject);
            spawnedObject = null;
        }
    }

    //平面検出オンオフ
    private void TogglePlaneDetection()
    {
        ClearUnselectedPlane();
        arPlaneManager.enabled = !arPlaneManager.enabled;
        toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = arPlaneManager.enabled ? "Disable Detection" : "Enable Detection";
    }
    //平面高さ調整
    private void AdjustPlaneHeight(int vec)
    {
        if(basePlane != null)
        {
            basePlane.transform.Translate(Vector3.up * vec * 0.01f);
        }
    }
    //クラウドアンカー読み込み時の生成
    public void ReCreatePlacement(Transform transform)
    {
        spawnedObject = Instantiate(placedPrefab, transform.position, transform.rotation).transform;
        spawnedObject.parent = transform;
    }
}
