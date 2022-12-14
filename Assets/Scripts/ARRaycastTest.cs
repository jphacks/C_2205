using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARRaycastTest : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager m_raycastManager;

    [SerializeField]
    private GameObject m_pointObject;

    [SerializeField]
    private LineRenderer m_lineRenderer;

    private List<ARRaycastHit> m_hits = new List<ARRaycastHit>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TestForRendering();
        }
    }

    private void TestForAR()
    {
        if (m_raycastManager.Raycast(Input.GetTouch(0).position, m_hits))
        {
            Vector3 pos = new Vector3(m_hits[m_hits.Count - 1].pose.position.x, m_hits[m_hits.Count - 1].pose.position.y + m_pointObject.transform.position.y, m_hits[m_hits.Count - 1].pose.position.z);
            Instantiate(m_pointObject, pos, Quaternion.identity);
        }
    }

    private void TestForRendering()
    {
        m_lineRenderer.positionCount++;
        m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, new Vector3(m_lineRenderer.positionCount, m_lineRenderer.positionCount, m_lineRenderer.positionCount));
    }
}
