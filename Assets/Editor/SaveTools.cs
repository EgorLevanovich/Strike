using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveTools : Editor
{
    [MenuItem("Tools/Clear Saves")]
    public static void ClearSaves()
    {
        System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(Application.persistentDataPath);

        foreach (FileInfo file in myDirInfo.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo directory in myDirInfo.GetDirectories())
        {
            directory.Delete(true);
        }

        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/Open Saves")]
    public static void OpenSaves()
    {
        Process.Start("explorer.exe", $"\"{Application.persistentDataPath}\"");
    }
}
