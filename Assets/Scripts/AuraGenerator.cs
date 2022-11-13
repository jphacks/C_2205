using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraGenerator : MonoBehaviour
{
    private float elapsedTime;
    [SerializeField] private float interval;
    private AllPlayerPositionInfoForKehai allPlayerPositionInfoForKehai;
    [SerializeField] GameObject particlePrefab;
    // Start is called before the first frame update
    void Start()
    {
        allPlayerPositionInfoForKehai = GetComponent<AllPlayerPositionInfoForKehai>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime; 
        if(elapsedTime > interval)
        {
            GenerateAura();
            elapsedTime = 0;
        }
    }
    private void GenerateAura()
    {
        /*
        foreach (Vector3 auraPosition in allPlayerPositionInfoForKehai.GetAllPlayerPosition())
        {
            Instantiate(particlePrefab, auraPosition, Quaternion.identity);
        }
        */
    }

}
