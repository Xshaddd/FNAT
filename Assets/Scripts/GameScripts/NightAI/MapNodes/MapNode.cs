using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapNode", menuName = "FNAT/MapNode")]
public class MapNode : ScriptableObject
{
    public string ID;
    public List<MapNode> NextNodes;
    public List<MapNode> PrevNodes;

    [TextArea]
    public string description;
}