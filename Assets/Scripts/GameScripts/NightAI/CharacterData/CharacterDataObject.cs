using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "CharacterDataObject", menuName = "FNAT/CharacterDataObject")]
public class CharacterDataObject : ScriptableObject
{
    public string Name;
    public MapNode StartingNode;
    public MapNode[] CharacterNodes;
    public MapNode[] FallBackNodes;
    public AudioClip FailClip;
    public AudioClip LeaveClip;
    public Texture2D FailSprite;
    public VideoClip Jumpscare;
    public VideoClip JumpscareAlpha;


    public RenderDataObject[] RenderData;
    public GameObject RenderImagePrefab;
}
