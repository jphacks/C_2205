using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffect : MonoBehaviour
{
    private ParticleSystem myPS;
    private Camera myCam;
    // Start is called before the first frame update
    void Start()
    {
        myPS = GetComponent<ParticleSystem>();
        myCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchposition = touch.position;
                touchposition.z = 10;
                transform.position = myCam.ScreenToWorldPoint(touchposition);
                myPS.Play();
            }
        }
    }
}
