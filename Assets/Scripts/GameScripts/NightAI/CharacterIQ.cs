using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class CharacterIQ
{
    public CharacterDataObject CharacterData;
    public int IQ;
    public MapNode CurrentNode;

    public CharacterIQ(CharacterDataObject characterData, int iq)
    {
        CharacterData = characterData;
        IQ = iq;
        CurrentNode = characterData.StartingNode;
    }

    public virtual void MoveForward()
    {
        bool tryToUpdate = false;
        var validNextNodes = CurrentNode.NextNodes
           .Where(node => CharacterData.CharacterNodes.Contains(node))
           .ToList();
        MapNode viewedCamera = CameraModeController.Instance.mapNodes[CameraModeController.Instance.currentCameraIndex];
        if (CurrentNode == viewedCamera) // set flag to update presence display or character displays (latter unimplemented as of a0.40)
        {
            tryToUpdate = true;
        }
        if (validNextNodes.Count > 0)
        {
            MapNode prevNode = CurrentNode;
            CurrentNode = validNextNodes[Random.Range(0, validNextNodes.Count)];

            foreach (MapNode node in IQController.Instance.footStepsNodesIndexes) 
            {
                if (CurrentNode == node)
                {
                    IQController.Instance.FootStepsSource.Play();
                }
            }

            Debug.Log($"[IQController] {CharacterData.Name} is advancing to {CurrentNode}");

            if (CurrentNode == CameraModeController.Instance.mapNodes[CameraModeController.Instance.currentCameraIndex])
            {
                tryToUpdate = true;
            }

            if (CurrentNode == IQController.Instance.Office)
            {
                if (CharacterData.Name == "Acke") // Acke kills instantly unlike Joker or Igor
                {
                    GameSceneController.Instance.SetOfficeMode();
                    GameSceneController.Instance.StopGame(CharacterData.Jumpscare, CharacterData.JumpscareAlpha);
                }
                else // Joker or Igor sequences
                {
                    if (prevNode == IQController.Instance.DoorNode)
                        GameSceneController.Instance.isDoorBlocked = true;
                    else if (prevNode == IQController.Instance.VentNode)
                        GameSceneController.Instance.isTrapDoorBlocked = true;

                    GameSceneController.Instance.ItIsSoOver(CharacterData.Jumpscare, CharacterData.JumpscareAlpha);
                }
            }

            if ((CurrentNode == viewedCamera || prevNode == viewedCamera) && 
                GameSceneController.Instance.currentMode == GameSceneController.Mode.Camera)
            {
                CameraModeController.Instance.BreakCamera();
            }
            if (tryToUpdate)
            {
                CameraModeController.Instance.UpdatePresenceDisplay(CameraModeController.Instance.currentCameraIndex);
                CameraModeController.Instance.RenderCharacters(CameraModeController.Instance.currentCameraIndex);
            }
                
        }
    }

    public void FallBack(string type)
    {
        CurrentNode = CharacterData.FallBackNodes[Random.Range(0, CharacterData.FallBackNodes.Length)];

        AudioSource soundPlayer = GameObject.Find("DoorLeaveSource").GetComponent<AudioSource>();
        soundPlayer.clip = CharacterData.LeaveClip;
        soundPlayer.Play();

        Debug.Log($"[CharacterIQ] {CharacterData.Name} Tried to enter office by {type} but failed! Falling back to {CurrentNode}");
    }

    public void MoveBackward() //unused currently
    {
        if (CurrentNode.PrevNodes.Count > 0)
        {
            CurrentNode = CurrentNode.PrevNodes[Random.Range(0, CurrentNode.PrevNodes.Count)];
        }
    }
}
