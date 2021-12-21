using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MonoBehaviour
{
    Material _playerMat;

    float _progress = 0.0f;

    private void Awake()
    {
        _playerMat = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        _progress += Time.deltaTime * 0.2f;
        _playerMat.SetFloat("_Progress", _progress);

        if(_progress >= 1.0f) Destroy(gameObject);
    }

    private void OnDisable()
    {
        _progress = 0.0f;
        _playerMat.SetFloat("_Progress", _progress);
    }
}
