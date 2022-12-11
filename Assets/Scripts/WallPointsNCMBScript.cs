using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using NCMB.Tasks;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;

public class WallPointsNCMBScript : MonoBehaviour
{
    [SerializeField]
    private LineRenderingTest m_lineRenderingTest;

    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] NCMBObject wallPointsClass;
    [SerializeField] Button startVRButton;
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
        wallPointsClass.SaveAsync((NCMBException e) =>
        {
            if(e != null)
            {
                debugText.text = "NCMBエラー:" + e;
            }
            else
            {
                debugText.text = "準備完了!\nゲームスタートした後\nゴーグルをつけて\n戦いを始めよう。";
                startVRButton.gameObject.SetActive(true);
            }
        });
    }
    /// <summary>
    /// NCMBから壁の座標の配列を受け取る
    /// </summary>
    /// <param name="baseposition">壁相対座標から絶対座標に変換するための基準点</param>
    /// <param name="eulerangles">基準点の向いている方向</param>
    public void ReceiveWallPoints(Vector3 baseposition, Vector3 eulerangles)
    {
        wallPointsClass.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                debugText.text = "NCMBエラー:" + e;
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
                m_lineRenderingTest.SetImportedPoints(positions, baseposition, eulerangles);

            }
        });
    }
    /*
     * 使わぬなら
     * 消してしまえ
     * すくりぷと
     * 
    /// <summary>
    /// NCMBから壁の座標の配列を受け取る
    /// </summary>
    /// <param name="baseposition">壁相対座標から絶対座標に変換するための基準点</param>
    public void ReceiveWallPoints(Vector3 baseposition)
    {
        wallPointsClass.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                debugText.text = "NCMBエラー:" + e;
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
                m_lineRenderingTest.SetImportedPoints(positions, baseposition);

            }
        });
    }
    */
}
