using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPartialCameraMove : MonoBehaviour
{
    [SerializeField]
    private Camera m_camera;


    // Update is called once per frame
    void Update()
    {
        // カメラのx,z座標, y方向回転を追従
        transform.position = new Vector3(m_camera.transform.position.x, transform.position.y, m_camera.transform.position.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, m_camera.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
