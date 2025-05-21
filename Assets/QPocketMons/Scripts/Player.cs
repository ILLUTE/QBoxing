using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public HandPunch leftHand;
    public HandPunch rightHand;

    [SerializeField] private Image healthUI;
    [SerializeField] private GameObject gameOverText;

    [SerializeField] private CameraShake cameraOffset;

    public bool IsPlayerBlocking = false;

    public Health health;

    private Camera cam;
    private Camera m_Camera
    {
        get
        {
            if (cam == null)
            {
                cam = Camera.main;
            }

            return cam;
        }
    }

    private readonly string[] hurtSFX = new[] { "Hurt1", "Hurt2", "Hurt3", "Hurt4", "Hurt5", "Hurt6", "Hurt7", "Hurt8", "Hurt9", "Hurt10" };

    private string GetRandomHurtSFX()
    {
        int x = UnityEngine.Random.Range(0, hurtSFX.Length - 1);

        return hurtSFX[x];
    }

    private void Start()
    {
        health.OnHealthUpdate.AddListener(UpdateHealth);
    }

    private void UpdateHealth(float curHealthValue)
    {
        leftHand.SetPlayerHealthOnGloves(curHealthValue);
        rightHand.SetPlayerHealthOnGloves(curHealthValue);
        healthUI.color = healthUI.color.SetAlpha(1 - curHealthValue);
        gameOverText.SetActive(curHealthValue <= 0);
    }

    private void Update()
    {
        if (m_Camera == null)
        {
            return;
        }

        Vector3 trackedPosition = m_Camera.transform.position;
        transform.position = trackedPosition;
    }

    public void SetDamage(int damage)
    {
        if (IsPlayerBlocking) return;
        health.SetDamage(damage);
        AudioManager.Instance.PlayOneShot(GetRandomHurtSFX(), transform.position);
        cameraOffset.ShakeCamera();
    }

    public void ResetGame()
    {
        health.ResetHealth();
    }
}
