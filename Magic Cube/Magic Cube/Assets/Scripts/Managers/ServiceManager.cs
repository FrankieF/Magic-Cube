using System.Collections.Generic;
using UnityEngine;

public class ServiceManager
{
    private static Dictionary<object, object> _services = new Dictionary<object, object>();

    internal ServiceManager()
    {
        
    }

    public static void Register<T>(T t, Component c)
    {
        if (_services.ContainsKey(typeof(T)))
        {
            LogManager.LogError($"{typeof(T)} is already registered!");
            return;
        }
        _services.Add(typeof(T), c);
    }
    public static void Unregister<T>(T t)
    {
        if (!_services.ContainsKey(typeof(T)))
        {
            LogManager.LogError($"{typeof(T)} is not registered!");
            return;
        }
        _services.Remove(typeof(T));
    }

    public static T Get<T>() where T : Object
    {
        if (!_services.ContainsKey(typeof(T)))
        {
            LogManager.LogError($"Key {typeof(T)} not found! Returning null.");
            return default(T);
        }
        else
        {
            return (T)_services[typeof(T)];
        }
    }
}
