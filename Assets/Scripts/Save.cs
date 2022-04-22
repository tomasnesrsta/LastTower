using System.IO;
using UnityEngine;

[System.Serializable]
public class Save
{
    public bool[] completedLevels;
    public float volume;
    public bool mute;

    public Save(bool[] completedLevels, float volume, bool mute)
    {
        this.completedLevels = completedLevels;
        this.volume = volume;
        this.mute = mute;
    }

    public void SaveToFile(string fileName)
    {
        File.WriteAllText(fileName, JsonUtility.ToJson(this));
    }

    public static Save LoadFromFile(string fileName)
    {
        string s = File.ReadAllText(fileName);
        return JsonUtility.FromJson<Save>(s);
    }
}
