using UnityEngine;
using System;
using System.Collections.Generic;

public class Profile: MonoBehaviour
{
    public struct ProfilePoint
    {
        public DateTime lastRecorded;
        public TimeSpan totalTime;
        public int totalCalls;
    }

    private static Dictionary<string, ProfilePoint> profiles = new Dictionary<string, ProfilePoint>();
    private static DateTime startTime = DateTime.UtcNow;

    public static void StartProfile(string tag)
    {
        ProfilePoint point;

        profiles.TryGetValue(tag, out point);
        point.lastRecorded = DateTime.UtcNow;
        profiles[tag] = point;
    }

    public static void EndProfile(string tag)
    {
        if (!profiles.ContainsKey(tag))
        {
            Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
            return;
        }
        ProfilePoint point = profiles[tag];
        point.totalTime += DateTime.UtcNow - point.lastRecorded;
        ++point.totalCalls;
        profiles[tag] = point;
    }

    public static void Reset()
    {
        profiles.Clear();
        startTime = DateTime.UtcNow;
    }

    public static void PrintResults()
    {
        TimeSpan endTime = DateTime.UtcNow - startTime;
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        output.Append("============================\n\tProfile results:\n============================\n");
        foreach (KeyValuePair<string, ProfilePoint> pair in profiles)
        {
            double totalTime = pair.Value.totalTime.TotalSeconds;
            int totalCalls = pair.Value.totalCalls;
            if (totalCalls < 1) continue;
            output.Append("\nProfile ");
            output.Append(pair.Key);
            output.Append(" took ");
            output.Append(totalTime.ToString("F5"));
            output.Append(" seconds to complete over ");
            output.Append(totalCalls);
            output.Append(" iteration");
            if (totalCalls != 1) output.Append("s");
            output.Append(", averaging ");
            output.Append((totalTime / totalCalls).ToString("F5"));
            output.Append(" seconds per call");
        }
        output.Append("\n\n============================\n\tTotal runtime: ");
        output.Append(endTime.TotalSeconds.ToString("F3"));
        output.Append(" seconds\n============================");
        Debug.Log(output.ToString());
    }

    void Start()
    {
        Console.CommandHandlers += Console_CommandHandlers;
    }

    void Console_CommandHandlers(string[] parts)
    {
        switch (parts[0])
        {
            case "profiler": PrintResults(); break;
        }
    }

    void OnApplicationQuit()
    {
        PrintResults();
    }

}