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

    private bool m_isHold; //ボタンを押し続けているかどうか

    private void Update()
    {
        if (!m_hasUpdatefunction)
        {
            return;
        }
        // 次に配置される壁の予測を表示する
        if (m_raycastManager.Raycast(m_centerPoint.transform.position, m_hits))
        {
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, m_hits[m_hits.Count - 1].pose.position);
        }
    }

    /// <summary>
    /// クリックで壁を順番に作成する
    /// </summary>
    public void OnButtonClick()
    {
        RenderingNextPoint();
    }

    /// <summary>
    /// 設定した点をすべて削除する
    /// </summary>
    public void ResetRendering()
    {
        m_lineRenderer.positionCount = 1;
    }

    /// <summary>
    /// 最後に設定した点を削除して1つ前に戻る
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
    /// ボタンの長押しし始めの処理
    /// </summary>
    public void OnButtonDown()
    {
        m_isHold = true;
        StartCoroutine(RenderingCoroutine());
    }

    /// <summary>
    /// 長押しをやめた時の処理
    /// </summary>
    public void OnButtonUp()
    {
        m_isHold = false;
    }

    /// <summary>
    /// 長押し中に呼び出されるコルーチン
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
    /// 次の壁を作るポイントを計算する
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
    /// 壁生成の終了
    /// </summary>
    public void FinishMakeWall()
    {
        m_hasUpdatefunction = false;
        m_lineRenderer.loop = true;
    }
}
