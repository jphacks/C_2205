using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using NCMB.Tasks;
using TMPro;
using System.Threading.Tasks;

public class WallPointsNCMBScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI debugText;
    NCMBObject wallPointsClass;
    private void Awake()
    {
        //NCMB上のIDの場所を特定
        wallPointsClass = new NCMBObject("WallPoints");
        wallPointsClass.ObjectId = "xAPdhJzA9PHX52IR";
    }

    //NCMBに壁の座標の配列を受け渡す
    public void HostWallPoints(Vector3[] positions)
    {
        int Len = positions.Length;
        // debugText.text = "Host Success!\nPosition:" + cloudAnchorHosted.pose.position + "\nRotation:" + cloudAnchorHosted.pose.rotation;
        // anchorHostInProgress = false;
        //x,y,zの配列に分解
        float[] posX = new float[Len], posY = new float[Len], posZ = new float[Len];
        for (int i = 0; i < Len; i++)
        {
            posX[i] = positions[i].x;
            posY[i] = positions[i].y;
            posZ[i] = positions[i].z;
        }
        //NCMBに呼び出し用のIDをアップロード
        wallPointsClass["posX"] = posX;
        wallPointsClass["posY"] = posY;
        wallPointsClass["posZ"] = posZ;
        wallPointsClass.SaveAsync();
    }

    //NCMBから壁の座標の配列を受け取る
    public void ReceiveWallPoints()
    {
        wallPointsClass.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                debugText.text = "NCMB Error:" + e;
            }
            else
            {
                ArrayList resX = wallPointsClass["posX"] as ArrayList;
                ArrayList resY = wallPointsClass["posY"] as ArrayList;
                ArrayList resZ = wallPointsClass["posZ"] as ArrayList;
                int Len = resX.Count;
                Vector3[] positions = new Vector3[Len];
                for (int i = 0; i < Len; i++)
                {
                    positions[i] = new Vector3(float.Parse(resX[i].ToString()), float.Parse(resY[i].ToString()), float.Parse(resZ[i].ToString()));
                }
                //Debug.Log(positions[1]);
                //ここでvoid 壁生成(Vector3[] positions)を呼ぶ

            }
        });
    }

    //座標をNCMBに保存するテスト関数
    public void TestVector3()
    {
        Vector3[] v3 = new[]{
            new Vector3(0.1f,1.2f,2.3f),
            new Vector3(3.4f,4.5f,5.6f),
            new Vector3(6.7f,7.8f,8.9f),
            new Vector3(9.1f,10.1f,11.1f)
        };
        HostWallPoints(v3);
    }
}
