using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image bloodSplat;
    public GameObject gameOverTxt;

    public AnimationCurve animationCurve;

    public void UpdateHealth(float health)
    {
       // bloodSplat.color = bloodSplat.color.SetAlpha(animationCurve.Evaluate(1 - health));
        //gameOverTxt.SetActive(health <= 0);        
    }
}
