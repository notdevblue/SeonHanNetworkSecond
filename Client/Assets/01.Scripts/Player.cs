using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;

    public int PlayerId {get; set;}

    public float t = 0.0f;

    public Vector3 lastPos;
    public Vector3 targetPos;



    protected virtual void Update()
    {
        ToDist();
    }

    public void ToDist()
    {
        t += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(lastPos, targetPos, t);
    }
}
