using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;

    public UnityEvent<float> OnHealthUpdate;

    public bool IsDead
    {
        get
        {
            return CurrentHealth <= 0;
        }
    }
    private void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        OnHealthUpdate?.Invoke(CurrentHealth / MaxHealth);
    }
    public void SetDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        OnHealthUpdate?.Invoke((float)CurrentHealth / MaxHealth);
    }
}
