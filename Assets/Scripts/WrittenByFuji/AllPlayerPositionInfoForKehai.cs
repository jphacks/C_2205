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
    private bool ncmbSyncStarted;

    [SerializeField] private GameObject syncDebugObject;
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
        if(elapsedTime > 5 && ncmbSyncStarted)
        {
            float[] positionAsArray = new float[] {positionTarget.position.x, positionTarget.position.y, positionTarget.position.z };
            myKehaiTable["position"] = positionAsArray;
            myKehaiTable["inGame"] = true;
            myKehaiTable.SaveAsync();
            elapsedTime = 0;
        }
    }

    //�Q�[���X�^�[�g�{�^�����������Ƃ��ANCMB�Ƃ̒ʐM���J�n
    public void StartNCMBSync()
    {
        elapsedTime = 0;
        ncmbSyncStarted = true;
        
        //�ŏ���{0,10,0}��o�^�A�Q�[���ɎQ�����Ă���Ɠo�^���ۑ�
        myKehaiTable["position"] = new float[] { 0, 10, 0 };
        myKehaiTable["inGame"] = true;
        myKehaiTable.SaveAsync();
    }
    //�Q�[���I�����ANCMB�Ƃ̒ʐM���I���A�Q�[���ɂ��Ȃ��A�Ɠo�^����
    public void EndNCMBSync()
    {
        ncmbSyncStarted = false;
        myKehaiTable["position"] = new float[] { 0, 10, 0 };
        myKehaiTable["inGame"] = false;
        myKehaiTable.SaveAsync();
    }
    public List<Vector3> GetAllPlayerPosition()
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
                    ArrayList objAsArray = obj["position"] as ArrayList;
                    Vector3 position = new Vector3(Convert.ToSingle(objAsArray[0]), Convert.ToSingle(objAsArray[1]), Convert.ToSingle(objAsArray[2]));
                    positionList.Add(position);
                }
            }
        });
        return positionList;
    }
}
