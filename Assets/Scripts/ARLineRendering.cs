using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARLineRendering : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager m_raycastManager;

    [SerializeField]
    private LineRenderer m_lineRenderer;

    [SerializeField]
    private RectTransform m_centerPoint;

    private List<ARRaycastHit> m_hits = new List<ARRaycastHit>();

    private IEnumerator m_renderingCoroutine;

    private bool m_isHold; //ƒ{ƒ^ƒ“‚ð‰Ÿ‚µ‘±‚¯‚Ä‚¢‚é‚©‚Ç‚¤‚©

    private void Start()
    {
        m_renderingCoroutine = RenderingCoroutine();
    }

    public void OnButtonDown()
    {
        m_isHold = true;
        StartCoroutine(m_renderingCoroutine);
    }

    public void OnButtonUp()
    {
        m_isHold=false;
    }

    public void ResetRendering()
    {
        m_lineRenderer.positionCount = 0;
    }

    private IEnumerator RenderingCoroutine()
    {
        while (m_isHold)
        {
            if (m_raycastManager.Raycast(m_centerPoint.transform.position, m_hits))
            {
                m_lineRenderer.positionCount++;
                m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, m_hits[m_hits.Count - 1].pose.position);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }


}
