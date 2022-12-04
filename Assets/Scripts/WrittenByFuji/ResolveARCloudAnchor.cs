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
            if(e != null)
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
    public void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchorResolved.cloudAnchorState;
        //���������炻�̏ꏊ�ɐ���
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            debugText.text = "�Ăяo������!\n�Q�[���X�^�[�g������\n�S�[�O��������\n�킢���n�߂悤�B";
            anchorResolveInProgress = false;
            resolvedObject = Instantiate(resolveObject, cloudAnchorResolved.transform).transform;
            // NCMB����Ǎ��W���擾����lineRenderer�ɔ��f
            pointsNCMBScript.ReceiveWallPoints(resolvedObject.position, cloudAnchorResolved.transform.eulerAngles);
            //�C�z���L�̊�ɂ��ׂ�Position,Rotation��n���A�I�[�������I�u�W�F�N�g�̐e���A���J�[�Ɠ����ʒu�A�p�x�ɂ���
            allPlayerPositionInfoForKehai.cloudAnchorPos = cloudAnchorResolved.transform.position;
            allPlayerPositionInfoForKehai.cloudAnchorRot = cloudAnchorResolved.transform.rotation.eulerAngles;
            allPlayerPositionInfoForKehai.auraGenerator.position = cloudAnchorResolved.transform.position;
            allPlayerPositionInfoForKehai.auraGenerator.rotation = cloudAnchorResolved.transform.rotation;
            switchToVR.switchToVRButton.gameObject.SetActive(true);
            //���E�̍����̃e�L�X�g
            VRdebugText.text = $"X:{cloudAnchorResolved.transform.position.x}\nY:{cloudAnchorResolved.transform.position.y}\nZ:{cloudAnchorResolved.transform.position.z}";
        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            debugText.text = $"�Ăяo������\n�G���[: {cloudAnchorState}";
            anchorResolveInProgress = false;
        }
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
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
}
