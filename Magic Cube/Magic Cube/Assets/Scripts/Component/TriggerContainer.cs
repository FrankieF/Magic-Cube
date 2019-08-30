using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerContainer : MonoBehaviour
{
    [SerializeField] private int _Axis;
    private List<SelectionTriggerController> _Triggers = new List<SelectionTriggerController>();

    private void OnEnable()
    {
        if (_Triggers.Count == 0)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
             var trigger = transform.GetChild(i).GetComponent<SelectionTriggerController>();
                if (trigger != null)
                {
                    _Triggers.Add(trigger);
                }
            }
        }
        TriggerManager.RegisterTriggerContainer(this, _Axis);
    }

    private void OnDisable()
    {
        TriggerManager.UnregisterTriggerContainer(this, _Axis);
    }

    public List<SelectionTriggerController> GetTriggers()
    {
        return _Triggers;
    }
}
