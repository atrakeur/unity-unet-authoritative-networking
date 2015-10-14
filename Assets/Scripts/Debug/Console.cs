using UnityEngine;
using System;
using System.Collections;

public class Console : MonoBehaviour {

    private static bool showed;

    private static string commandBuffer = "";
    private static string consoleBuffer = "";

    public delegate void CommandHandler(string[] parts);
    public static event CommandHandler CommandHandlers;

    void Awake()
    {
        Application.logMessageReceived += Application_logMessageReceived;

        Console.CommandHandlers += Console_CommandHandlers;
    }

    void Console_CommandHandlers(string[] parts)
    {
        switch (parts[0])
        {
            case "console": showed = ParseBoolean(parts[1]); break;
        }
    }

    void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Log || type == LogType.Warning)
        {
            consoleBuffer = condition + "\n" + consoleBuffer;
        }
        else
        {
            consoleBuffer = condition + "\n" + consoleBuffer;
            consoleBuffer = stackTrace + "\n" + consoleBuffer;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F12))
        {
            showed = !showed;
        }
    }

    Vector2 scrollPosition;
    void OnGUI()
    {
        if (!showed)
        {
            return;
        }

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height / 2));
            GUILayout.BeginVertical();
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                    GUILayout.TextArea(consoleBuffer, new GUILayoutOption[]{GUILayout.ExpandHeight(true)});
                GUILayout.EndScrollView();
                GUILayout.BeginHorizontal();
                    commandBuffer = GUILayout.TextField(commandBuffer);
                    if (GUILayout.Button("Run", new GUILayoutOption[]{GUILayout.Width(Screen.width / 6)})) {
                        ExecuteCommand(commandBuffer);
                        consoleBuffer = " -> " + commandBuffer + "\n" + consoleBuffer;
                        commandBuffer = "";
                        scrollPosition = Vector2.left * 1000;
                    }
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public static void ExecuteCommand(string command)
    {
        string[] parts = command.Split(' ');

        if (parts.Length == 0)
        {
            return;
        }

        CommandHandlers(parts);
    }

    /// <summary>
    /// Return
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public static bool ParseBoolean(string part)
    {
        part = part.ToLower();
        if (part == "true" || part == "yes" || part == "1" || part == "on")
        {
            return true;
        }
        else if (part == "false" || part == "no" || part == "0" || part == "off")
        {
            return false;
        }

        throw new Exception("Argument '"+part+"' not reconized as boolean");
    }

}
