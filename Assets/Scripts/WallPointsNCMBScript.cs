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
        //NCMB���ID�̏ꏊ�����
        wallPointsClass = new NCMBObject("WallPoints");
        wallPointsClass.ObjectId = "xAPdhJzA9PHX52IR";
    }

    //NCMB�ɕǂ̍��W�̔z����󂯓n��
    public void HostWallPoints(Vector3[] positions)
    {
        int Len = positions.Length;
        // debugText.text = "Host Success!\nPosition:" + cloudAnchorHosted.pose.position + "\nRotation:" + cloudAnchorHosted.pose.rotation;
        // anchorHostInProgress = false;
        //x,y,z�̔z��ɕ���
        float[] posX = new float[Len], posY = new float[Len], posZ = new float[Len];
        for (int i = 0; i < Len; i++)
        {
            posX[i] = positions[i].x;
            posY[i] = positions[i].y;
            posZ[i] = positions[i].z;
        }
        //NCMB�ɌĂяo���p��ID���A�b�v���[�h
        wallPointsClass["posX"] = posX;
        wallPointsClass["posY"] = posY;
        wallPointsClass["posZ"] = posZ;
        wallPointsClass.SaveAsync((NCMBException e) =>
        {
            if(e != null)
            {
                debugText.text = "NCMB�G���[:" + e;
            }
            else
            {
                debugText.text = "��������!\n�Q�[���X�^�[�g������\n�S�[�O��������\n�킢���n�߂悤�B";
                startVRButton.gameObject.SetActive(true);
            }
        });
    }
    /// <summary>
    /// NCMB����ǂ̍��W�̔z����󂯎��
    /// </summary>
    /// <param name="baseposition">�Ǒ��΍��W�����΍��W�ɕϊ����邽�߂̊�_</param>
    /// <param name="eulerangles">��_�̌����Ă������</param>
    public void ReceiveWallPoints(Vector3 baseposition, Vector3 eulerangles)
    {
        wallPointsClass.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                debugText.text = "NCMB�G���[:" + e;
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
     * �g��ʂȂ�
     * �����Ă��܂�
     * ������Ղ�
     * 
    /// <summary>
    /// NCMB����ǂ̍��W�̔z����󂯎��
    /// </summary>
    /// <param name="baseposition">�Ǒ��΍��W�����΍��W�ɕϊ����邽�߂̊�_</param>
    public void ReceiveWallPoints(Vector3 baseposition)
    {
        wallPointsClass.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                debugText.text = "NCMB�G���[:" + e;
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
