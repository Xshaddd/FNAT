using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int CurrentNight;

    public SaveData(int CurrentNight = 1)
    {
        this.CurrentNight = CurrentNight;
    }
}
