using UnityEngine;

public class DisableDebugOnBuild : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void DisableLogger()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }
}
