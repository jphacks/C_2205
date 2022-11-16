using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Google.XR.ARCoreExtensions;

public class LineRenderingTest : MonoBehaviour
{
    [SerializeField]
    private LineRenderer m_lineRenderer;

    [SerializeField]
    private RectTransform m_centerPoint;

    [SerializeField]
    private bool m_hasUpdatefunction = true;

    private bool m_isHold; // ボタンを押し続けているかどうか

    private ARCloudAnchor m_cloudAnchor;

    private void Update()
    {
        if (!m_hasUpdatefunction) 
        {
            return;
        }
        // 次に配置される壁の予測を表示する
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(m_centerPoint.position);
        if (Physics.Raycast(ray, out hit))
        {
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, hit.point);
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
    /// 壁生成の終了
    /// </summary>
    public void FinishMakeWall()
    {
        m_hasUpdatefunction = false;
        m_lineRenderer.loop = true;
    }

    /// <summary>
    /// linerendererの相対ベクトル配列を出力
    /// </summary>
    /// <param name="basepoint">壁表示の基準となる空間座標</param>
    /// <returns></returns>
    public Vector3[] ExportLinePoints(Vector3 basepoint)
    {
        Vector3[] linePoints = new Vector3[m_lineRenderer.positionCount];
        m_lineRenderer.GetPositions(linePoints);
        // 相対座標に変換
        for (int i=0; i<linePoints.Length; i++)
        {
            linePoints[i] -= basepoint;
        }
        return linePoints;
    }


    /// <summary>
    /// linerendererの絶対座標配列を出力
    /// </summary>
    /// <returns></returns>
    public Vector3[] ExportLinePoints()
    {
        Vector3[] linePoints = new Vector3[m_lineRenderer.positionCount];
        m_lineRenderer.GetPositions(linePoints);
        return linePoints;
    }

    /// <summary>
    /// linerendererの相対ベクトル配列を取得し、適用
    /// </summary>
    /// <param name="points"></param>
    /// <param name="basepoint">壁表示の基準となる空間座標</param>
    public void SetImportedPoints(Vector3[] points, Vector3 basepoint)
    {
        // 絶対座標に変換
        for (int i = 0; i < points.Length; i++)
        {
            points[i] += basepoint;
        }
        m_lineRenderer.positionCount = points.Length;
        m_lineRenderer.SetPositions(points);
    }

    /// <summary>
    /// linerendererの相対ベクトル配列を取得し、適用
    /// </summary>
    /// <param name="points"></param>
    /// <param name="basepoint">壁表示の基準となる空間座標</param>
    /// <param name="eulerangles">中心座標の回転の向き</param>
    public void SetImportedPoints(Vector3[] points, Vector3 basepoint, Vector3 eulerangles)
    {
        // 絶対座標に変換
        for (int i = 0; i < points.Length; i++)
        {
            points[i] += basepoint;
        }
        m_lineRenderer.positionCount = points.Length;
        m_lineRenderer.SetPositions(points);
        m_lineRenderer.gameObject.transform.eulerAngles = eulerangles;
    }

    /// <summary>
    /// linerendererの絶対座標配列を取得し、適用
    /// </summary>
    /// <param name="points"></param>
    public void SetImportedPoints(Vector3[] points)
    {
        m_lineRenderer.positionCount = points.Length;
        m_lineRenderer.SetPositions(points);
    }
}
