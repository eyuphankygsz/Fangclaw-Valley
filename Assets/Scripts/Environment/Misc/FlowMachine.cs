using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowMachine : MonoBehaviour
{
    [SerializeField]
    private List<Flow> _flows;

    void Update()
    {
        for (int i = 0; i < _flows.Count; i++)
            _flows[i].GoToTarget();
    }
}
