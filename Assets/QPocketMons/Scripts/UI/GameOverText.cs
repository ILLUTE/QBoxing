using UnityEngine;
public class GameOverText : MonoBehaviour
{
    private Camera m_Camera;


    private Vector3 targetPos;

    private float distance = 1;
    private Vector3 speedDetermined;
    private float smoothTime = .3f;
    private float rotationSpeed = 5f;
    private void OnEnable()
    {
        m_Camera = Camera.main;
    }
    void Update()
    {
        if (m_Camera == null) return;

        targetPos = m_Camera.transform.position + m_Camera.transform.forward * distance;

        transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, targetPos, ref speedDetermined, smoothTime), Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - m_Camera.transform.position), Time.deltaTime * rotationSpeed));
    }
}
