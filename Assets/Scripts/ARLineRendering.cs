using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARLineRendering : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager m_raycastManager;

    [SerializeField]
    private LineRenderer m_lineRenderer;

    [SerializeField]
    private RectTransform m_centerPoint;

    [SerializeField]
    private bool m_hasUpdatefunction = true;

    private List<ARRaycastHit> m_hits = new List<ARRaycastHit>();

    private IEnumerator m_renderingCoroutine;

    private bool m_isHold; //�{�^�������������Ă��邩�ǂ���

    private void Update()
    {
        if (!m_hasUpdatefunction)
        {
            return;
        }
        // ���ɔz�u�����ǂ̗\����\������
        if (m_raycastManager.Raycast(m_centerPoint.transform.position, m_hits))
        {
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, m_hits[m_hits.Count - 1].pose.position);
        }
    }

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
    /// �{�^���̒��������n�߂̏���
    /// </summary>
    public void OnButtonDown()
    {
        m_isHold = true;
        StartCoroutine(RenderingCoroutine());
    }

    /// <summary>
    /// ����������߂����̏���
    /// </summary>
    public void OnButtonUp()
    {
        m_isHold = false;
    }

    /// <summary>
    /// ���������ɌĂяo�����R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator RenderingCoroutine()
    {
        while (m_isHold)
        {
            RenderingNextPoint();
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// ���̕ǂ����|�C���g���v�Z����
    /// </summary>
    private void RenderingNextPoint()
    {
        if (m_raycastManager.Raycast(m_centerPoint.transform.position, m_hits))
        {
            m_lineRenderer.positionCount++;
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, m_hits[m_hits.Count - 1].pose.position);
        }
    }

    /// <summary>
    /// �ǐ����̏I��
    /// </summary>
    public void FinishMakeWall()
    {
        m_hasUpdatefunction = false;
        m_lineRenderer.loop = true;
    }
}
