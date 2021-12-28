using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectManager : MonoBehaviour
{
    private Volume _volume = null;
    ChromaticAberration _chromatic;


    private void Awake()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet<ChromaticAberration>(out _chromatic);
        if(_chromatic == null) Destroy(this);
    }

    private void Update()
    {
        if(_chromatic.intensity.value > 0) {
            _chromatic.intensity.value = Mathf.Clamp(_chromatic.intensity.value - Time.deltaTime * 2.0f, 0.0f, 3.0f);
        }

        if(Input.GetButtonDown("Jump")) {
            _chromatic.intensity.value = 3.0f;
        }
    }
}
