using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RengeGames.HealthBars;
public class EnemyHealthUI : MonoBehaviour
{
    public RadialSegmentedHealthBar healthBar;
    public void UpdateHealth(float health)
    {
        healthBar.SetPercent(health);
    }
}
