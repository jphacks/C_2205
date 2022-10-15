using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]

public class PlacementController : MonoBehaviour
{
    [SerializeField] private Button setPlaneButton, clearPlaneButton, toggleButton, spawnButton, planeUpButton, planeDownButton;
    private ARPlaneManager arPlaneManager;
    private ARRaycastManager arRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField] private GameObject placedPrefab;

    private ARPlane planeSelected, basePlane;
    [SerializeField] private Material defaultMaterial, selectedPlaneMaterial;
    private Pose hitPose;
    private Transform spawnedObject;

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

    //�N�����A�R���|�[�l���g�擾�A�{�^���ɋ@�\�t�^
    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arRaycastManager = GetComponent<ARRaycastManager>();
        if(setPlaneButton != null && clearPlaneButton!= null && toggleButton != null && spawnButton != null && planeUpButton != null && planeDownButton != null)
        {
            setPlaneButton.onClick.AddListener(SetSelectedPlane);
            clearPlaneButton.onClick.AddListener(ClearUnselectedPlane);
            toggleButton.onClick.AddListener(TogglePlaneDetection);
            spawnButton.onClick.AddListener(SpawnAtTouchPoint);
            planeUpButton.onClick.AddListener(() => AdjustPlaneHeight(1));
            planeDownButton.onClick.AddListener(() => AdjustPlaneHeight(-1));
        }
    }

    //UI���^�b�`���Ă��Ȃ��L���ȃ^�b�`�ł��邩�ǂ����A�L���Ȃ�΃^�b�`�����ꏊ���o�͂���
    bool IsValidTouch(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //GameObject�Ȃ�UI�Ƀ^�b�`���Ă��Ȃ��Ɗm��ł���̂�true
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    touchPosition = touch.position;
                    return true;
                }
                //�^�b�`�����_�ɃC�x���g������Ȃ�Raycast���ʂɊi�[�A���ʂ�0�Ȃ�UI�ɐG��Ă��Ȃ��B
                PointerEventData eventPosition = new PointerEventData(EventSystem.current);

                //Raycast�̎n�_�Ƃ��邽�߈ʒu��ݒ肷��
                eventPosition.position = touch.position;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventPosition, results);

                //�o�͂���touchPosition��Raycast�̌��ʂɂ��
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
        //�L���ȃ^�b�`�Ȃ�Ώꏊ��ARPlane���擾
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
        //�ŏ��Ƀq�b�g����pose���i�[
        hitPose = hits[0].pose;

        //����ʂ�ݒ肵�Ă��Ȃ��Ȃ�
        if (basePlane == null)
        {
            //�ŏ��Ƀq�b�g����ARPlane���i�[�A�}�e���A����ʂ̂�
            foreach (ARPlane plane in arPlaneManager.trackables)
            {
                plane.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
            planeSelected = arPlaneManager.GetPlane(hits[0].trackableId);
            planeSelected.gameObject.GetComponent<MeshRenderer>().material = selectedPlaneMaterial;
        }
    }
    //����ʐݒ�{�^��
    private void SetSelectedPlane()
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
    //�Ō�̗L���^�b�`�̏ꏊ�ɐ����A1�̂܂�
    private void SpawnAtTouchPoint()
    {
        if (hitPose != null)
        {
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).transform;
            }
            else
            {
                spawnedObject.position = hitPose.position;
                spawnedObject.rotation = hitPose.rotation;
            }
        }
    }

    //���ʌ��o�I���I�t
    private void TogglePlaneDetection()
    {
        ClearUnselectedPlane();
        arPlaneManager.enabled = !arPlaneManager.enabled;
        toggleButton.GetComponentInChildren<TextMeshProUGUI>().text = arPlaneManager.enabled ? "Disable Detection" : "Enable Detection";
    }
    private void AdjustPlaneHeight(int vec)
    {
        if(basePlane != null)
        {
            basePlane.transform.Translate(Vector3.up * vec * 0.01f);
        }
    }
}
