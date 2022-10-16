using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveSimurator : MonoBehaviour
{
    [SerializeField]
    private Camera m_camera;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            m_camera.transform.eulerAngles -= new Vector3(Time.deltaTime * 10, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_camera.transform.eulerAngles -= new Vector3(0, Time.deltaTime * 10, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_camera.transform.eulerAngles += new Vector3(Time.deltaTime * 10, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_camera.transform.eulerAngles += new Vector3(0, Time.deltaTime * 10, 0);
        }
    }
}
