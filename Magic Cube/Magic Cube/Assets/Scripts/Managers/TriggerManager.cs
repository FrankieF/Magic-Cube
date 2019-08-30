using System.Collections.Generic;
using UnityEngine;

public class TriggerManager
{
    private static SelectionTriggerController _EmptyTrigger;
    public static Dictionary<string, SelectionTriggerController> StringToTrigger = new Dictionary<string, SelectionTriggerController>();
    private static SelectionTriggerController[] _triggersAsArray;
    private static HashSet<SelectionTriggerController> _selectionTriggers = new HashSet<SelectionTriggerController>();
    private static TriggerContainer _xTriggers;
    private static TriggerContainer _yTriggers;
    private static TriggerContainer _zTriggers;

    private void OnDestroy()
    {
        ServiceManager.Unregister(this);
    }

    public HashSet<SelectionTriggerController> GetSelectionTriggers()
    {
        return _selectionTriggers;
    }

    public static void RegisterSelectionTrigger(SelectionTriggerController selectionTrigger)
    {
        if (!_selectionTriggers.Contains(selectionTrigger))
        {
            _selectionTriggers.Add(selectionTrigger);
            StringToTrigger.Add(selectionTrigger.name, selectionTrigger);
        }
        else
        {
            LogManager.LogError($"Tried to register {selectionTrigger} but was already in the registered.");
        }
    }
    
    public static void RegisterTriggerContainer(TriggerContainer container, int axis)
    {
        if (axis == Consts.X_TRIGGER_AXIS)
        {
            _xTriggers = container;
        }
        else if (axis == Consts.Y_TRIGGER_AXIS)
        {
            _yTriggers = container;
        }
        else
        {
            _zTriggers = container;
        }
        foreach (var trigger in container.GetTriggers())
        {
            StringToTrigger.Add(trigger.name, trigger);
        }
    }
    
    public static void UnregisterTriggerContainer(TriggerContainer container, int axis)
    {
        if (axis == Consts.X_TRIGGER_AXIS)
        {
            _xTriggers = null;
        }
        else if (axis == Consts.Y_TRIGGER_AXIS)
        {
            _yTriggers = null;
        }
        else
        {
            _zTriggers = null;
        }
        foreach (var trigger in container.GetTriggers())
        {
            if (StringToTrigger.ContainsKey(trigger.name))
            {
                StringToTrigger.Remove(trigger.name);
            }
        }
    }

    public static void RegisterEmptyTrigger(SelectionTriggerController trigger)
    {
        _EmptyTrigger = trigger;
    }

    public static SelectionTriggerController GetRandomTrigger(int axis)
    {
        if (axis == Consts.X_TRIGGER_AXIS)
        {
            var count = _xTriggers.GetTriggers().Count;
            return _xTriggers.GetTriggers()[Random.Range(0, count - 1)];
        }
        else if (axis == Consts.Y_TRIGGER_AXIS)
        {
            var count = _yTriggers.GetTriggers().Count;
            return _yTriggers.GetTriggers()[Random.Range(0, count - 1)];
        }
        else if (axis == Consts.Z_TRIGGER_AXIS)
        {
            var count = _zTriggers.GetTriggers().Count; 
            return _zTriggers.GetTriggers()[Random.Range(0, count - 1)];
        }
        return null;
    }
    
    public static SelectionTriggerController GetEmptyTrigger()
    {
        return _EmptyTrigger;
    }
    
}
