using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Io.AppMetrica;



public class Analytics
{
   public static Analytics Instance { get; private set; }

    public Analytics()
    {
        Instance = this;
        Init();
    }

    private void Init()
    {

        var config = new AppMetricaConfig("bd3fea23-552a-4f42-8b0a-51848a21e9a2")
        {
            Logs = true,
            SessionTimeout = 300,
            FirstActivationAsUpdate = false
        };
        AppMetrica.Activate(config);
    }

    public void LevelStart(int level)
    {
        var data = new Dictionary<string, object>()
        {
            {"level_count", level}
        };
        SendEvent("level_start", data, true);
    }

    private void SendEvent(string eventName, Dictionary<string,object> parameters, bool sendBuffer = false)
    {
        var json = ToJson(parameters);
        Log(eventName, json);

        AppMetrica.ReportEvent(eventName, json);
        if(sendBuffer)
        {
            AppMetrica.SendEventsBuffer();
        }
    }

    private string ToJson(IDictionary<string, object> fields)
    {
        var text = new System.Text.StringBuilder();
        text.Append("{");
        if (fields != null)
        {
            var count = 0;
            foreach (var entry in fields)
            {
                if (entry.Value == null)
                {
                    Debug.LogWarning($"Skipped parameter {entry.Key}");
                    continue;
                }

                if (count++ > 0)
                {
                    text.Append(",");
                }

                text.Append(Stringify(entry.Key));
                text.Append(":");
                text.Append(ToString(entry.Value));
            }
        }

        text.Append("}");
        return text.ToString();
    }

    private string ToString(object value)
    {
        if (value is IDictionary<string, object> nested)
            return ToJson(nested);

        if (value is string text)
        {
            return Stringify(text);
        }

        return value.ToString();
    }

    private string Stringify(string text)
    {
        return $"\"{text}\"";
        
    }

    private void Log(string eventName, string json)
    {
        string text = $"Event: {eventName}, Time: {System.DateTime.Now}\n{json}";
        Debug.Log($"<color=yellow>{text}</color>");
    }
}
