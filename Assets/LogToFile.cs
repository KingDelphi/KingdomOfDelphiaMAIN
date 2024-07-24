using UnityEngine;
using System.IO;

public class LogToFile : MonoBehaviour
{
    private StreamWriter logWriter;

    void Awake()
    {
        // Ensure that this object is not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);

        // Define the path and name of the log file
        string logFilePath = Path.Combine(Application.persistentDataPath, "UnityLogs.txt");

        // Print the path to the console
        Debug.Log("Log file path: " + logFilePath);

        // Create or open the log file
        logWriter = new StreamWriter(logFilePath, true);
        logWriter.AutoFlush = true;

        // Register the log callback
        Application.logMessageReceived += HandleLog;

        // Optionally, write a message to indicate the start of a new session
        logWriter.WriteLine("----- New Log Session -----");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Write the log message to the file
        logWriter.WriteLine($"{type}: {logString}");
        if (type == LogType.Error || type == LogType.Exception)
        {
            logWriter.WriteLine(stackTrace);
        }
    }

    void OnDestroy()
    {
        // Unregister the log callback
        Application.logMessageReceived -= HandleLog;

        // Close the log file
        logWriter.Close();
    }
}
