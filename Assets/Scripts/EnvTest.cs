using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvTest : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_TextMeshPro;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_TextMeshPro2;

    // Start is called before the first frame update
    void Start()
    {
        m_TextMeshPro.text = Environment.GetEnvironmentVariable("appKey");
        m_TextMeshPro2.text = Environment.GetEnvironmentVariable("clientKey");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
