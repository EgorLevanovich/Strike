using System.Collections.Generic;
using Assets.Scripts.Gameplay;
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

    public void LevelStart()
    {
        var container = GameplayContainer.Instance;
        var data = CreteLevelData();
        SendEvent("level_start", data, true);
    }
    
    public void LevelFinish(string result)
    {
        var data = CreteLevelData();
        data.Add("result", result);
        SendEvent("level_finish", data, true);
    }
    
    public void AdsStart(string type)
    {
        var data = new Dictionary<string, object>
        {
            { "type", type }
        };
        SendEvent("ads_start", data);
    }
    
    public void AdsFail(string type)
    {
        var data = new Dictionary<string, object>
        {
            { "type", type }
        };
        SendEvent("ads_fail", data);
    }
    
    public void AdsWatch(string type)
    {
        var data = new Dictionary<string, object>
        {
            { "type", type }
        };
        SendEvent("ads_watch", data);
    }
    
    public void AdsClicked(string type)
    {
        var data = new Dictionary<string, object>
        {
            { "type", type }
        };
        SendEvent("ads_click", data);
    }

    public void BallSelected(string name)
    {
        var data = new Dictionary<string, object>
        {
            { "name", name }
        };
        
        SendEvent("ball_select", data);
    }
    
    public void PlatformSelected(string name)
    {
        var data = new Dictionary<string, object>
        {
            { "name", name }
        };
        
        SendEvent("platform_select", data);
    }
    
    public void MapSelected(string name)
    {
        var data = new Dictionary<string, object>
        {
            { "name", name }
        };
        
        SendEvent("map_select", data);
    }

    public void EnemyColorSelected(string name)
    {
        var data = new Dictionary<string, object>
        {
            { "name", name }
        };
        
        SendEvent("enemy_color_select", data);
    }
    
    public void EnemyColorsBought()
    {
        var data = new Dictionary<string, object>();
        SendEvent("enemy_color_bought", data);
    }

    private IDictionary<string, object> CreteLevelData()
    {
        var container = GameplayContainer.Instance;
        return new Dictionary<string, object>
        {
            {"level_count", container.LevelCount},
            { "ball", container.BallName },
            { "platform", container.PlatformName},
            { "map", container.LevelName }
        };
    }

    private void SendEvent(string eventName, IDictionary<string, object> parameters, bool sendBuffer = false)
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
