using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_ParticleInsideForLoop : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("Inst");
    }

    IEnumerator Inst()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return null;
            transform.position = (transform.position + Vector3.right * i);
            Instantiate(particle);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (transform.position + Vector3.right * 0.1f);
        transform.GetComponent<ParticleSystem>().Play();
    }
}
