using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateSeonHan : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Instantiate(prefab, pos, Quaternion.identity);
        }
    }
}
