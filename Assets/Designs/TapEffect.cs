using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffect : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        camera = Camera.main;
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
                transform.position = camera.ScreenToWorldPoint(touchposition);
                particleSystem.Play();
            }
        }
    }
}
