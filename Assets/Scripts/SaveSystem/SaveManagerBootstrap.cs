using UnityEngine;

public class SaveManagerBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitSaveManager()
    {
        if (SaveManager.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("SaveManager");
            if (prefab != null)
            {
                GameObject instance = Object.Instantiate(prefab);
                instance.name = "SaveManager";
                Object.DontDestroyOnLoad(instance); 
            }
            else
            {
                Debug.LogError("SaveManager prefab not found in Resources!");
            }
        }
    }
}
