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

        //プレイヤー固有のIDがあるならそのテーブルにつく
        if (PlayerPrefs.HasKey("PlayerID"))
        {
            myKehaiTable.ObjectId = PlayerPrefs.GetString("PlayerID");
        }
        //ないならIDを{0,10,0},falseを保存しPlayerPrefsにID登録
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
     * このアプリ戻るボタン押しても何もないからここ要らないかもよ
    private void OnApplicationFocus(bool focus)
    {
        //戻るボタンで中断されたときはPauseが呼ばれるかわからないのでこっちを呼ぶ。更新を停止しNCMBをリセット
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
        //ゲームを中断したら更新を停止、NCMBをリセット
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

    //ゲームスタートボタンを押したとき、NCMBとの通信を開始
    public void StartNCMBSync()
    {
        elapsedTime = 0;
        ncmbSyncStarted = true;
        
        //最初は{0,10,0}を登録、ゲームに参加していると登録し保存
        myKehaiTable["position"] = new float[] { 0, 10, 0 };
        myKehaiTable["rotation"] = new float[] { 0, 0, 0 };
        myKehaiTable["inGame"] = true;
        myKehaiTable.SaveAsync();
    }
    //ゲーム終了時、NCMBとの通信を終了、ゲームにいない、と登録する
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
        //自分以外のIDのデータ取得
        ncmbQuery.WhereNotEqualTo("objectId", PlayerPrefs.GetString("PlayerID"));
        //ゲームに参加しているデータ取得
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
                    //NCMBオブジェクトをfloatの配列に変換
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
