using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // This script needs to be on Camera Offset.

    [SerializeField] private float shakeDuration = 0.3f;

    private Coroutine _shakeRoutine = null;

    private Vector3 originalPosition;
    public void Start()
    {
        originalPosition = transform.localPosition;
    }
    public void ShakeCamera(float threshold = 0.1f)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeCoroutine(threshold));
    }

    private IEnumerator ShakeCoroutine(float shakeMagnitude)
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
