using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventsHandler : MonoBehaviour
{
    public event Action<string> OnAnimationEventCalled;

    public void OnAnimationEvent(string animationName)
    {
        OnAnimationEventCalled?.Invoke(animationName);
    }
}
