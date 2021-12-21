using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    Material _playerMat;

    private void Start() 
    {
        _playerMat = GetComponent<SpriteRenderer>().material;
    }

    private void Update() 
    {
        // if(Input.GetButtonDown("Jump"))
        // {
        //     float current = _playerMat.GetFloat("_Intensity");
        //     _playerMat.SetFloat("_Intensity", current + 0.5f);
        // }
        
        _playerMat.SetFloat("_Intensity", Mathf.Sin(Time.deltaTime) * 1.5f + 1.5f);


    }
}
