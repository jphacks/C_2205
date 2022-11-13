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
    [SerializeField] private float syncInterval;
    [SerializeField] private GameObject particlePrefab;
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
            float[] positionAsArray = new float[] {positionTarget.position.x, positionTarget.position.y, positionTarget.position.z };
            myKehaiTable["position"] = positionAsArray;
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
        myKehaiTable["inGame"] = true;
        myKehaiTable.SaveAsync();
    }
    //ゲーム終了時、NCMBとの通信を終了、ゲームにいない、と登録する
    public void EndNCMBSync()
    {
        ncmbSyncStarted = false;
        myKehaiTable["position"] = new float[] { 0, 10, 0 };
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
                    ArrayList objAsArray = obj["position"] as ArrayList;
                    Vector3 auraPosition = new Vector3(Convert.ToSingle(objAsArray[0]), Convert.ToSingle(objAsArray[1]), Convert.ToSingle(objAsArray[2]));
                    Instantiate(particlePrefab, auraPosition, Quaternion.identity);
                }
            }
        });
    }
}
