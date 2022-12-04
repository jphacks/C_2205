using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;
using TMPro;
using System;

public class AllPlayerPositionInfoForKehai : MonoBehaviour
{
    [SerializeField] private Button gameStartButton,gameEndButton;
    NCMBObject myKehaiTable;
    NCMBQuery<NCMBObject> ncmbQuery;
    private float elapsedTime;
    [SerializeField] private Transform positionTarget;
    [SerializeField] private GameObject auraPrefab;
    private bool ncmbSyncStarted;
    [SerializeField] private float syncInterval;
    public Transform auraGenerator;
    [HideInInspector] public Vector3 cloudAnchorPos, cloudAnchorRot;

    // Start is called before the first frame update
    private void Start()
    {
        gameStartButton.onClick.AddListener(StartNCMBSync);
        gameEndButton.onClick.AddListener(EndNCMBSync);

        myKehaiTable = new NCMBObject("kehai");
        ncmbQuery = new NCMBQuery<NCMBObject>("kehai");

        //�v���C���[�ŗL��ID������Ȃ炻�̃e�[�u���ɂ�
        if (PlayerPrefs.HasKey("PlayerID"))
        {
            myKehaiTable.ObjectId = PlayerPrefs.GetString("PlayerID");
        }
        //�Ȃ��Ȃ�ID��{0,10,0},false��ۑ���PlayerPrefs��ID�o�^
        else
        {
            myKehaiTable["position"] = new float[] { 0, 10, 0 };
            myKehaiTable["inGame"] = false;
            myKehaiTable.SaveAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    //error
                }
                else
                {
                    PlayerPrefs.SetString("PlayerID", myKehaiTable.ObjectId);
                    myKehaiTable.ObjectId = PlayerPrefs.GetString("PlayerID");
                }
            });
        }
        ncmbSyncStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > syncInterval && ncmbSyncStarted)
        {
            float[] positionDeltaAsArray = new float[] { positionTarget.position.x - cloudAnchorPos.x, positionTarget.position.y - cloudAnchorPos.y, positionTarget.position.z - cloudAnchorPos.z };
            float[] rotationAsArray = new float[] { cloudAnchorRot.x, cloudAnchorRot.y, cloudAnchorRot.z };
            myKehaiTable["position"] = positionDeltaAsArray;
            myKehaiTable["rotation"] = rotationAsArray;
            myKehaiTable["inGame"] = true;
            myKehaiTable.SaveAsync();
            elapsedTime = 0;
            GenerateAura();
        }
    }
    /*
     * ���̃A�v���߂�{�^�������Ă������Ȃ����炱���v��Ȃ�������
    private void OnApplicationFocus(bool focus)
    {
        //�߂�{�^���Œ��f���ꂽ�Ƃ���Pause���Ă΂�邩�킩��Ȃ��̂ł��������ĂԁB�X�V���~��NCMB�����Z�b�g
        if (!focus)
        {
            ncmbSyncStarted = false;
            myKehaiTable["position"] = new float[] { 0, 10, 0 };
            myKehaiTable["inGame"] = false;
            myKehaiTable.SaveAsync();
        }
    }
    */
    private void OnApplicationPause(bool pause)
    {
        //�Q�[���𒆒f������X�V���~�ANCMB�����Z�b�g
        if (pause)
        {
            ncmbSyncStarted = false;
            myKehaiTable["position"] = new float[] { 0, 10, 0 };
            myKehaiTable["rotation"] = new float[] { 0, 0, 0 };
            myKehaiTable["inGame"] = false;
            myKehaiTable.SaveAsync();
        }
        else
        {
            ncmbSyncStarted = true;
        }
    }

    //�Q�[���X�^�[�g�{�^�����������Ƃ��ANCMB�Ƃ̒ʐM���J�n
    public void StartNCMBSync()
    {
        elapsedTime = 0;
        ncmbSyncStarted = true;
        
        //�ŏ���{0,10,0}��o�^�A�Q�[���ɎQ�����Ă���Ɠo�^���ۑ�
        myKehaiTable["position"] = new float[] { 0, 10, 0 };
        myKehaiTable["rotation"] = new float[] { 0, 0, 0 };
        myKehaiTable["inGame"] = true;
        myKehaiTable.SaveAsync();
    }
    //�Q�[���I�����ANCMB�Ƃ̒ʐM���I���A�Q�[���ɂ��Ȃ��A�Ɠo�^����
    public void EndNCMBSync()
    {
        ncmbSyncStarted = false;
        myKehaiTable["position"] = new float[] { 0, 10, 0 };
        myKehaiTable["rotation"] = new float[] { 0, 0, 0 };
        myKehaiTable["inGame"] = false;
        myKehaiTable.SaveAsync();
    }
    private void GenerateAura()
    {
        List<Vector3> positionList = new List<Vector3>();
        //�����ȊO��ID�̃f�[�^�擾
        ncmbQuery.WhereNotEqualTo("objectId", PlayerPrefs.GetString("PlayerID"));
        //�Q�[���ɎQ�����Ă���f�[�^�擾
        ncmbQuery.WhereEqualTo("inGame", true);
        ncmbQuery.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                //error
            }
            else
            {
                foreach (NCMBObject obj in objList)
                {
                    //NCMB�I�u�W�F�N�g��float�̔z��ɕϊ�
                    ArrayList posAsArray = obj["position"] as ArrayList;
                    ArrayList rotAsArray = obj["rotation"] as ArrayList;
                    Vector3 rotDelta = new Vector3(cloudAnchorRot.x - Convert.ToSingle(rotAsArray[0]), cloudAnchorRot.y - Convert.ToSingle(rotAsArray[1]), cloudAnchorRot.z - Convert.ToSingle(rotAsArray[2]));
                    auraGenerator.GetChild(0).position = new Vector3(Convert.ToSingle(posAsArray[0]) + cloudAnchorPos.x, Convert.ToSingle(posAsArray[1]) + cloudAnchorPos.y, Convert.ToSingle(posAsArray[2]) + cloudAnchorPos.z);
                    auraGenerator.rotation = Quaternion.Euler(cloudAnchorRot - rotDelta);
                    positionList.Add(auraGenerator.GetChild(0).position);
                }
                for (int i = 0; i < positionList.Count; i++)
                {
                    Instantiate(auraPrefab, positionList[i], Quaternion.identity);
                }
            }
        });
    }
}
