using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFrame : MonoBehaviour
{
    public int index;
    public Image selectionFrame;

    public static CharacterFrame selectedFrame;

    public void Select()
    {
        if (selectedFrame != null)
        {
            selectedFrame.selectionFrame.enabled = false;
        }

        selectedFrame = this;
        selectionFrame.enabled = this;
        GameManager.Instance.EnemyIndex = index;
    }

    public void Unselect()
    {
        selectionFrame.enabled = false;
        selectedFrame = null;
    }
}
