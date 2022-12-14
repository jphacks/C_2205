using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using TMPro;

public class LineRenderingTest : MonoBehaviour
{
    [SerializeField]
    private LineRenderer m_lineRenderer;

    [SerializeField]
    private RectTransform m_centerPoint;

    [SerializeField]
    private bool m_hasUpdatefunction = true;

    [SerializeField]
    private TextMeshProUGUI tM;

    private bool m_isHold; // �{�^�������������Ă��邩�ǂ���

    private WallPointsNCMBScript m_wallPointsNCMBScript;

    [HideInInspector] public Vector3 uploadBasePosition;
    private void Start()
    {
        m_wallPointsNCMBScript = GetComponent<WallPointsNCMBScript>();
    }

    private void Update()
    {
        if (!m_hasUpdatefunction) 
        {
            return;
        }
        // ���ɔz�u�����ǂ̗\����\������
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(m_centerPoint.position);
        if (Physics.Raycast(ray, out hit))
        {
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, hit.point);
        }
    }
    #region UIButton�n
    /// <summary>
    /// �N���b�N�ŕǂ����Ԃɍ쐬����
    /// </summary>
    public void OnButtonClick()
    {
        RenderingNextPoint();
    }

    /// <summary>
    /// �ݒ肵���_�����ׂč폜����
    /// </summary>
    public void ResetRendering()
    {
        m_lineRenderer.positionCount = 1;
    }

    /// <summary>
    /// �Ō�ɐݒ肵���_���폜����1�O�ɖ߂�
    /// </summary>
    public void BackPreviousRendering()
    {
        if (m_lineRenderer.positionCount == 0)
        {
            return;
        }
        m_lineRenderer.positionCount--;
    }

    /// <summary>
    /// ���̕ǂ����|�C���g���v�Z����
    /// </summary>
    private void RenderingNextPoint()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(m_centerPoint.position);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.point);
            m_lineRenderer.positionCount++;
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, hit.point);
        }
    }

    /// <summary>
    /// �ǐ����̏I��
    /// </summary>
    public void FinishMakeWall()
    {
        m_hasUpdatefunction = false;
        m_lineRenderer.loop = true;
        m_wallPointsNCMBScript.HostWallPoints(ExportLinePoints(uploadBasePosition));
    }

    #endregion

    /// <summary>
    /// linerenderer�̑��΃x�N�g���z����o��
    /// </summary>
    /// <param name="basepoint">�Ǖ\���̊�ƂȂ��ԍ��W</param>
    /// <returns></returns>
    private Vector3[] ExportLinePoints(Vector3 basepoint)
    {
        Vector3[] linePoints = new Vector3[m_lineRenderer.positionCount];
        //LineRenderer�̓_��linePoints�Ɋi�[
        m_lineRenderer.GetPositions(linePoints);
        // ���΍��W�ɕϊ�
        for (int i=0; i<linePoints.Length; i++)
        {
            // linePoints[i] -= basepoint;
            tM.text += linePoints[i] + "\n";
        }
        return linePoints;
    }

    /// <summary>
    /// linerenderer�̑��΃x�N�g���z����擾���A�K�p
    /// </summary>
    /// <param name="points"></param>
    /// <param name="basepoint">�Ǖ\���̊�ƂȂ��ԍ��W</param>
    /// <param name="eulerangles">���S���W�̉�]�̌���</param>
    public void SetImportedPoints(Vector3[] points, Vector3 basepoint, Vector3 eulerangles)
    {
        
        // ��΍��W�ɕϊ�
        for (int i = 0; i < points.Length; i++)
        {
            // points[i] += basepoint;
            tM.text += points[i] + "\n";
        }
        
        m_lineRenderer.positionCount = points.Length;
        m_lineRenderer.SetPositions(points);
        m_lineRenderer.gameObject.transform.eulerAngles = eulerangles;
    }
}
