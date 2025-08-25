using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    public string savePath { get; private set; }

    public void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to " + savePath);
    }

    public SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded from " + savePath);
            return data;
        }
        else
        {
            Debug.LogWarning($"No save file found. Returning default data and creating new file at {savePath}.");
            string json = JsonUtility.ToJson(new SaveData(), true);
            File.WriteAllText(savePath, json);
            return new SaveData();
        }
    }
}