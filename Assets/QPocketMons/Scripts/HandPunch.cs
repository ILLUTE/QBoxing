using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandPunch : MonoBehaviour
{
    public float PunchThreshold;
    private Vector3 lastPosition;
    private float lastCoolPunchDown;

    public bool IsPunching;

    public float handRadius = 0.1f;

    public float PunchCoolDownTime = 1f;

    public LayerMask ownLayer;
    public LayerMask ignoredLayers;

    public ParticleSystem[] hitParticles;
    public ParticleSystem missParticle;

    public Handed hand;

    public Image heartFill;

    private string[] punchSFX = new[] { "Punch1", "Punch2", "Punch3", "Punch4", "Punch5", "Punch6" };
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    private string GetRandomPunchAudio()
    {
        int x = Random.Range(0, punchSFX.Length - 1);

        return punchSFX[x];
    }

    // Update is called once per frame
    void Update()
    {

        CheckForPunch();

        if (Time.time < lastCoolPunchDown + PunchCoolDownTime)
        {
            return;
        }
        if (!IsPunching)
        {
            return;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, handRadius, ~ignoredLayers);
        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out HitCollider hit))
                {
                    lastCoolPunchDown = Time.time;
                    if (hit.CallForHit())
                    {
                        PlayHit(transform.position);
                        AudioManager.Instance.PlayOneShot(GetRandomPunchAudio(), transform.position);
                    }
                    else
                    {
                        PlayMiss(transform.position);
                    }
                    break;
                }
            }
        }
    }

    public void PlayHit(Vector3 position)
    {
        foreach (ParticleSystem hit in hitParticles)
        {
            hit.transform.position = position;
            hit.Emit(1);
        }
    }

    public void PlayMiss(Vector3 position)
    {
        missParticle.Stop();
        missParticle.transform.position = position;
        missParticle.Play();
    }

    private void CheckForPunch()
    {
        Vector3 currentPosition = transform.position;
        Vector3 velocity = (currentPosition - lastPosition) / Time.deltaTime;
        Debug.Log(velocity.magnitude);
        lastPosition = currentPosition;
        Vector3 punchDirection = transform.forward; 
        // Check if the hand is moving in the punching direction
        float directionMatch = Vector3.Dot(velocity.normalized, punchDirection.normalized);
        IsPunching = (velocity.magnitude > PunchThreshold) && (directionMatch > 0.25f);
    }

    public bool IsBlocking()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, handRadius, ownLayer);
        return colliders.Length == 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, handRadius);
    }

    public void SetPlayerHealthOnGloves(float curHealth)
    {
        heartFill.fillAmount = curHealth;
    }
}

public enum Handed
{
    Left,
    Right
}

  
