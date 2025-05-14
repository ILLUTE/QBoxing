using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{

    public CharacterFrame[] characterFrames;
    public Canvas canvas;

   public void SelectCharacter()
    {
        GameManager.Instance.StartGame();
    }

    public void ResetSelection()
    {
        foreach (var frame in characterFrames)
        {
            frame.Unselect();   
        }
    }
}
