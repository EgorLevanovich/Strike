using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private static int _enemysKilled = 0;

    public static int EnemysKilled
    {
        get { return _enemysKilled; }
        set
        {
            _enemysKilled = value;
            PlayerPrefs.SetInt("EnemiesKilled", _enemysKilled);
        }


    }

    static GameData()
    {
        _enemysKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);
    }

    public static void ResetData()
    {
        EnemysKilled = 0;
        PlayerPrefs.DeleteKey("EnemiesKilled");
    }
}
