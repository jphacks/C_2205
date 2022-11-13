using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using System;

public class NCMBqueryTest : MonoBehaviour
{
    [SerializeField] AllPlayerPositionInfoForKehai allPlayerPositionInfoForKehai;
    NCMBObject testObj;
    // Start is called before the first frame update
    void Start()
    {
        testObj = new NCMBObject("kehai");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
