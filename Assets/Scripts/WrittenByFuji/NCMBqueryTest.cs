using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using System;

public class NCMBqueryTest : MonoBehaviour
{
    NCMBObject testObj;
    // Start is called before the first frame update
    void Start()
    {
        testObj = new NCMBObject("kehai");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            testObj["position"] = new float[] { 1, 1, 1 };
            testObj["inGame"] = false;
            testObj.SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log("error");
                }
                else
                {
                    Debug.Log(testObj.ObjectId);
                }
            });
            
        }
    }
}
