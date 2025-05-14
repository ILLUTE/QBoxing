using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Camera _camera;

    public bool m_IgnoreY = true;
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_camera == null) return;

        Vector3 pos = _camera.transform.position;
        Vector3 ourPosition = transform.position;
        if (m_IgnoreY)
        {
            pos.y = 0;
            ourPosition.y = 0;
        }
        transform.rotation = Quaternion.LookRotation((ourPosition - pos).normalized);
    }
}
