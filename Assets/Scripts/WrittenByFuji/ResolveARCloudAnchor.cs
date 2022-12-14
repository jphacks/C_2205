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
        //NCMB���ID�̏ꏊ�����
        resolveIDClass = new NCMBObject("ResolveAnchorID");
        resolveIDClass.ObjectId = "vF64xh7DPtrds3ol";
        if (resolveButton != null)
        {
            resolveButton.onClick.AddListener(ResolveAnchor);
        }
        arAnchorManager = GetComponent<ARAnchorManager>();
    }
    //�Ăяo��
    public void ResolveAnchor()
    {
        debugText.text = "�Ăяo�����c";
        resolveIDClass.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                debugText.text = "�G���[����:" + e;
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
    //�Ăяo����Ƃ̊ē�
    IEnumerator CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchorResolved.cloudAnchorState;
        //���������炻�̏ꏊ�ɐ���
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "�Ăяo������!\n�Q�[���X�^�[�g������\n�S�[�O��������\n�킢���n�߂悤�B";
            anchorResolveInProgress = false;
            cloudAnchorTransform = Instantiate(resolveObject, cloudAnchorResolved.transform.position, cloudAnchorResolved.transform.rotation).transform;
            yield return null;
            // NCMB����Ǎ��W���擾����lineRenderer�ɔ��f
            pointsNCMBScript.ReceiveWallPoints(cloudAnchorTransform.position, cloudAnchorTransform.eulerAngles);
            anchorPosTemp = cloudAnchorTransform.position;
            yield return null;
            //InvokeRepeating("AdjustPosition", 0, 5);

            //�C�z���L�̊�ɂ��ׂ�Position,Rotation��n���A�I�[�������I�u�W�F�N�g�̐e���A���J�[�Ɠ����ʒu�A�p�x�ɂ���
            shareAura.cloudAnchorPos = cloudAnchorTransform.position;
            shareAura.cloudAnchorRot = cloudAnchorTransform.rotation.eulerAngles;
            shareAura.auraGenerator.position = cloudAnchorTransform.position;
            shareAura.auraGenerator.rotation = cloudAnchorTransform.rotation;
            shareAura.SetDebugAxis(cloudAnchorTransform.position, cloudAnchorTransform.rotation.eulerAngles);
            switchToVR.switchToVRButton.gameObject.SetActive(true);
            //���E�̍����̃e�L�X�g
            VRdebugText.text = $"X:{cloudAnchorResolved.transform.rotation.x}\nY:{cloudAnchorResolved.transform.rotation.y}\nZ:{cloudAnchorResolved.transform.rotation.z}";
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"�Ăяo������\n�G���[: {cloudAnchorState}";
            anchorResolveInProgress = false;
        }
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        //���[�h�`�F�b�N
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
        //�ۑ������A���J�[�̈ʒuV3�ƌ��݂̃A���J�[�ʒuV3�̋�����10cm�����Ȃ�return
        if (Vector3.Distance(anchorPosTemp, cloudAnchorTransform.transform.position) < 0.1f)
        {
            debugText.text = Vector3.Distance(anchorPosTemp, cloudAnchorTransform.transform.position).ToString();
            return;
        }
        //�����łȂ��Ȃ�LineRenderer�̃I�u�W�F�N�g�̈ʒu�����������ړ������A�ۑ����X�V
        else
        {
            shareAura.SetDebugAxis(cloudAnchorTransform.position, cloudAnchorTransform.rotation.eulerAngles);
            debugText.text = Vector3.Distance(lineRendererTransform.position, cloudAnchorTransform.transform.position).ToString();
            lineRendererTransform.position = lineRendererTransform.position + (cloudAnchorTransform.transform.position - anchorPosTemp);
            anchorPosTemp = cloudAnchorTransform.transform.position;
        }
    }
}
