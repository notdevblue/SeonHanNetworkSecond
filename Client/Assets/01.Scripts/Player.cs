using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerId {get; set;}


    public Vector3 targetPos;

    protected virtual void Update()
    {
        ToDist();
    }

    public void ToDist()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.05f);
    }
}
