using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jazz : MonoBehaviour
{
    private Camera _camera;
    private float _hue;
    public float hueRate = 0.5f;

    private void OnEnable() => _camera = Camera.main;
    void Update()
    {
        _hue += hueRate * Time.deltaTime;
        if (_hue >= 1f)
            _hue = 0f;

        _camera.backgroundColor = Color.HSVToRGB(_hue, 1f, 1f);
    }
}
