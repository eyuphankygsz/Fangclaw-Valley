using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField]
    private List<UnityEvent> _events;

    public void StartEvents(int id)
    {
        _events[id]?.Invoke();
    }
}
