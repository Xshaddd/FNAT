using UnityEngine;

[CreateAssetMenu(fileName = "AIValuesObject", menuName = "FNAT/AIValuesObject")]
public class AIValuesObject : ScriptableObject
{
    [Range(0, 20)] public int AckeAggression;
    [Range(0, 20)] public int JokerAggression;
    [Range(0, 20)] public int IgorAggression;
    [Range(0, 20)] public int MazikAggression;
    public int NightNumber;
}
