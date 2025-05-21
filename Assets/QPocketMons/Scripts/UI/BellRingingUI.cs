using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BellRingingUI : MonoBehaviour
{
    [SerializeField] private RectTransform layout;
    private void OnEnable()
    {
        layout.DOKill();
        layout.DOAnchorPosY(0, 0.5f);
    }

    private void OnDisable()
    {
        layout.anchoredPosition = Vector2.down * 200;
    }
}
