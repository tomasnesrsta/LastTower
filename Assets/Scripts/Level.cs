using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int level;

    public string[] levelMap;
    
    public List<string> enemies = new List<string>();
    public List<float> delays = new List<float>();

    public Point spawnerPoint;
    public Point despawnerPoint;

    public static Level ReadFromFile(int level)
    {
        return JsonUtility.FromJson<Level>(Resources.Load<TextAsset>("Level" + level).text);
    }
}
