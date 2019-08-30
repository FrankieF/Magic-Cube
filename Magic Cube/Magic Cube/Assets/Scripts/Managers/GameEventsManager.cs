using System;
using System.Collections.Generic;

public class GameEventsManager
{
    private static Dictionary<string, List<Action>> _events = new Dictionary<string, List<Action>>();
    
    internal GameEventsManager()
    {
        LogManager.LogError("GameEventsManager was initialized!");
    }

    public static void Register(string eventName, Action action)
    {
        if (_events.ContainsKey(eventName))
        {
            _events[eventName].Add(action);
        }
        else
        {
            _events[eventName] = new List<Action>();
            _events[eventName].Add(action);
        }
    }

    public static void Unregister(string eventName, Action action)
    {
        if (eventName.Contains(eventName))
        {
            if (_events[eventName].Contains(action))
            {
                _events[eventName].Remove(action);
            }
            else
            {
                LogManager.LogError($"Tried to unregister {action} for {eventName} but action was not registered for event.");
            }
        }
        else
        {
            LogManager.LogError($"Tried to unregister for event {eventName}, but event is not registered.");
        }
    }

    public static void BroadcastMessage(string message)
    {
        if (!_events.ContainsKey(message)) { return; }
        var actions = _events[message];
        foreach (var action in actions)
        {
            action?.Invoke();
        }
    }
}
