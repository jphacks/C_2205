using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_move : MonoBehaviour
{
    private Vector3 inputVector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.z = Input.GetAxis("Vertical");
        if(inputVector.magnitude > 0.2f)
        {
            transform.position = transform.position + (inputVector * Time.deltaTime);
        }
        
    }
}
