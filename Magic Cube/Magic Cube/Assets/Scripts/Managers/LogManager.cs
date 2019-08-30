using UnityEngine;

public class LogManager
{
    private static bool _logsMessages = true;
    private static bool _logErrors = true;
    
    internal LogManager()
    {
        Debug.LogError("LogManager has been initialized!");
    }

    public static void SetLogMessages(bool on)
    {
        _logsMessages = on;
    }

    public static void SetLogErrors(bool on)
    {
        _logErrors = on;
    }
    
    public static void LogError(string message)
    {
        if (_logErrors)
        {
            Debug.LogError(message);
        }
    }

    public static void LogMessage(string message)
    {
        if (_logsMessages)
        {
            Debug.Log(message);
        }
    }
}
