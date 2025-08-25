using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RenderDataObject", menuName = "FNAT/RenderDataObject")]
public class RenderDataObject : ScriptableObject
{
    public MapNode node;
    public Sprite sprite;
}
